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

    public void SetMiddleTarget(Vector3 target, Vector3 left)
    {
        targetMiddle.position = target;
        leftTarget.position = target + left * targetSpan;
        rightTarget.position = target - left * targetSpan;
    }

    // [SerializeField] private Transform target;
    // [SerializeField] private Transform anchor;
    // [SerializeField] private bool _isDummy;
    // private bool _isVaulting;
    // private Vector3 _targetPosition;
    // private Vector3 _restingOffset;
    //
    // private void Start()
    // {
    //     _restingOffset = target.position - anchor.position;
    // }
    //
    // public void StartVaultingIK(Vector3 wallPoint, Vector3 topOfWall, Vector3 direction)
    // {
    //     Vector3 wallEdgePosition = new Vector3(wallPoint.x, topOfWall.y, wallPoint.z) + direction;
    //     _isVaulting = true;
    //     _targetPosition = wallEdgePosition;
    // }
    //
    // public void StopVaultingIK()
    // {
    //     _isVaulting = false;
    //     _targetPosition = _restingOffset + anchor.position;
    // }
    //
    // private void Update()
    // {
    //     if (_isDummy)
    //     {
    //         return;
    //     }
    //     if (!_isVaulting)
    //     {
    //         _targetPosition = _restingOffset + anchor.position;
    //         target.position = _targetPosition;
    //     }
    //     else
    //     {
    //         target.position = _targetPosition;
    //         // Debug.Break();
    //     }
    //     
    // }
}
