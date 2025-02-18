using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySword : MonoBehaviour
{
    [SerializeField] private int _damage = 20;

    private EnemyBehaviour _enemyBehaviour;
    private Collider _collider;
    private Transform _root;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    private void Start()
    {
        _enemyBehaviour = GetComponentInParent<EnemyBehaviour>();
        _root = _enemyBehaviour.transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth.Instance.TakeDamage(_root, _damage);
            _collider.enabled = false;
        }
    }

    public void ResetAttack()
    {
        _collider.enabled = true;
    }
}
