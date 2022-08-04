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
    }
    
    public override void Patrol()
    {
        
    }
    
    public override void Pursue()
    {
        
    }
    
    public override void Engage()
    {
        
    }
}