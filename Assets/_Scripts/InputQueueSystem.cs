using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputQueueSystem : MonoBehaviour
{
    [HideInInspector] public Queue<InputCode> attackInputQueue, movementInputQueue;

    private void Awake()
    {
        attackInputQueue = new Queue<InputCode>();
        movementInputQueue = new Queue<InputCode>();
    }

    public void EnqueueAttackInput(String input)
    {
        InputCode inputCode = new InputCode(input, 1.0f);
        attackInputQueue.Enqueue(inputCode);
    }

    public string GetNextAttackInput()
    {
        return attackInputQueue.Count != 0 ? attackInputQueue.Peek().inputName : ""; 
    }

    public void DequeueAttackInput()
    {
        attackInputQueue.Dequeue();
    }
}

public class InputCode
{
    public String inputName;
    public float expiration;

    public InputCode(String inputName, float expiration)
    {
        this.inputName = inputName;
        this.expiration = expiration;
    }
}