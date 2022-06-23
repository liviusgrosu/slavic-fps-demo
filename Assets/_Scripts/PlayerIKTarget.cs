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

    [SerializeReference] private Transform targetMiddle;
    [SerializeReference] private Transform leftTarget, rightTarget;

    public void SetMiddleTarget(Vector3 target)
    {
        targetMiddle.position = target;
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
