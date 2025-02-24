using UnityEngine;
using UnityEngine.Rendering;

public class PlayerInput : MonoBehaviour
{
    // Movement input
    public KeyCode JumpKey = KeyCode.Space;
    public KeyCode DashKey = KeyCode.LeftShift;
    
    // Attacking input
    public KeyCode LightAttackButton = KeyCode.Mouse0;
    public KeyCode HeavyAttackButton = KeyCode.Mouse3;
    public KeyCode BlockingButton = KeyCode.Mouse1;
    private void Update()
    {
        if (PlayerState.IsDead)
        {
            return;
        }

        if (Input.GetKeyDown(JumpKey))
        {
            InputQueueSystem.Instance.MovementInputQueue.EnqueueInput("Jump");
        }
        
        if (Input.GetKeyDown(DashKey))
        {
            InputQueueSystem.Instance.MovementInputQueue.EnqueueInput("Dash");
        } 
        
        if (Input.GetKeyDown(LightAttackButton) && !PlayerState.IsBlocking)
        {
            InputQueueSystem.Instance.AttackInputQueue.EnqueueInput("Light Attack");
        }
        
        if (Input.GetKeyDown(HeavyAttackButton) && !PlayerState.IsBlocking)
        {
            InputQueueSystem.Instance.AttackInputQueue.EnqueueInput("Heavy Attack");
        }

        if (Input.GetKeyDown(BlockingButton) && !PlayerState.IsAttacking)
        {
            InputQueueSystem.Instance.AttackInputQueue.EnqueueInput("Blocking Hold");
        }

        if (Input.GetKeyUp(BlockingButton) && !PlayerState.IsAttacking)
        {
            InputQueueSystem.Instance.AttackInputQueue.EnqueueInput("Blocking Release");
        }
    }
}
