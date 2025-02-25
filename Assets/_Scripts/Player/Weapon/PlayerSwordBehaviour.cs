using System;
using System.Net;
using UnityEngine;

public class PlayerSwordBehaviour : MonoBehaviour, IPlayerWeaponBehaviour
{
    // TODO: Remove this singleton
    public static PlayerSwordBehaviour Instance;
    private PlayerSwordAnimationController _animationController;

    public static event Action<bool> IsAttackingEvent;
    public static event Action<bool> IsBlockingEvent;

    public float BlockTime { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
            return;
        }
        Instance = this;

        _animationController = GetComponent<PlayerSwordAnimationController>();
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
            InputQueueSystem.Instance.AttackInputQueue.DequeueInput();
            PlayerState.IsAttacking = true;
            IsAttackingEvent?.Invoke(true);
            _animationController.PlayLightAttackAnimation();
        }

        else if (nextInput == "Primary Hold")
        {
            InputQueueSystem.Instance.AttackInputQueue.DequeueInput();
            // TODO: Play aerial attack here
        }

        else if (nextInput == "Primary Release")
        {
            InputQueueSystem.Instance.AttackInputQueue.DequeueInput();
        }

        else if (nextInput == "Secondary Hold" && !PlayerState.IsBlocking)
        {
            InputQueueSystem.Instance.AttackInputQueue.DequeueInput();
            
            PlayerState.IsBlocking = true;
            BlockTime = Time.time;
            IsBlockingEvent?.Invoke(true);
            _animationController.PlayerBlockingHoldAnimation();
        }

        else if (nextInput == "Secondary Release")
        {
            InputQueueSystem.Instance.AttackInputQueue.DequeueInput();
            
            PlayerState.IsBlocking = false;
            IsBlockingEvent?.Invoke(false);
            _animationController.PlayerBlockingReleaseAnimation();
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