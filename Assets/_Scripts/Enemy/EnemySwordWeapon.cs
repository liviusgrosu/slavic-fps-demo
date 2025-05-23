﻿using System;
using UnityEngine;

public class EnemySwordWeapon : MonoBehaviour
{
    [SerializeField] private int _damage = 20;

    private KnightEnemyBehaviour _enemyBehaviour;
    private EnemySwordAttackingBehaviour _enemyAttackingBehaviour;
    private Collider _collider;
    private Transform _root;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    private void Start()
    {
        _enemyBehaviour = GetComponentInParent<KnightEnemyBehaviour>();
        _enemyAttackingBehaviour = GetComponentInParent<EnemySwordAttackingBehaviour>();
        _root = _enemyBehaviour.transform;
        ToggleSwordCollider(0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (PlayerState.IsBlocking)
            {
                if (IsEligibleForParry())
                {
                    _enemyAttackingBehaviour.BecomeStaggered();
                    SoundManager.Instance.PlaySoundFXClip("Sword Parry", transform);
                }
                else
                {
                    SoundManager.Instance.PlaySoundFXClip("Sword Block", transform);
                }
            }
            PlayerHealth.Instance.TakeDamage(_root, _damage);
            ToggleSwordCollider(0);
        }
    }

    // Animation even can't do bool so we're stuck with ints
    // 0 = false
    // 1 = true
    public void ToggleSwordCollider(int state)
    {
        _collider.enabled = state == 1;
    }

    private bool IsEligibleForParry()
    {
        var timeDifference = Time.time - PlayerWeaponManager.Instance.CurrentWeaponBehaviour.BlockTime;
        return timeDifference < _enemyAttackingBehaviour.ParryTime;
    }
}
