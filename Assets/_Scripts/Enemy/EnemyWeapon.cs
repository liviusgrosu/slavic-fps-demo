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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (PlayerState.IsBlocking && IsEligableForParry())
            {
                _enemyAttackingBehaviour.BecomeStaggered();
            }
            PlayerHealth.Instance.TakeDamage(_root, _damage);
            _collider.enabled = false;
        }
    }

    public void TurnSwordColliderBackOn()
    {
        _collider.enabled = true;
    }

    private bool IsEligableForParry()
    {
        var timeDifference = Time.time - PlayerAttacking.Instance.BlockTime;
        return timeDifference < _enemyAttackingBehaviour.ParryTime;
    }
}
