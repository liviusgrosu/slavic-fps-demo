using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySwordAnimator : MonoBehaviour
{
    [SerializeField] private string _idleStateName;
    [SerializeField] private List<string> _attackStateNames;
    private HashSet<int> _attackStateHashes;

    private Animator _animator;
    private EnemySword _enemySword;

    private void Awake()
    {
        _enemySword = GetComponentInChildren<EnemySword>();
        _animator = GetComponent<Animator>();

        _attackStateHashes = new HashSet<int>(_attackStateNames.Select(Animator.StringToHash));

    }


    public void PlayAttack()
    {
        _animator.SetTrigger("Light Attack");
    }

    public void ResetAttack()
    {
        _enemySword.ResetAttack();
    }

    public bool IsIdiling()
    {
        return _animator.GetCurrentAnimatorStateInfo(0).IsName(_idleStateName);
    }
    public bool IsAttacking()
    {
        var currentState = _animator.GetCurrentAnimatorStateInfo(0);
        return _attackStateHashes.Contains(currentState.shortNameHash);
    }
}
