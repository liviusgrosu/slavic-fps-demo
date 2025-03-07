using System;
using System.Collections;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float walkingSpeed = 4f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float airMultiplier = 0.4f;
    private const float MovementMultiplier = 10f;
    private float _horizontalMovement, _verticalMovement;


    [Header("Rotation")]
    [HideInInspector] public Vector3 moveDirection;
    private Transform _mainCamera;
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
    [HideInInspector] public bool IsDashing;
    private bool _canDash => DashCurrentPoints > 0 && !IsDashing;

    private Coroutine _cooldownCoroutine;

    [Header("Vaulting")]
    [Tooltip("How much distance from the vault point to the collider will a vault trigger")]
    [SerializeField] private float vaultDistanceTolerance = 1f;
    [Tooltip("Most amount of time it takes to vault")]
    [SerializeField] private float vaultTimeMax = 1f;
    [Tooltip("Least amount of time it takes to vault")]
    [SerializeField] private float vaultTimeMin = 0.1f;
    private float _currentVaultTimeMax;
    private float _vaultTimeCurrent;
    private Vector3 _startPoint, _middlePoint, _endPoint;
    private bool _isVaulting;
    private Transform _vaultingDetectionPoint;

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
    
    // Debug Events
    public static event Action<bool> IsGroundedEvent;
    public static event Action<bool> IsOnSlopeEvent;
    public static event Action<bool> IsJumpingEvent;
    public static event Action<float> GraceTimerEvent;
    public static event Action<Vector3> RigidbodySpeedEvents;

    // Dashing events
    public static event Action<float, float> DashTimerEvent;

    // Vaulting events
    public static event Action VaultingEvent;
    public static event Action<bool> IsVaultingEvent;
    public static event Action<float, float> VaultTimeEvent;

    // Components
    private Rigidbody _rigidbody;
    private PlayerEffects _playerEffects;
    private CapsuleCollider _collider;

    private Vector3 _startingPosition;
    private Quaternion _startingRotation;

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
         
        _collider = GetComponent<CapsuleCollider>();
        _rigidbody = GetComponent<Rigidbody>();
        _playerEffects = FindFirstObjectByType<PlayerEffects>();
        _vaultingDetectionPoint = transform.Find("Vault Detection Point");
        _rigidbody.freezeRotation = true;
        DashCurrentPoints = DashMaxPoints;

        _startingPosition = transform.position;
        _startingRotation = transform.rotation;
    }

    private void Start()
    {
        Respawner.TriggerRestart += ResetPlayer;

        _mainCamera = Camera.main.transform;
    }

    private void Update()
    {
        if (PlayerState.IsDead)
        {
            return;
        }

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

        RigidbodySpeedEvents?.Invoke(_rigidbody.linearVelocity);
    }
    
    private void GetMovementInput()
    {
        // Regular movement input
        if (!IsDashing)
        {
            _horizontalMovement = Input.GetAxisRaw("Horizontal");
            _verticalMovement = Input.GetAxisRaw("Vertical");

            var camNotY = Vector3.ProjectOnPlane(_mainCamera.forward, Vector3.up).normalized;
            
            moveDirection = camNotY * _verticalMovement + _mainCamera.right * _horizontalMovement;
            if (PlayerState.IsGrounded)
            {
                // Added so that players can bounce off the ground when looking down
                moveDirection += new Vector3(0f, -moveDirection.y, 0f);
            }
        }
        
        // Jumping input
        if (InputQueueSystem.Instance.MovementInputQueue.GetNextInput() == "Jump" && 
            (PlayerState.IsGrounded || _graceTimeCurrent < graceTimeMax) &&
            !IsDashing && 
            !_isJumping)
        {
            InputQueueSystem.Instance.MovementInputQueue.DequeueInput();
            Jump();
        }
        
        // Dashing input
        if (InputQueueSystem.Instance.MovementInputQueue.GetNextInput() == "Dash" && 
            _canDash &&
            !_isVaulting &&
            moveDirection != Vector3.zero)
        {
            InputQueueSystem.Instance.MovementInputQueue.DequeueInput();
            Dash();
        }
    }

    #region Physics
    private IEnumerator StartGraceTimer()
    {
        // Start the grace timer
        while (_graceTimeCurrent <= graceTimeMax)
        {
            _graceTimeCurrent += Time.deltaTime;
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
    private void Jump()
    {
        // Ignore the ground so we can't jump twice
        StartCoroutine(StartIgnoreGroundedTimer());

        // Add the jumping force to the rigidbody
        _rigidbody.linearVelocity = Vector3.ProjectOnPlane(_rigidbody.linearVelocity, Vector3.up);
        _rigidbody.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        // Ignore any grace jump timing
        _graceTimeCurrent = graceTimeMax;
        _isJumping = true;
    }

    private void ControlSpeed()
    {
        // Dashing speed
        if (IsDashing)
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
        _rigidbody.linearDamping = PlayerState.IsGrounded && !IsDashing ? groundDrag : airDrag;
    }

    private bool OnSlope()
    {
        // Check if player is on a slope depending on the floors normal
        if (!Physics.Raycast(transform.position, Vector3.down, out _slopeHit, 1.5f, ~LayerMask.GetMask("Ignore Ledge")))
        {
            return false;
        }

        return _slopeHit.normal != Vector3.up;
    }
    private void AdjustGravity()
    {
        // Adjust gravity according to the surfaces normal so the player doesnt slide
        _gravity = Physics.gravity;

        if (OnSlope())
        {
            _gravity = -_slopeHit.normal * Physics.gravity.magnitude;
        }

        if (IsDashing)
        {
            _gravity = Vector3.zero;
        }
    }

    private void CheckGrounded()
    {
        // Check if a sphere collides with the ground as the ground check
        // TODO: might change this to avoid certain layers rather then looking for a layer
        var groundBelow = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (groundBelow && !PlayerState.IsGrounded)
        {
            //PlayerWalkingSound.Instance.TriggerLandingSound();
        }
        PlayerState.IsGrounded = groundBelow;
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
    #endregion

    #region Dashing
    private void Dash()
    {
        _playerEffects.PerformDashEffect(moveDirection, dashTimeMax);
        SoundManager.Instance.PlaySoundFXClip("Dash", transform);
        // Start dashing
        StartCoroutine(StartDashingTimer());
    }

    private IEnumerator StartDashingTimer()
    {
        IsDashing = true;
        DashCurrentPoints--;

        // Store the players velocity as this will the direction of the dash
        Vector3 oldPlayerVelocity = _rigidbody.linearVelocity;
        // We do this because we ignore y axis when moving in air but we want it for dashing
        var moveDirectionWithY = _mainCamera.forward * _verticalMovement
                                + _mainCamera.right * _horizontalMovement;
        _rigidbody.linearVelocity = moveDirectionWithY.normalized * dashSpeed * MovementMultiplier;

        _dashTimeCurrent = 0f;
        while (_dashTimeCurrent <= dashTimeMax)
        {
            DashTimerEvent?.Invoke(_dashTimeCurrent, dashTimeMax);
            _dashTimeCurrent += Time.deltaTime;
            yield return null;
        }

        // Reduce players velocity by a quarter as they are coming off a dash
        IsDashing = false;
        _rigidbody.linearVelocity = oldPlayerVelocity / 4f;
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
    #endregion

    #region Vaulting
    private void CheckVaulting()
    {
        if (!PlayerState.IsGrounded &&
            !PlayerState.InCombat &&
            !_isVaulting &&
            _isJumping &&
            VaultableInFront())
        {
            StartCoroutine(StartVaultingTimer());
        }
    }

    private void ToggleVaultingParams(bool state)
    {
        // Toggle vaulting paramters
        _collider.isTrigger = state;
        _rigidbody.isKinematic = state;
        _isVaulting = state;
        IsVaultingEvent?.Invoke(state);
        PlayerState.IsVaulting = state;
    }

    private IEnumerator StartVaultingTimer()
    {
        ToggleVaultingParams(true);
        //VaultingEvent?.Invoke();

        // Start Lerping between start and end position of vault
        _vaultTimeCurrent = 0f;
        while (_vaultTimeCurrent <= _currentVaultTimeMax)
        {
            _vaultTimeCurrent += Time.deltaTime;
            transform.position = EvaluateQuadratic(_startPoint, _middlePoint, _endPoint, _vaultTimeCurrent / _currentVaultTimeMax);
            yield return null;
        }

        ToggleVaultingParams(false);
    }

    private bool VaultableInFront()
    {
        // Check from top forward of the player to see if there is a vaultable object in front of the player
        if (!Physics.SphereCast(_vaultingDetectionPoint.position, 0.3f, -Vector3.up, out var hit, vaultDistanceTolerance, LayerMask.GetMask("Environment")) ||
            hit.normal.y < 0.6f)
        {
            return false;
        }

        _startPoint = transform.position;
        _endPoint = hit.point + Vector3.up * _collider.height / 2f;
        _middlePoint = Vector3.ProjectOnPlane(transform.position - _endPoint, Vector3.up) + _endPoint;

        // TODO: Need to introduce the min when calculating the ratio instead of clamping it
        _currentVaultTimeMax = (vaultDistanceTolerance - (hit.point - _vaultingDetectionPoint.position).magnitude) / vaultDistanceTolerance * vaultTimeMax;
        _currentVaultTimeMax = Mathf.Max(_currentVaultTimeMax, vaultTimeMin);
        VaultTimeEvent?.Invoke(_currentVaultTimeMax, vaultTimeMax);
        return true;
    }

    #endregion

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

    private void ResetPlayer()
    {
        transform.position = _startingPosition;
        transform.rotation = _startingRotation;
    }
}