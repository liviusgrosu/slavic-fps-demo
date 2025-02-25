using UnityEngine;
using UnityEngine.Rendering;

public class PlayerInput : MonoBehaviour
{
    // Movement input
    public KeyCode JumpKey = KeyCode.Space;
    public KeyCode DashKey = KeyCode.LeftShift;
    
    // Attacking input
    public KeyCode PrimaryMouseButton = KeyCode.Mouse0;
    public KeyCode SecondaryMouseButton = KeyCode.Mouse1;

    public enum KeyPress
    {
        Primary,
        Secondary,
        None
    }

    [Tooltip("How long the player holds a button until its considered a hold")]
    [SerializeField] private float _holdThreshold = 0.5f;
    private float _currentHoldThreshold  = 0f;

    private void Awake()
    {
        _currentHoldThreshold = _holdThreshold;
    }

    private void Update()
    {
        if (PlayerState.IsDead)
        {
            return;
        }

        CheckAttackInput(KeyPress.Primary, PrimaryMouseButton);
        CheckAttackInput(KeyPress.Secondary, SecondaryMouseButton);

        if (Input.GetKeyDown(JumpKey))
        {
            InputQueueSystem.Instance.MovementInputQueue.EnqueueInput("Jump");
        }
        
        if (Input.GetKeyDown(DashKey))
        {
            InputQueueSystem.Instance.MovementInputQueue.EnqueueInput("Dash");
        } 

        return;
    }

    private void CheckAttackInput(KeyPress state, KeyCode key)
    {
        if (Input.GetKeyDown(key))
        {
            _currentHoldThreshold = 0f;
        }

        if (Input.GetKey(key))
        {
            _currentHoldThreshold += Time.deltaTime;
            if (_currentHoldThreshold >= _holdThreshold)
            {
                InputQueueSystem.Instance.AttackInputQueue.EnqueueInput($"{state} Hold");
            }
        }

        if (Input.GetKeyUp(key))
        {
            if (_currentHoldThreshold >= _holdThreshold)
            {
                InputQueueSystem.Instance.AttackInputQueue.EnqueueInput($"{state} Release");
            }
            else
            {
                InputQueueSystem.Instance.AttackInputQueue.EnqueueInput($"{state} Press");
            }
        }
    }
}
