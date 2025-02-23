using System;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    [SerializeField] private int _damage = 20;


    private EnemyBehaviour _enemyBehaviour;
    private EnemyAttackingBehaviour _enemyAttackingBehaviour;
    private Collider _collider;
    private Transform _root;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    private void Start()
    {
        _enemyBehaviour = GetComponentInParent<EnemyBehaviour>();
        _enemyAttackingBehaviour = GetComponentInParent<EnemyAttackingBehaviour>();
        _root = _enemyBehaviour.transform;
        ToggleSwordCollider(0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (PlayerState.IsBlocking && IsEligibleForParry())
            {
                _enemyAttackingBehaviour.BecomeStaggered();
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
        var timeDifference = Time.time - PlayerAttacking.Instance.BlockTime;
        return timeDifference < _enemyAttackingBehaviour.ParryTime;
    }
}
