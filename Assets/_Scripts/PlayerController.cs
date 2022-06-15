using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // General Movement
    private float _horizontalMovement;
    private float _verticalMovement;
    private Vector3 _moveDirection;

    // Movement Speeds
    [SerializeField]
    private float _walkSpeed;
    [SerializeField]
    private float _runSpeed;
    [SerializeField]
    private float _jumpSpeed;
    [SerializeField] 
    private float _jumpAcceleration;
    [SerializeField] 
    private float _fallRate;

    // Movement States
    private bool _isRunning;
    private bool _isJumping;

    // Jumping containers
    private float _startJumpTime;
    private float _maxJumpTime;
    private float _airJumpTime = 1f;


    private Rigidbody _rigidBody;
    private CapsuleCollider _collider;
    private float _distanceToFeet;
    private float _colliderRadius = 0.1f;
    private float _maxDistance = 1.0f;


    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
        _distanceToFeet = _collider.height / 2f;
    }

    void Update()
    {
        IsGrounded();

        _horizontalMovement = Input.GetAxis("Horizontal");
        _verticalMovement = Input.GetAxis("Vertical");

        _moveDirection = (_horizontalMovement * transform.right + _verticalMovement * transform.forward).normalized;

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            _isJumping = true;
            _startJumpTime = Time.time;
            _maxJumpTime = _startJumpTime + _airJumpTime;

            Jump();
        }
        else if(Input.GetKey(KeyCode.Space) && !IsGrounded() && (_startJumpTime + _maxJumpTime > Time.time))
        {
            HoldJump();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _isRunning = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _isRunning = false;
        }
    }

    private void FixedUpdate()
    {
        float movementSpeed = _isRunning ? _runSpeed : _walkSpeed;

        Vector3 YAxisGravity = new Vector3(0, _rigidBody.velocity.y - _fallRate, 0);

        _rigidBody.velocity = (_moveDirection * movementSpeed * Time.fixedDeltaTime * 100f) + YAxisGravity;
    }

    private void Jump()
    {
        _rigidBody.AddForce(Vector3.up * _jumpSpeed, ForceMode.Impulse);
    }

    private void HoldJump()
    {
        _rigidBody.AddForce(Vector3.up * _jumpAcceleration, ForceMode.Acceleration);
    }

    private bool IsGrounded()
    {
        RaycastHit hitInfo;
        if (Physics.SphereCast(transform.position, _colliderRadius, Vector3.down, out hitInfo, _distanceToFeet))
        {
            // Reset jump state
            _isJumping = false;
            return true;
        }

        return false;
    }
}