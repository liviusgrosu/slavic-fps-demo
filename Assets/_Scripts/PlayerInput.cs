using System.Runtime.InteropServices;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private InputQueueSystem inputQueue;
    [SerializeField] private PlayerController controller;
    
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
    }
}
