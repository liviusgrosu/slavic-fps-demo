using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttacking : MonoBehaviour
{
    [SerializeField] private PlayerAnimationController animatorController;
    [SerializeField] private InputQueueSystem inputQueue;

    private bool _isAttacking;

    private void Update()
    {
        if (inputQueue.AttackInputQueue.GetNextInput() == "Light Attack" && !_isAttacking)
        {
            _isAttacking = true;
            PlayerState.IsAttacking = _isAttacking;
            
            inputQueue.AttackInputQueue.DequeueInput();
            if (!PlayerState.IsGrounded)
            {
                animatorController.PlayAerialAttackAnimation();
            }
            else
            {
                animatorController.PlayLightAttackAnimation();
            }
        }
        else if (inputQueue.AttackInputQueue.GetNextInput() == "Heavy Attack" && !_isAttacking)
        {
            _isAttacking = true;
            PlayerState.IsAttacking = _isAttacking;
            
            inputQueue.AttackInputQueue.DequeueInput();
            animatorController.PlayHeavyAttackAnimation();
        }
    }

    public void AttackPieceFinished()
    {
        _isAttacking = false;
        PlayerState.IsAttacking = _isAttacking;
    }
}