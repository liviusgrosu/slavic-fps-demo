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
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float airMultiplier = 0.4f;
    private const float MovementMultiplier = 10f;
    private float _horizontalMovement, _verticalMovement;
 
    [Header("Rotation")]
    [SerializeField] private Transform mainCamera = null;
    [HideInInspector] public Vector3 moveDirection;
    private Vector3 _slopeMoveDirection;
    
    [Header("Sprinting")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float sprintSpeed = 6f;
    [SerializeField] private float acceleration = 10f;

    [Header("Jumping")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float graceTimeMax = 1f;
    private float _graceTimeCurrent;
    private bool _isJumping;
    
    [Header("Dashing")]
    [SerializeField] private float dashSpeed = 5f;
    [SerializeField] private float dashTimeMax = 1f;
    [SerializeField] private float dashCooldownTimeMax = 1f;
    private float _dashTimeCurrent;
    private float _dashTimeCooldownCurrent;
    private Vector3 _lockedMovementDirection;
    private bool _isDashing;
    
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
    [HideInInspector] public bool isGrounded;
    private RaycastHit _slopeHit;
    private float _ignoreGroundedMaxTime = 0.1f;
    private float _ignoreGroundedCurrentTime = 0.1f;
    
    [Header("Misc.")]
    [SerializeField] private InputQueueSystem inputQueue;
    
    // Debug Events
    public static event Action<bool> IsGroundedEvent;
    public static event Action<bool> IsOnSlopeEvent;
    public static event Action<bool> IsJumpingEvent;
    public static event Action<float> GraceTimerEvent;
    public static event Action<float> DashDebugCooldownEvent;
    
    // Dashing events
    public static event Action<float, float> DashTimerEvent;
    public static event Action<float, float> DashCooldownTimerEvent;
    
    // Vaulting events
    public static event Action VaultingEvent;
    public static event Action DashingEvent;

    // Components
    private Rigidbody _rigidbody;
    private PlayerInput _inputManager;
    private PlayerEffects _playerEffects;
    private CapsuleCollider _collider;

    private void Awake()
    {
        _inputManager = GetComponent<PlayerInput>();
        _collider = GetComponent<CapsuleCollider>();
        _rigidbody = GetComponent<Rigidbody>();
        _playerEffects = FindObjectOfType<PlayerEffects>();
        _rigidbody.freezeRotation = true;
        _dashTimeCooldownCurrent = dashCooldownTimeMax;
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
        if (isGrounded && !OnSlope())
        {
            _rigidbody.AddForce(moveDirection.normalized * moveSpeed * MovementMultiplier + _gravity, ForceMode.Acceleration);
        }
        // Apply movement force on slope
        else if (isGrounded && OnSlope())
        {
            _rigidbody.AddForce(_slopeMoveDirection.normalized * moveSpeed * MovementMultiplier + _gravity, ForceMode.Acceleration);
        }
        // Apply movement force when in the air
        else if (!isGrounded)
        {
            _rigidbody.AddForce(moveDirection.normalized * moveSpeed * MovementMultiplier * airMultiplier + _gravity, ForceMode.Acceleration);
        }
    }

    private void CheckVaulting()
    {
        if (!isGrounded && !_isVaulting && _isJumping && VaultableInFront())
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
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && _ignoreGroundedCurrentTime >= _ignoreGroundedMaxTime)
        {
            // Stop any coroutines related to touching the ground
            StopCoroutine(StartIgnoreGroundedTimer());
            StopCoroutine(StartGraceTimer());

            _graceTimeCurrent = 0f;
            _isJumping = false;
        }
        else if (!isGrounded && !_isJumping && _graceTimeCurrent == 0)
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
            if (isGrounded)
            {
                // Added so that players can bounce off the ground when looking down
                moveDirection += new Vector3(0f, -moveDirection.y, 0f);
            }
        }
        
        // // Jumping input
        if (inputQueue.MovementInputQueue.GetNextInput() == "Jump" && (isGrounded || _graceTimeCurrent < graceTimeMax) &&
            !_isDashing)
        {
            inputQueue.MovementInputQueue.DequeueInput();
            Jump();
        }
        
        // Dashing input
        if (inputQueue.MovementInputQueue.GetNextInput() == "Dash" && _dashTimeCooldownCurrent >= dashCooldownTimeMax && !_isDashing && moveDirection != Vector3.zero)
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
        DashingEvent?.Invoke();
        _playerEffects.PerformDashEffect(moveDirection, dashTimeMax);
        // Start dashing
        StartCoroutine(StartDashingTimer());
    }

    private void ControlSpeed()
    {
        // Change move speed if player is pressing on the spring key
        if (Input.GetKey(_inputManager.SprintKey) && isGrounded)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, acceleration * Time.deltaTime);
        }
        // Dashing speed
        else if (_isDashing)
        {
            moveSpeed = dashSpeed;
        }
        // Normal walking speed
        else
        {
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, acceleration * Time.deltaTime);
        }
    }

    private void ControlDrag()
    {
        // Apply drag depending if the player is in the air or not
        _rigidbody.drag = isGrounded && !_isDashing ? groundDrag : airDrag;
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
        IsGroundedEvent?.Invoke(isGrounded);
        IsOnSlopeEvent?.Invoke(OnSlope());
        IsJumpingEvent?.Invoke(_isJumping);
        GraceTimerEvent?.Invoke(_graceTimeCurrent);
        DashDebugCooldownEvent?.Invoke(_dashTimeCooldownCurrent);
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
        // Store the players velocity as this will the direction of the dash
        Vector3 oldPlayerVelocity = _rigidbody.velocity;
        _rigidbody.velocity = moveDirection.normalized * dashSpeed * MovementMultiplier;
        
        _isDashing = true;
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
        // Start cooldown of dash
        StartCoroutine(StartDashingCooldown());
    }

    private IEnumerator StartDashingCooldown()
    {
        // Perform the dashing process for some time
        _dashTimeCooldownCurrent = 0f;
        while (_dashTimeCooldownCurrent <= dashCooldownTimeMax)
        {
            _dashTimeCooldownCurrent += Time.deltaTime;
            DashCooldownTimerEvent?.Invoke(_dashTimeCooldownCurrent, dashCooldownTimeMax);
            yield return null;
        }
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
    }

    public float GetMovemenetSpeedPercent()
    {
        return moveSpeed / sprintSpeed;
    }
}