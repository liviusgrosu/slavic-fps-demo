using UnityEngine;

public class PlayerInput : MonoBehaviour
{
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
            InputQueueSystem.Instance.MovementInputQueue.EnqueueInput("Jump");
        }
        
        if (Input.GetKeyDown(DashKey))
        {
            InputQueueSystem.Instance.MovementInputQueue.EnqueueInput("Dash");
        } 
        
        if (Input.GetKeyDown(LightAttackButton))
        {
            InputQueueSystem.Instance.AttackInputQueue.EnqueueInput("Light Attack");
        }
        
        if (Input.GetKeyDown(RightAttackButton))
        {
            InputQueueSystem.Instance.AttackInputQueue.EnqueueInput("Heavy Attack");
        }
    }
}
