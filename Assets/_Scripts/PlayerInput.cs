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

    private Command buttonW = new MoveForward();
    private Command buttonA = new DoNothing();
    private Command buttonS = new DoNothing();
    private Command buttonD = new DoNothing();
    
    private Command buttonSpace = new Jump();

    
    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.W))
        // {
        //     buttonW.Execute();
        // }

        if (Input.GetKeyDown(JumpKey))
        {
            inputQueue.movementInputQueue.EnqueueInput("Jump");
        }
        
        if (Input.GetKeyDown(DashKey))
        {
            inputQueue.movementInputQueue.EnqueueInput("Dash");
        } 
        
        if (Input.GetKeyDown(LightAttackButton))
        {
            inputQueue.attackInputQueue.EnqueueInput("Light Attack");
        }
    }
}
