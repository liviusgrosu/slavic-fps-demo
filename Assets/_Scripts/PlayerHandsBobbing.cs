using System;
using UnityEngine;
public class PlayerHandsBobbing : MonoBehaviour
{
    [SerializeField] private Transform _playerArms;
    [SerializeField] private PlayerController playerController;

    public float walkingBobbingSpeed = 14f;
    public float bobbingAmount = 0.05f;

    float defaultPosY = 0;
    float timer = 0;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        defaultPosY = _playerArms.transform.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        if(Mathf.Abs(playerController.moveDirection.x) > 0.1f || Mathf.Abs(playerController.moveDirection.z) > 0.1f)
        {
            //Player is moving
            timer += Time.deltaTime * walkingBobbingSpeed;
            _playerArms.transform.localPosition = new Vector3(_playerArms.transform.localPosition.x, defaultPosY + Mathf.Sin(timer) * bobbingAmount, _playerArms.transform.localPosition.z);
        }
        else
        {
            //Idle
            timer = 0;
            _playerArms.transform.localPosition = new Vector3(_playerArms.transform.localPosition.x, Mathf.Lerp(_playerArms.transform.localPosition.y, defaultPosY, Time.deltaTime * walkingBobbingSpeed), _playerArms.transform.localPosition.z);
        }
    }
}