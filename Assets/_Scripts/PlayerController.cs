using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float _walkSpeed;
    [SerializeField]
    private float _runSpeed;

    private bool _isRunning;

    private float _horizontalMovement;
    private float _verticalMovement;
    private Vector3 _moveDirection;
    private Rigidbody _rigidBody;


    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        _horizontalMovement = Input.GetAxis("Horizontal");
        _verticalMovement = Input.GetAxis("Vertical");

        Debug.Log($"{_horizontalMovement}, {_verticalMovement}");

        _moveDirection = (_horizontalMovement * transform.right + _verticalMovement * transform.forward).normalized;

        

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
        _rigidBody.velocity = _moveDirection * movementSpeed * Time.fixedDeltaTime * 100f;
    }

}