using System;
using UnityEngine;

public class PlayerBowBehaviour : MonoBehaviour, IPlayerWeaponBehaviour
{
    private PlayerBowAnimationController _animationController;
    private bool _readyToFire = true;

    private ArrowSpawner _arrowSpawner;
    public float BlockTime { get; private set; }

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
        SoundManager.Instance.PlaySoundFXClip("Bow Release", _arrowSpawner.transform);
        _arrowSpawner.SpawnArrow();
        SoundManager.Instance.PlaySoundFXClip("Arrow Loose", _arrowSpawner.transform);
    }

    public bool IsIdling()
    {
        return _animationController.IsIdling();
    }

    public void OnDisable()
    {
        // Because the switch weapon can cause this to be stuck on true we have to reset
        PlayerState.IsAttacking = false;
    }
}
