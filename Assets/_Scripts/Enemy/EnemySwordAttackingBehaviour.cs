using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySwordAttackingBehaviour : MonoBehaviour
{
    // TODO: Might remove
    [SerializeField] private string _idleStateName;
    [Tooltip("The tag used to check if the state is an attack state. Look at the animator state to get an idea")]
    [SerializeField]private string _attackStateTag;
    [Tooltip("Delay between each attack chain")]
    [SerializeField] private float _delayAttackTime = 1f;
    [Tooltip("Time between player blocking and enemy hitting enemy to be considered a parry")]
    public float ParryTime = 0.25f;

    private Animator _animator;
    private EnemySwordWeapon _enemySword;

    private bool _readyToAttack;
    private bool _attackCooldownFinished = true;
    private Queue<string> _currentAttackQueue;

    private void Awake()
    {
        _enemySword = GetComponentInChildren<EnemySwordWeapon>();
        _animator = GetComponent<Animator>();
        _currentAttackQueue = new Queue<string>();

    }

    private void Update()
    {
        // Check the top and ensure that ready to attack is set to true
        if (_currentAttackQueue.Count != 0 &&
            _currentAttackQueue.Peek() == "Light Attack"  &&
            _readyToAttack)
        {
            _currentAttackQueue.Dequeue();
            _readyToAttack = false;
            _animator.SetTrigger("Light Attack");
        }
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

        var chainLength = Random.Range(1, 4);
        Enumerable.Repeat("Light Attack", chainLength)
                    .ToList()
                    .ForEach(_currentAttackQueue.Enqueue);
        ReadyToAttackAgain();
    }

    public void ReadyToAttackAgain()
    {
        _readyToAttack = true;
    }

    public void BecomeStaggered()
    {
        _currentAttackQueue.Clear();
        _animator.SetTrigger("Stagger");
    }

    public bool IsIdiling()
    {
        return _animator.GetCurrentAnimatorStateInfo(0).IsName(_idleStateName);
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

    // I have to put it here cause the animator doesn't know where EnemySword is
    // TODO: Might add a transient between the two
    public void ToggleSwordCollider(int state)
    {
        _enemySword.ToggleSwordCollider(state);
    }
}
