using System;
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

    // Camera
    public float _lookSpeed = 2.0f;
    public float _lookXLimit = 45.0f;
    private float _rotationX = 0;

    // Movement States
    private bool _isRunning;
    private bool _isJumping;

    // Grounded timers
    private float _ignoreGroundedMaxTime = 0.1f;
    private float _ignoreGroundedCurrentTime = 0.1f;
    private float _ignoreJumpingMaxTime = 0.1f;
    private float _ignoreJumpingMinTime = 0.1f;

    private Rigidbody _rigidBody;
    private CapsuleCollider _collider;
    private float _distanceToFeet;

    // Grace jump
    [SerializeField] private float _graceTimeMax;
    private float _graceTimeCurrent;

    // Debug Events
    public static event Action<bool> IsGroundedEvent;
    public static event Action<bool> IsJumpingEvent;
    public static event Action<float> GraceTimerEvent;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
        _distanceToFeet = _collider.bounds.extents.y;
    }

    void Update()
    {
        IsGroundedEvent?.Invoke(IsGrounded());
        IsJumpingEvent?.Invoke(_isJumping);
        GraceTimerEvent?.Invoke(_graceTimeCurrent);

        _rotationX += -Input.GetAxis("Mouse Y") * _lookSpeed;
        _rotationX = Mathf.Clamp(_rotationX, -_lookXLimit, _lookXLimit);
        Camera.main.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * _lookSpeed, 0);

        _horizontalMovement = Input.GetAxis("Horizontal");
        _verticalMovement = Input.GetAxis("Vertical");

        _moveDirection = (_horizontalMovement * transform.right + _verticalMovement * transform.forward).normalized;

        if (Input.GetKeyDown(KeyCode.Space) && (IsGrounded() || _graceTimeCurrent < _graceTimeMax))
        {
            _isJumping = true;
            StartCoroutine(StartIgnoreGroundedTimer());
            Jump();

            // Don't allow a second jump after the first grace jump
            _graceTimeCurrent = _graceTimeMax;
        }

        if (!IsGrounded() && !_isJumping && _graceTimeCurrent == 0)
        {
            StartCoroutine(StartIgnoreGroundedTimer());
            StartCoroutine(StartGraceTimer());
        }

        // Reset the jumping
        if (IsGrounded())
        {
            if (_isJumping)
            {
                // Reset jump state
                _isJumping = false;
            }
            _graceTimeCurrent = 0f;
            StopAllCoroutines();
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

    private IEnumerator StartGraceTimer()
    {
        while (_graceTimeCurrent <= _graceTimeMax)
        {
            _graceTimeCurrent += Time.deltaTime;
            yield return null;
        }
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

    private bool IsGrounded()
    {
        bool dontIgnoreGrounded = _ignoreGroundedCurrentTime >= _ignoreGroundedMaxTime;
        return Physics.Raycast(transform.position, -Vector3.up, _distanceToFeet + 0.1f) && dontIgnoreGrounded;
    }
}