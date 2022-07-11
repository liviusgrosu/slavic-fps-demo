using System.Collections.Generic;
using UnityEngine;

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

public class InputQueue
{
    private Queue<InputCode> queue;
    private float actionLifeTime;

    public InputQueue(float actionLifeTime)
    {
        queue = new Queue<InputCode>();
        this.actionLifeTime = actionLifeTime;
    }

    public void EnqueueInput(string input)
    {
        InputCode inputCode = new InputCode(input);
        queue.Enqueue(inputCode);
    }

    public void DequeueInput()
    {
        queue.Dequeue();
    }
    
    public string GetNextInput()
    {
        if (queue.Count == 0)
        {
            return "";
        }

        if (Time.time - queue.Peek().timeOfCreation > actionLifeTime)
        {
            DequeueInput();
            return "";
        }

        return queue.Peek().inputName;
    }
}

public class InputQueueSystem : MonoBehaviour
{
    [SerializeField] private float attackActionLifeTime = 0.3f;
    [SerializeField] private float movementActionLifeTime = 0.2f;

    [HideInInspector] public InputQueue attackInputQueue, movementInputQueue;
    
    private void Awake()
    {
        attackInputQueue = new InputQueue(attackActionLifeTime);
        movementInputQueue = new InputQueue(movementActionLifeTime);
    }
}