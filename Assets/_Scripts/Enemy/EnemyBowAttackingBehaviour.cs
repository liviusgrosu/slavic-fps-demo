using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyBowAttackingBehaviour : MonoBehaviour
{
    [Tooltip("The tag used to check if the state is an attack state. Look at the animator state to get an idea")]
    [SerializeField] private string _attackStateTag;
    [Tooltip("Delay between each attack chain")]
    [SerializeField] private float _delayAttackTime = 1f;
    [Tooltip("The arrow within the model that will turn off")]
    [SerializeField] private GameObject _arrow;

    private bool _attackCooldownFinished = true;
    private ArrowSpawner _arrowSpawner;

    private Animator _animator;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _arrowSpawner = GetComponentInChildren<ArrowSpawner>();
    }

    public void Attack()
    {
        if (IsAttacking() || !_attackCooldownFinished)
        {
            return;
        }

        if (_attackCooldownFinished)
        {
            _attackCooldownFinished = false;
            StartCoroutine(ResetAttackCooldown(_delayAttackTime));
        }

        _animator.SetTrigger("Fire");
    }

    public bool IsAttacking()
    {
        return _animator.GetCurrentAnimatorStateInfo(0).IsTag(_attackStateTag);
    }

    private IEnumerator ResetAttackCooldown(float delay)
    {
        yield return new WaitForSeconds(delay);
        _attackCooldownFinished = true;
    }
    public void SpawnArrow()
    {
        _arrowSpawner.SpawnArrow();
    }

    public void ResetToFire()
    {
    }

    public void ToggleArrowRenderer(int state)
    {
        //_arrow.SetActive(state == 1);
    }
}
