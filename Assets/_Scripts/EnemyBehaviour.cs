using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public EnemyStats stats;
    
    private enum State
    {
        Patrolling,
        Pursuing,
        Engaging
    } 
    
    private State _currentState;
    
    public virtual void Patrol() { }

    public virtual void Pursue() { }

    public virtual void Engage() { }

    protected void Start()
    {
        _currentState = State.Patrolling;
        Debug.Log(gameObject.name);
    }
    
    private void Update()
    {
        // Change states here
        switch (_currentState)
        {
            case State.Patrolling:
                Patrol();
                break;
            case State.Pursuing:
                Pursue();
                break;
            case State.Engaging:
                Engage();
                break;
            default:
                break;
        }
    }
}
