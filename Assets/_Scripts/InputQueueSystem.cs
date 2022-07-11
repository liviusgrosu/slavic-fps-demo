using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputQueueSystem : MonoBehaviour
{
    private Queue<InputCode> _attackInputQueue, _movementInputQueue;
    [SerializeField] private float actionLifeTime;
    
    private void Awake()
    {
        _attackInputQueue = new Queue<InputCode>();
        _movementInputQueue = new Queue<InputCode>();
    }

    public void EnqueueAttackInput(string input)
    {
        InputCode inputCode = new InputCode(input);
        _attackInputQueue.Enqueue(inputCode);
    }

    public string GetNextAttackInput()
    {
        if (_attackInputQueue.Count == 0)
        {
            return "";
        }

        if (Time.time - _attackInputQueue.Peek().timeOfCreation > actionLifeTime)
        {
            DequeueAttackInput();
            return "";
        }

        return _attackInputQueue.Peek().inputName;
    }

    public void DequeueAttackInput()
    {
        _attackInputQueue.Dequeue();
    }

    public void EnqueueMovementInput(string input)
    {
        InputCode inputCode = new InputCode(input);
        _movementInputQueue.Enqueue(inputCode);
    }
    
    public string GetNextMovementInput()
    {
        if (_movementInputQueue.Count == 0)
        {
            return "";
        }

        if (Time.time - _movementInputQueue.Peek().timeOfCreation > actionLifeTime)
        {
            DequeueMovementInput();
            return "";
        }

        return _movementInputQueue.Peek().inputName;
    }
    
    public void DequeueMovementInput()
    {
        _movementInputQueue.Dequeue();
    }
}

public class InputCode
{
    public string inputName;
    public float timeOfCreation;

    public InputCode(string inputName)
    {
        this.inputName = inputName;
        timeOfCreation = Time.time;
    }
}