using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private InputQueueSystem inputQueue;
    
    // Movement input
    public KeyCode JumpKey = KeyCode.Space;
    public KeyCode SprintKey = KeyCode.LeftShift;
    public KeyCode DashKey = KeyCode.LeftAlt;
    
    // Attacking input
    public KeyCode LightAttackButton = KeyCode.Mouse0;

    private void Update()
    {
        if (Input.GetKeyDown(JumpKey))
        {
            inputQueue.EnqueueMovementInput("Jump");
        }
        
        if (Input.GetKeyDown(DashKey))
        {
            inputQueue.EnqueueMovementInput("Dash");
        } 
        
        if (Input.GetKeyDown(LightAttackButton))
        {
            inputQueue.EnqueueAttackInput("Light Attack");
        }
    }
}
