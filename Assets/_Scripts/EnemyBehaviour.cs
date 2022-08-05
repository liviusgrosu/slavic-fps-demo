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

    // Debug
    public Material PatrolMaterial, PursueMaterial, EngageMaterial;
    private MeshRenderer mesh;
    
    protected void Start()
    {
        _currentState = State.Patrolling;
        mesh = GetComponent<MeshRenderer>();
    }
    
    private void Update()
    {
        // Change states

        if (LevelInitialization.PlayerInstance)
        {
            Vector3 targetDistance = (LevelInitialization.PlayerInstance.transform.position - transform.position);
            
            if (targetDistance.magnitude <= stats.pursueRadius && targetDistance.magnitude > stats.engageRadius)
            {
                _currentState = State.Pursuing;
            }
            else if (targetDistance.magnitude <= stats.engageRadius)
            {
                _currentState = State.Engaging;
            }
        }

        // Change states here
        switch (_currentState)
        {
            case State.Patrolling:
                mesh.sharedMaterial = PatrolMaterial;
                Patrol();
                break;
            case State.Pursuing:
                mesh.sharedMaterial = PursueMaterial;
                Pursue();
                break;
            case State.Engaging:
                mesh.sharedMaterial = EngageMaterial;
                Engage();
                break;
            default:
                break;
        }
    }
}
