using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class WalkingEnemyBehaviour : EnemyBehaviour
{
    private NavMeshAgent _agent;
    
    private new void Start()
    {
        base.Start();
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = stats.movementSpeed;
    }
    
    public override void Patrol()
    {
        
    }
    
    public override void Pursue()
    {
        // Follow the player
        _agent.SetDestination(LevelInitialization.PlayerInstance.transform.position);
    }
    
    public override void Engage()
    {
        
    }
}