using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public float jumpForce = 5f;

    [Header("Drag")]
    [SerializeField] private float groundDrag = 6f;
    [SerializeField] private float airDrag = 2f;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundDistance = 0.2f;
    private bool _isGrounded;
    private RaycastHit _slopeHit;
    
    
    // Debug Events
    public static event Action<bool> IsGroundedEvent;
    public static event Action<bool> IsOnSlope;

    // Components
    private Rigidbody _rigidbody;
    private PlayerInput _inputManager;

    private void Awake()
    {
        _inputManager = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.freezeRotation = true;
    }

    private void Update()
    {
        _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        PlayerInput();
        ControlDrag();
        ControlSpeed();

        if (Input.GetKeyDown(_inputManager.JumpKey) && _isGrounded)
        {
            Jump();
        }

        _slopeMoveDirection = Vector3.ProjectOnPlane(_moveDirection, _slopeHit.normal);
        
        IsGroundedEvent?.Invoke(_isGrounded);
        IsOnSlope?.Invoke(OnSlope());
    }

    private void FixedUpdate()
    {
        if (_isGrounded && !OnSlope())
        {
            _rigidbody.AddForce(_moveDirection.normalized * moveSpeed * MovementMultiplier + Physics.gravity, ForceMode.Acceleration);
        }
        else if (_isGrounded && OnSlope())
        {
            _rigidbody.AddForce(_slopeMoveDirection.normalized * moveSpeed * MovementMultiplier + -_slopeHit.normal * Physics.gravity.magnitude, ForceMode.Acceleration);
        }
        else if (!_isGrounded)
        {
            _rigidbody.AddForce(_moveDirection.normalized * moveSpeed * MovementMultiplier * airMultiplier + Physics.gravity, ForceMode.Acceleration);
        }
    }
    
    private void PlayerInput()
    {
        _horizontalMovement = Input.GetAxisRaw("Horizontal");
        _verticalMovement = Input.GetAxisRaw("Vertical");

        _moveDirection = orientation.forward * _verticalMovement + orientation.right * _horizontalMovement;
    }

    private void Jump()
    {
        if (!_isGrounded)
        {
            return;
        }
        
        _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z);
        _rigidbody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ControlSpeed()
    {
        if (Input.GetKey(_inputManager.SprintKey) && _isGrounded)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, acceleration * Time.deltaTime);
        }
    }

    private void ControlDrag()
    {
        _rigidbody.drag = _isGrounded ? groundDrag : airDrag;
    }

    private bool OnSlope()
    {
        if (!Physics.Raycast(transform.position, Vector3.down, out _slopeHit, 1.5f))
        {
            return false;
        }

        return _slopeHit.normal != Vector3.up;
    }
}