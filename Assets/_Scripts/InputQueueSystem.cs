using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputQueueSystem : MonoBehaviour
{
    [HideInInspector] public Queue<InputCode> attackInputQueue, movementInputQueue;
    [SerializeField] private float actionLifeTime;
    
    private void Awake()
    {
        attackInputQueue = new Queue<InputCode>();
        movementInputQueue = new Queue<InputCode>();
    }

    public void EnqueueAttackInput(String input)
    {
        InputCode inputCode = new InputCode(input);
        attackInputQueue.Enqueue(inputCode);
    }

    public string GetNextAttackInput()
    {
        if (attackInputQueue.Count != 0)
        {
            if (Time.time - attackInputQueue.Peek().timeOfCreation > actionLifeTime)
            {
                DequeueAttackInput();
            }
            else
            {
                return attackInputQueue.Peek().inputName;
            }
        }
        return "";
    }

    public void DequeueAttackInput()
    {
        attackInputQueue.Dequeue();
    }

    // private void Update()
    // {
    //     if (attackInputQueue.Count != 0)
    //     {
    //         //  Debug.Log(attackInputQueue.Count);
    //         Debug.Log(Time.deltaTime - attackInputQueue.Peek().timeOfCreation);
    //         if (Time.deltaTime - attackInputQueue.Peek().timeOfCreation > actionLifeTime)
    //         {
    //             Debug.Log($"Dequeing: {attackInputQueue.Peek().inputName}");
    //             DequeueAttackInput();
    //         }
    //     }
    // }
}

public class InputCode
{
    public String inputName;
    public float timeOfCreation;

    public InputCode(String inputName)
    {
        this.inputName = inputName;
        this.timeOfCreation = Time.time;
    }
}