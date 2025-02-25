using System;
using UnityEngine;

public class PlayerBowBehaviour : MonoBehaviour
{
    private PlayerBowAnimationController _animationController;
    private bool _readyToFire = true;

    private ArrowSpawner _arrowSpawner;

    private void Awake()
    {
        _animationController = GetComponent<PlayerBowAnimationController>();
        _arrowSpawner = GetComponentInChildren<ArrowSpawner>();
    }

    void Update()
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

        if (nextInput.Contains("Primary") && _readyToFire)
        {
            _readyToFire = false;
            InputQueueSystem.Instance.AttackInputQueue.DequeueInput();
            _animationController.PlayFireAnimation();
        }

        else
        {
            if (nextInput != "Primary Press")
            {
                InputQueueSystem.Instance.AttackInputQueue.DequeueInput();
            }
        }
    }

    public void ResetToFire()
    {
        _readyToFire = true;
    }

    public void SpawnArrow()
    {
        _arrowSpawner.SpawnArrow();
    }
}
