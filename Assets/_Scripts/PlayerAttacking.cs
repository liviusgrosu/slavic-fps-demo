using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttacking : MonoBehaviour
{
    [SerializeField] private PlayerAnimationController animatorController;
    [SerializeField] private InputQueueSystem inputQueue;

    private void Update()
    {
        if (PlayerState.IsVaulting)
        {
            return;
        }
        if (inputQueue.AttackInputQueue.GetNextInput() == "Light Attack" && !PlayerState.IsAttacking)
        {
            PlayerState.IsAttacking = true;
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
        else if (inputQueue.AttackInputQueue.GetNextInput() == "Heavy Attack" && !PlayerState.IsAttacking)
        {
            PlayerState.IsAttacking = true;
            
            inputQueue.AttackInputQueue.DequeueInput();
            animatorController.PlayHeavyAttackAnimation();
        }
    }

    public void AttackPieceFinished()
    {
        PlayerState.IsAttacking = false;
    }
}