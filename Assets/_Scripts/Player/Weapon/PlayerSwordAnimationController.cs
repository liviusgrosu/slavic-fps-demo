using System;
using UnityEngine;
public class PlayerSwordAnimationController : MonoBehaviour
{
    [SerializeField] private string _idleStateName;
    private Animator _playerArms;

    private void Awake()
    {
        _playerArms = GetComponent<Animator>();
    }

    public void PlayLightAttackAnimation()
    {
        _playerArms.SetTrigger("Light Attack");
    }
    
    public void PlayHeavyAttackAnimation()
    {
        _playerArms.SetTrigger("Heavy Attack");
    }
    
    public void PlayAerialAttackAnimation()
    {
        _playerArms.SetTrigger("Aerial Attack");
    }

    public void PlayerBlockingHoldAnimation()
    {
        _playerArms.SetTrigger("Blocking Hold");
    }

    public void PlayerBlockingReleaseAnimation()
    {
        _playerArms.SetTrigger("Blocking Release");
    }

    public bool IsIdling()
    {
        return _playerArms.GetCurrentAnimatorStateInfo(0).IsName(_idleStateName);
    }
}