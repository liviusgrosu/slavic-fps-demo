using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Numerics;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [HideInInspector] public static PlayerController Instance;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float walkingSpeed = 4f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float airMultiplier = 0.4f;
    private const float MovementMultiplier = 10f;
    private float _horizontalMovement, _verticalMovement;


    [Header("Rotation")]
    [SerializeField] private Transform mainCamera = null;
    [HideInInspector] public Vector3 moveDirection;
    private Vector3 _slopeMoveDirection;
    
    [Header("Jumping")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float graceTimeMax = 1f;
    private float _graceTimeCurrent;
    private bool _isJumping;
    
    [Header("Dashing")]
    [SerializeField] private float dashSpeed = 5f;
    [SerializeField] private float dashTimeMax = 1f;
    private float _dashTimeCurrent;

    [Header("Dashing - Timers")]
    [SerializeField] public int DashMaxPoints = 3;
    [SerializeField] private float dashCooldownTime = 0.5f;
    private int _dashCurrentPoints;
    public static event Action<int> DashCooldownEvent;
    public int DashCurrentPoints
    {
        get => _dashCurrentPoints;
        set
        {
            _dashCurrentPoints = value;
            DashCooldownEvent?.Invoke(value);
        }
    }
    private float _dashCurrentCooldownTime;
    private bool _isDashing;
    private bool _canDash => DashCurrentPoints > 0 && !_isDashing;

    private Coroutine _cooldownCoroutine;

    [Header("Vaulting")]
    [SerializeField] private float minDistanceToVaultable = 0.1f;
    [SerializeField] private float vaultTimeMax = 1f;
    private float _vaultTimeCurrent;
    private Vector3 _startPoint, _middlePoint, _endPoint;
    private bool _isVaulting;

    [Header("Physics")]
    [SerializeField] private float groundDrag = 6f;
    [SerializeField] private float airDrag = 2f;
    private Vector3 _gravity;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundDistance = 0.2f;
    private RaycastHit _slopeHit;
    private float _ignoreGroundedMaxTime = 0.1f;
    private float _ignoreGroundedCurrentTime = 0.1f;
    
    [Header("Misc.")]
    [SerializeField] private InputQueueSystem inputQueue;
    public Dictionary<string, int > runningCoroutines = new Dictionary<string, int>();
    
    // Debug Events
    public static event Action<bool> IsGroundedEvent;
    public static event Action<bool> IsOnSlopeEvent;
    public static event Action<bool> IsJumpingEvent;
    public static event Action<float> GraceTimerEvent;

    // Dashing events
    public static event Action<float, float> DashTimerEvent;


    // Vaulting events
    public static event Action VaultingEvent;

    // Components
    private Rigidbody _rigidbody;
    private PlayerInput _inputManager;
    private PlayerEffects _playerEffects;
    private CapsuleCollider _collider;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
            return;
        }

        Instance = this;
        // Might not need this but might be useful
        //DontDestroyOnLoad(Instance);

        _inputManager = GetComponent<PlayerInput>();
        _collider = GetComponent<CapsuleCollider>();
        _rigidbody = GetComponent<Rigidbody>();
        _playerEffects = FindObjectOfType<PlayerEffects>();
        _rigidbody.freezeRotation = true;
        DashCurrentPoints = DashMaxPoints;
    }
    
    private void Update()
    {
        AdjustGravity();
        GetMovementInput();
        CheckGrounded();
        ControlDrag();
        ControlSpeed();
        CheckVaulting();
        UpdateDebugWindow();
        
        _slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, _slopeHit.normal);
    }

    private void FixedUpdate()
    {
        // Apply movement force not on slope
        if (PlayerState.IsGrounded && !OnSlope())
        {
            _rigidbody.AddForce(moveDirection.normalized * moveSpeed * MovementMultiplier + _gravity, ForceMode.Acceleration);
        }
        // Apply movement force on slope
        else if (PlayerState.IsGrounded && OnSlope())
        {
            _rigidbody.AddForce(_slopeMoveDirection.normalized * moveSpeed * MovementMultiplier + _gravity, ForceMode.Acceleration);
        }
        // Apply movement force when in the air
        else if (!PlayerState.IsGrounded)
        {
            _rigidbody.AddForce(moveDirection.normalized * moveSpeed * MovementMultiplier * airMultiplier + _gravity, ForceMode.Acceleration);
        }
    }

    private void CheckVaulting()
    {
        if (!PlayerState.IsGrounded && !_isVaulting && _isJumping && !PlayerState.IsAttacking && VaultableInFront())
        {
            StartCoroutine(StartVaultingTimer());
        }
    }
    
    private void AdjustGravity()
    {
        // Adjust gravity according to the surfaces normal so the player doesnt slide
        _gravity = Physics.gravity;
        
        if (OnSlope())
        {
            _gravity = -_slopeHit.normal * Physics.gravity.magnitude;
        }

        if (_isDashing)
        {
            _gravity = Vector3.zero;
        }
    }
    
    private void CheckGrounded()
    {
        // Check if a sphere collides with the ground as the ground check
        PlayerState.IsGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        PlayerState.IsGrounded = PlayerState.IsGrounded;
        if (PlayerState.IsGrounded && _ignoreGroundedCurrentTime >= _ignoreGroundedMaxTime)
        {
            // Stop any coroutines related to touching the ground
            StopCoroutine(StartIgnoreGroundedTimer());
            StopCoroutine(StartGraceTimer());

            _graceTimeCurrent = 0f;
            _isJumping = false;
        }
        else if (!PlayerState.IsGrounded && !_isJumping && _graceTimeCurrent == 0)
        {
            // Start any coroutines related not touching the ground
            StartCoroutine(StartIgnoreGroundedTimer());
            StartCoroutine(StartGraceTimer());
        }
    }
    
    private void GetMovementInput()
    {
        // Regular movement input
        if (!_isDashing)
        {
            _horizontalMovement = Input.GetAxisRaw("Horizontal");
            _verticalMovement = Input.GetAxisRaw("Vertical");
            
            moveDirection = mainCamera.forward * _verticalMovement + mainCamera.right * _horizontalMovement;
            if (PlayerState.IsGrounded)
            {
                // Added so that players can bounce off the ground when looking down
                moveDirection += new Vector3(0f, -moveDirection.y, 0f);
            }
        }
        
        // Jumping input
        if (inputQueue.MovementInputQueue.GetNextInput() == "Jump" && 
            (PlayerState.IsGrounded || _graceTimeCurrent < graceTimeMax) &&
            !_isDashing && 
            !_isJumping)
        {
            inputQueue.MovementInputQueue.DequeueInput();
            Jump();
        }
        
        // Dashing input
        if (inputQueue.MovementInputQueue.GetNextInput() == "Dash" && 
            _canDash && 
            moveDirection != Vector3.zero)
        {
            inputQueue.MovementInputQueue.DequeueInput();
            Dash();
        }
    }

    private void Jump()
    {
        // Ignore the ground so we can't jump twice
        StartCoroutine(StartIgnoreGroundedTimer());
        
        // Add the jumping force to the rigidbody
        _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z);
        _rigidbody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        
        // Ignore any grace jump timing
        _graceTimeCurrent = graceTimeMax;
        _isJumping = true;
    }

    private void Dash()
    {
        _playerEffects.PerformDashEffect(moveDirection, dashTimeMax);
        // Start dashing
        StartCoroutine(StartDashingTimer());
    }

    private void ControlSpeed()
    {
        // Dashing speed
        if (_isDashing)
        {
            moveSpeed = dashSpeed;
        }
        // Normal walking speed
        else
        {
            moveSpeed = Mathf.Lerp(moveSpeed, walkingSpeed, acceleration * Time.deltaTime);
        }
    }

    private void ControlDrag()
    {
        // Apply drag depending if the player is in the air or not
        _rigidbody.drag = PlayerState.IsGrounded && !_isDashing ? groundDrag : airDrag;
    }

    private bool OnSlope()
    {
        // Check if player is on a slope depending on the floors normal
        if (!Physics.Raycast(transform.position, Vector3.down, out _slopeHit, 1.5f))
        {
            return false;
        }
        
        return _slopeHit.normal != Vector3.up;
    }

    private bool VaultableInFront()
    {
        RaycastHit hit;

        if (!Physics.Raycast(transform.position, GetCameraForward(), out hit, minDistanceToVaultable) || 
            !hit.transform.GetComponent<BoxCollider>())
        {
            return false;
        }

        // Get forward vector
        float distanceToPoint = (hit.point - transform.position).magnitude + _collider.radius;
        Vector3 forwardDisplacement = transform.position + (GetCameraForward() * distanceToPoint);
        
        // Get upward vector
        float vaultObjectHeight = hit.transform.GetComponent<MeshRenderer>().bounds.size.y / 2f; 
        Vector3 topOfCollider = hit.transform.position + new Vector3(0f, vaultObjectHeight, 0f);

        // Get top of vault object Y
        float vaultObjectYTop = topOfCollider.y + _collider.height / 2f;
        Vector3 playerToColliderTop = new Vector3(forwardDisplacement.x, vaultObjectYTop, forwardDisplacement.z);
        float distanceToTop = Vector3.Distance(forwardDisplacement, playerToColliderTop);
        
        if (distanceToTop <= _collider.height)
        {
            _startPoint = transform.position;
            _middlePoint = transform.position + transform.up * distanceToTop;
            _endPoint = playerToColliderTop;
            return true;
        }

        return false;
    }
    
    private Vector3 GetCameraForward()
    {
        // Get the cameras forward direction in the xz plane
        Vector3 cameraForward = mainCamera.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();
        return cameraForward;
    }
    
    private static Vector3 EvaluateQuadratic(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        // Quadratic equation to lerp between 3 points
        Vector3 p0 = Vector3.Lerp(a, b, t);
        Vector3 p1 = Vector3.Lerp(b, c, t);
        return Vector3.Lerp(p0, p1, t);
    }

    private void UpdateDebugWindow()
    {
        // Update any states from this script onto a canvas for display 
        IsGroundedEvent?.Invoke(PlayerState.IsGrounded);
        IsOnSlopeEvent?.Invoke(OnSlope());
        IsJumpingEvent?.Invoke(_isJumping);
        GraceTimerEvent?.Invoke(_graceTimeCurrent);
    }

    private IEnumerator StartGraceTimer()
    {
        // Start the grace timer
        while (_graceTimeCurrent <= graceTimeMax)
        {
            _graceTimeCurrent += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator StartDashingTimer()
    {
        _isDashing = true;
        DashCurrentPoints--;

        // Store the players velocity as this will the direction of the dash
        Vector3 oldPlayerVelocity = _rigidbody.velocity;
        _rigidbody.velocity = moveDirection.normalized * dashSpeed * MovementMultiplier;
        
        _dashTimeCurrent = 0f;
        while (_dashTimeCurrent <= dashTimeMax)
        {
            DashTimerEvent?.Invoke(_dashTimeCurrent, dashTimeMax);
            _dashTimeCurrent += Time.deltaTime;
            yield return null;
        }
        
        // Reduce players velocity by a quarter as they are coming off a dash
        _isDashing = false;
        _rigidbody.velocity = oldPlayerVelocity / 4f;
        // Start delay to start dash cooldown
        if (_cooldownCoroutine != null) StopCoroutine(_cooldownCoroutine);
        _cooldownCoroutine = StartCoroutine(StartDashingCooldown());    
    }

    private IEnumerator StartDashingCooldown()
    {
        // Recharge dashing points
        _dashCurrentCooldownTime = 0f;
        while (DashCurrentPoints < DashMaxPoints)
        {
            _dashCurrentCooldownTime += Time.deltaTime;
            if (_dashCurrentCooldownTime >= dashCooldownTime)
            {
                _dashCurrentCooldownTime = 0f;
                DashCurrentPoints++;
            }
            yield return null;
        }
        _cooldownCoroutine = null;
    }

    private IEnumerator StartIgnoreGroundedTimer()
    {
        // Ignore the ground for some time
        _ignoreGroundedCurrentTime = 0f;
        while (_ignoreGroundedCurrentTime <= _ignoreGroundedMaxTime)
        {
            _ignoreGroundedCurrentTime += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator StartVaultingTimer()
    {
        ToggleVaultingParams(true);
        VaultingEvent?.Invoke();

        // Start Lerping between start and end position of vault
        _vaultTimeCurrent = 0f;
        while (_vaultTimeCurrent <= vaultTimeMax)
        {
            _vaultTimeCurrent += Time.deltaTime;
            transform.position = EvaluateQuadratic(_startPoint, _middlePoint, _endPoint, _vaultTimeCurrent / vaultTimeMax);
            yield return null;
        }

        ToggleVaultingParams(false);
    }

    private void ToggleVaultingParams(bool state)
    {
        // Toggle vaulting paramters
        _collider.isTrigger = state;
        _rigidbody.isKinematic = state;
        _isVaulting = state;
        PlayerState.IsVaulting = state;
    }
}