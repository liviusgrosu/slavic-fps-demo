using UnityEngine;
public class PlayerHandsBobbing : MonoBehaviour
{
    [SerializeField] private Transform anchorPoint;
    private Rigidbody _rigidbody;
    private Transform _playerArms => PlayerWeaponManager.Instance.GetArms();
    private PlayerController _playerController;

    public float bobbingSpeed = 10f;
    public float bobbingAmount = 0.05f;

    private Vector3 _defaultPos;
    private float _defaultPosY = 0;
    private float _timer = 0;

    [SerializeField] private float defaultOffsetAmount = 0.1f;
    [SerializeField] private float dashOffsetAmount = 0.1f;
    [SerializeField] private float offsetTimeMultiplier = 1f;
    private float _currentOffsetAmount;
    private bool _resetWalkTrigger = true;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _rigidbody = GetComponent<Rigidbody>();
        _currentOffsetAmount = defaultOffsetAmount;

        PlayerController.DashTimerEvent += ModifyOffsetAmount;
    }

    private void Start()
    {
        _defaultPos = _playerArms.position - anchorPoint.position;
        _defaultPosY = _playerArms.transform.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        bool isPlayerMovingHorizontally = Mathf.Abs(_playerController.moveDirection.x) > 0.1f ||
                                          Mathf.Abs(_playerController.moveDirection.z) > 0.1f;
        
        if(isPlayerMovingHorizontally && PlayerState.IsGrounded)
        {
            //Player is moving
            _timer += Time.deltaTime * bobbingSpeed;
            _playerArms.transform.localPosition = new Vector3(_playerArms.transform.localPosition.x, _defaultPosY - Mathf.Sin(_timer) * bobbingAmount, _playerArms.transform.localPosition.z);
            if (!PlayerState.IsDashing)
            {
                if (Mathf.Sin(_timer) > 0f && _resetWalkTrigger)
                {
                    PlayerWalkingSound.Instance.TriggerWalkSound();
                    _resetWalkTrigger = false;
                }
                if (Mathf.Sin(_timer) < 0f && !_resetWalkTrigger)
                {
                    _resetWalkTrigger = true;
                }
            }
        }
        else
        {
            //Idle
            _resetWalkTrigger = true;
            _timer = 0;
            _playerArms.transform.localPosition = new Vector3(_playerArms.transform.localPosition.x, Mathf.Lerp(_playerArms.transform.localPosition.y, _defaultPosY, Time.deltaTime * bobbingSpeed), _playerArms.transform.localPosition.z);
        }
        
        Vector3 offsetDirection = Vector3.zero;
        if (isPlayerMovingHorizontally)
        {
            offsetDirection = -(_rigidbody.linearVelocity.normalized);
        }
        
        Vector3 offsetTarget = _defaultPos + anchorPoint.position + (offsetDirection * _currentOffsetAmount);
        _playerArms.position = Vector3.Lerp(_playerArms.position, offsetTarget, Time.deltaTime * offsetTimeMultiplier);
    }

    private void ModifyOffsetAmount(float current, float max)
    {
        _currentOffsetAmount = Mathf.Lerp(defaultOffsetAmount, dashOffsetAmount, Mathf.PingPong(current * 2,max) / max);
    }
}