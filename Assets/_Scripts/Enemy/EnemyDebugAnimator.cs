using UnityEngine;

public class EnemyDebugAnimator : MonoBehaviour
{
    private Animator _animator;
    private EnemySword _enemySword;

    private void Awake()
    {
        _enemySword = GetComponentInChildren<EnemySword>();
        _animator = GetComponent<Animator>();
        InvokeRepeating(nameof(Attack), 1f, 1f);
    }

    private void Attack()
    {
        _animator.SetTrigger("Light Attack");
    }

    public void ResetAttack()
    {
        _enemySword.ResetAttack();
    }
}
