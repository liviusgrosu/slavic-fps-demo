using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyBowAttackingBehaviour : MonoBehaviour
{
    [Tooltip("The tag used to check if the state is an attack state. Look at the animator state to get an idea")]
    [SerializeField] private string _attackStateTag;

    private Animator _animator;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public bool IsAttacking()
    {
        return _animator.GetCurrentAnimatorStateInfo(0).IsTag(_attackStateTag);
    }
}
