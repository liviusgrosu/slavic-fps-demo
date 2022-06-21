using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Numerics;
using UnityEditor;
using UnityEngine;
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
    [SerializeField] private Transform orientation;
    private Vector3 _moveDirection;
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
    private float _dashTimeCurrent;
    private Vector3 _lockedMovementDirection;
    private bool _isDashing;
    
    [Header("Vaulting")]
    [SerializeField] private float minDistanceToVaultable = 0.1f;
    
    [Header("Physics")]
    [SerializeField] private float groundDrag = 6f;
    [SerializeField] private float airDrag = 2f;
    private Vector3 _gravity;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundDistance = 0.2f;
    private bool _isGrounded;
    private RaycastHit _slopeHit;
    private float _ignoreGroundedMaxTime = 0.1f;
    private float _ignoreGroundedCurrentTime = 0.1f;
    
    // Debug Events
    public static event Action<bool> IsGroundedEvent;
    public static event Action<bool> IsOnSlopeEvent;
    public static event Action<bool> IsJumpingEvent;
    public static event Action<float> GraceTimerEvent;
    public static event Action<float> DashTimerEvent;

    // Components
    private Rigidbody _rigidbody;
    private PlayerInput _inputManager;
    private CapsuleCollider _collider;

    private void Awake()
    {
        _inputManager = GetComponent<PlayerInput>();
        _collider = GetComponent<CapsuleCollider>();
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.freezeRotation = true;

    }
    
    private void Update()
    {
        AdjustGravity();
        GetMovementInput();
        CheckGrounded();
        ControlDrag();
        ControlSpeed();

        if (Input.GetKeyDown(_inputManager.JumpKey) && (_isGrounded || _graceTimeCurrent < graceTimeMax))
        {
            Jump();
        }

        if (Input.GetKeyDown(_inputManager.DashKey) && _moveDirection != Vector3.zero)
        {
            Dash();
        }

        if (!_isGrounded && VaultableInFront())
        {
            
        }
        
        _slopeMoveDirection = Vector3.ProjectOnPlane(_moveDirection, _slopeHit.normal);

        IsGroundedEvent?.Invoke(_isGrounded);
        IsOnSlopeEvent?.Invoke(OnSlope());
        IsJumpingEvent?.Invoke(_isJumping);
        GraceTimerEvent?.Invoke(_graceTimeCurrent);
        DashTimerEvent?.Invoke(_dashTimeCurrent);
    }

    private void FixedUpdate()
    {
        if (_isGrounded && !OnSlope())
        {
            _rigidbody.AddForce(_moveDirection.normalized * moveSpeed * MovementMultiplier + _gravity, ForceMode.Acceleration);
        }
        else if (_isGrounded && OnSlope())
        {
            _rigidbody.AddForce(_slopeMoveDirection.normalized * moveSpeed * MovementMultiplier + _gravity, ForceMode.Acceleration);
        }
        else if (!_isGrounded)
        {
            _rigidbody.AddForce(_moveDirection.normalized * moveSpeed * MovementMultiplier * airMultiplier + _gravity, ForceMode.Acceleration);
        }
    }

    private void AdjustGravity()
    {
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
        _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (_isGrounded && _ignoreGroundedCurrentTime >= _ignoreGroundedMaxTime)
        {
            StopCoroutine(StartIgnoreGroundedTimer());
            StopCoroutine(StartGraceTimer());

            _graceTimeCurrent = 0f;
            _isJumping = false;
        }
        else if (!_isGrounded && !_isJumping && _graceTimeCurrent == 0)
        {
            StartCoroutine(StartIgnoreGroundedTimer());
            StartCoroutine(StartGraceTimer());
        }
    }
    
    private void GetMovementInput()
    {
        if (_isDashing)
        {
            return;
        }
        
        _horizontalMovement = Input.GetAxisRaw("Horizontal");
        _verticalMovement = Input.GetAxisRaw("Vertical");

        _moveDirection = orientation.forward * _verticalMovement + orientation.right * _horizontalMovement;
    }

    private void Jump()
    {
        StartCoroutine(StartIgnoreGroundedTimer());
        
        _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z);
        _rigidbody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        
        _graceTimeCurrent = graceTimeMax;
        _isJumping = true;
    }

    private void Dash()
    {
        StartCoroutine(StartDashingTimer());
    }

    private void ControlSpeed()
    {
        if (Input.GetKey(_inputManager.SprintKey) && _isGrounded)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, acceleration * Time.deltaTime);
        }
        else if (_isDashing)
        {
            moveSpeed = dashSpeed;
        }
        else
        {
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, acceleration * Time.deltaTime);
        }
    }

    private void ControlDrag()
    {
        _rigidbody.drag = _isGrounded && !_isDashing ? groundDrag : airDrag;
    }

    private bool OnSlope()
    {
        if (!Physics.Raycast(transform.position, Vector3.down, out _slopeHit, 1.5f))
        {
            return false;
        }

        return _slopeHit.normal != Vector3.up;
    }

    private bool VaultableInFront()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, GetCameraForward(), out hit, minDistanceToVaultable))
        {
            if (!hit.transform.GetComponent<BoxCollider>())
            {
                return false;
            }
            
            // Get forward vector
            float distanceToPoint = (hit.point - transform.position).magnitude + _collider.radius;
            Vector3 forwardDisplacement = transform.position + (GetCameraForward() * distanceToPoint);
            
            // Get upward vector
            float objectHeight = hit.transform.GetComponent<MeshRenderer>().bounds.size.y / 2f; 
            Vector3 topOfCollider = hit.transform.position + new Vector3(0f, objectHeight, 0f);

            Vector3 playerToColliderTop = new Vector3(forwardDisplacement.x, topOfCollider.y + _collider.height / 2f, forwardDisplacement.z);
            float distanceToTop = Vector3.Distance(forwardDisplacement, playerToColliderTop);

            Debug.DrawLine(transform.position, forwardDisplacement, Color.blue);
            Debug.DrawLine(forwardDisplacement, playerToColliderTop, Color.magenta);
            
            if (distanceToTop <= _collider.height)
            {
                _rigidbody.velocity = Vector3.zero;
                transform.position = playerToColliderTop;
            }
        }

        return false;
    }
    
    private IEnumerator StartGraceTimer()
    {
        while (_graceTimeCurrent <= graceTimeMax)
        {
            _graceTimeCurrent += Time.deltaTime;
            yield return null;
        }
    }
    
    private IEnumerator StartDashingTimer()
    {
        Vector3 oldPlayerVelocity = _rigidbody.velocity;
        _rigidbody.velocity = _moveDirection.normalized * dashSpeed * MovementMultiplier;
        
        _dashTimeCurrent = 0f;
        _isDashing = true;
        
        while (_dashTimeCurrent <= dashTimeMax)
        {
            _dashTimeCurrent += Time.deltaTime;
            yield return null;
        }
        
        _isDashing = false;
        _rigidbody.velocity = oldPlayerVelocity / 4f;
    }

    private Vector3 GetCameraForward()
    {
        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();
        return cameraForward;
    }
    
    private IEnumerator StartIgnoreGroundedTimer()
    {
        _ignoreGroundedCurrentTime = 0f;
        while (_ignoreGroundedCurrentTime <= _ignoreGroundedMaxTime)
        {
            _ignoreGroundedCurrentTime += Time.deltaTime;
            yield return null;
        }
    }
}