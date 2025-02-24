using System.Collections.Generic;
using UnityEngine;

public class InputCode
{
    public readonly string inputName;
    public readonly float timeOfCreation;

    public InputCode(string inputName)
    {
        this.inputName = inputName;
        timeOfCreation = Time.time;
    }
}

public class InputQueue
{
    private Queue<InputCode> _queue;
    private readonly float _actionLifeTime;

    public InputQueue(float actionLifeTime)
    {
        _queue = new Queue<InputCode>();
        _actionLifeTime = actionLifeTime;
    }

    public void EnqueueInput(string input)
    {
        InputCode inputCode = new InputCode(input);
        _queue.Enqueue(inputCode);
    }

    public void DequeueInput()
    {
        _queue.Dequeue();
    }
    
    public string GetNextInput()
    {
        if (_queue.Count == 0)
        {
            return "";
        }

        if (Time.time - _queue.Peek().timeOfCreation > _actionLifeTime)
        {
            DequeueInput();
            return "";
        }

        return _queue.Peek().inputName;
    }
}

public class InputQueueSystem : MonoBehaviour
{
    [SerializeField] private float attackActionLifeTime = 0.3f;
    [SerializeField] private float movementActionLifeTime = 0.2f;

    [HideInInspector] public InputQueue AttackInputQueue, MovementInputQueue;
    public static InputQueueSystem Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
            return;
        }

        Instance = this;

        AttackInputQueue = new InputQueue(attackActionLifeTime);
        MovementInputQueue = new InputQueue(movementActionLifeTime);
    }
}