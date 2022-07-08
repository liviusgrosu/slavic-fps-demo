using System;
using TMPro;
using UnityEngine;
public class PlayerHandsBobbing : MonoBehaviour
{
    [SerializeField] private Transform playerArms;
    [SerializeField] private Transform anchorPoint;
    private Rigidbody _rigidbody;
    private PlayerController _playerController;

    public float bobbingSpeed = 10f;
    public float bobbingAmount = 0.05f;

    private Vector3 _defaultPos;
    private float _defaultPosY = 0;
    private float _timer = 0;

    private Vector3 _offset;
    [SerializeField] private float offsetAmount = 0.1f;
    [SerializeField] private float offsetTimeMultiplier = 1f;
    
    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _rigidbody = GetComponent<Rigidbody>();
        _defaultPos = playerArms.position - anchorPoint.position;
        _defaultPosY = playerArms.transform.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 offsetDirection = Vector3.zero;
        if (_playerController.moveDirection != Vector3.zero)
        {
            offsetDirection = -(_rigidbody.velocity.normalized);
        }
        
        Vector3 offsetTarget = _defaultPos + anchorPoint.position + (offsetDirection * offsetAmount);
        playerArms.position = Vector3.Lerp(playerArms.position, offsetTarget, Time.deltaTime * offsetTimeMultiplier);
        
        
        bool isPlayerMovingHorizontally = Mathf.Abs(_playerController.moveDirection.x) > 0.1f ||
                                          Mathf.Abs(_playerController.moveDirection.z) > 0.1f;
        
        if(isPlayerMovingHorizontally && _playerController.isGrounded)
        {
            float bobbingPercentage = (_playerController.GetMovemenetSpeedPercent() * bobbingSpeed);
            
            //Player is moving
            _timer += Time.deltaTime * bobbingPercentage;
            playerArms.transform.localPosition = new Vector3(playerArms.transform.localPosition.x, _defaultPosY + Mathf.Sin(_timer) * bobbingAmount, playerArms.transform.localPosition.z);
        }
        else
        {
            //Idle
            _timer = 0;
            playerArms.transform.localPosition = new Vector3(playerArms.transform.localPosition.x, Mathf.Lerp(playerArms.transform.localPosition.y, _defaultPosY, Time.deltaTime * bobbingSpeed), playerArms.transform.localPosition.z);
        }
    }
}