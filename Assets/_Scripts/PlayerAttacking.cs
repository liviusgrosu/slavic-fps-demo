﻿using System;
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
        if (inputQueue.GetNextAttackInput() == "Light Attack" && !_isAttacking)
        {
            _isAttacking = true;
            inputQueue.DequeueAttackInput();
            animatorController.PlayLighAttackAnimation();
        }
    }

    public void LightAttackFinished()
    {
        _isAttacking = false;
    }
}