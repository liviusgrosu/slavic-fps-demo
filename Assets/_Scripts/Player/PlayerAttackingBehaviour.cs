using System;
using System.Net;
using UnityEngine;

public class PlayerAttackingBehaviour : MonoBehaviour
{
    public static PlayerAttackingBehaviour Instance;

    public static event Action<bool> IsAttackingEvent;
    public static event Action<bool> IsBlockingEvent;

    public float BlockTime;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        if (PlayerState.IsVaulting)
        {
            return;
        }

        var nextInput = InputQueueSystem.Instance.AttackInputQueue.GetNextInput();

        if (nextInput == "")
        {
            return;
        }

        if (nextInput == "Primary Press" && !PlayerState.IsAttacking)
        {
            PlayerState.IsAttacking = true;
            IsAttackingEvent?.Invoke(true);


            InputQueueSystem.Instance.AttackInputQueue.DequeueInput();
            if (!PlayerState.IsGrounded)
            {
                // TODO: this needs to be removed because once we create the animation then we can rely on its event to set IsAttacking back to false
                PlayerState.IsAttacking = false;
                IsAttackingEvent?.Invoke(false);
                //PlayerAnimationController.Instance.PlayAerialAttackAnimation();
            }
            else
            {
                PlayerAnimationController.Instance.PlayLightAttackAnimation();
            }
        }

        else if (nextInput == "Primary Hold")
        {
            InputQueueSystem.Instance.AttackInputQueue.DequeueInput();
            Debug.Log(nextInput);
        }

        else if (nextInput == "Primary Release")
        {
            InputQueueSystem.Instance.AttackInputQueue.DequeueInput();
            Debug.Log(nextInput);
        }

        else if (nextInput == "Secondary Hold" && !PlayerState.IsBlocking)
        {
            BlockTime = Time.time;
            PlayerState.IsBlocking = true;
            IsBlockingEvent?.Invoke(true);
            InputQueueSystem.Instance.AttackInputQueue.DequeueInput();
            PlayerAnimationController.Instance.PlayerBlockingHoldAnimation();
        }

        else if (nextInput == "Secondary Release")
        {
            Debug.Log(nextInput);
            PlayerState.IsBlocking = false;
            IsBlockingEvent?.Invoke(false);
            InputQueueSystem.Instance.AttackInputQueue.DequeueInput();
            PlayerAnimationController.Instance.PlayerBlockingReleaseAnimation();
        }

        else
        {
            if (nextInput != "Primary Press")
            {
                InputQueueSystem.Instance.AttackInputQueue.DequeueInput();
            }
        }
    }
    
    public void AttackPieceFinished()
    {
        PlayerState.IsAttacking = false;
        IsAttackingEvent?.Invoke(false);
    }

    // I have to put it here cause the animator doesn't know where EnemySword is
    // TODO: Might add a transient between the two
    public void ToggleSwordCollider(int state)
    {
        PlayerWeapon.Instance.ToggleSwordCollider(state);
    }
}