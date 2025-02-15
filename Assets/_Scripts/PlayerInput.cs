using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private InputQueueSystem inputQueue;
    [SerializeField] private PlayerController controller;
    
    // Movement input
    public KeyCode JumpKey = KeyCode.Space;
    public KeyCode DashKey = KeyCode.LeftShift;
    
    // Attacking input
    public KeyCode LightAttackButton = KeyCode.Mouse0;
    public KeyCode RightAttackButton = KeyCode.Mouse1;
    private void Update()
    {
        if (Input.GetKeyDown(JumpKey))
        {
            inputQueue.MovementInputQueue.EnqueueInput("Jump");
        }
        
        if (Input.GetKeyDown(DashKey))
        {
            inputQueue.MovementInputQueue.EnqueueInput("Dash");
        } 
        
        if (Input.GetKeyDown(LightAttackButton))
        {
            inputQueue.AttackInputQueue.EnqueueInput("Light Attack");
        }
        
        if (Input.GetKeyDown(RightAttackButton))
        {
            inputQueue.AttackInputQueue.EnqueueInput("Heavy Attack");
        }
    }
}
