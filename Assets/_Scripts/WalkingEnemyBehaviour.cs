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
        _agent.stoppingDistance = stats.engageRadius;
    }
    
    public override void Patrol()
    {
        // Do nothing for now...
    }
    
    public override void Pursue()
    {
        // Follow the player
        _agent.isStopped = false;
        _agent.destination = LevelInitialization.PlayerInstance.transform.position;
    }
    
    public override void Engage()
    {
        _agent.isStopped = true;
        // TODO: Start attacking
    }
}