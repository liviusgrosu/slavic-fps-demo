using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIKTarget : MonoBehaviour
{
    /* Algo
     * 1. Player controller needs to inform this script the target
     *      - the target 
     */

    [SerializeField] private Transform targetMiddle;
    [SerializeField] private Transform leftTarget, rightTarget;
    [Range(0.1f, 1.5f)]
    [SerializeField] private float targetSpan = 1f;

    [SerializeField] private Transform leftIKTarget;
    [SerializeField] private Transform defaultLeftIKTarget;
    [HideInInspector] public bool lockState;
    
    private void Update()
    {
        if (lockState)
        {
            leftIKTarget.position = leftTarget.position;
        }
        else
        {
            leftIKTarget.position = defaultLeftIKTarget.position;
        }
    }
    
    public void SetMiddleTarget(Vector3 target, Vector3 left)
    {
        targetMiddle.position = target;
        leftTarget.position = target + left * targetSpan;
        rightTarget.position = target - left * targetSpan;
    }
}
