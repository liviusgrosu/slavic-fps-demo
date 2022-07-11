using System;
using UnityEngine;
public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private Animator _playerArms;

    private void Start()
    {
        PlayerController.VaultingEvent += PlayVaultingAnimation;
    }

    private void PlayVaultingAnimation()
    {
        _playerArms.SetTrigger("Vault");
    }

    public void PlayLighAttackAnimation()
    {
        _playerArms.SetTrigger("Light Attack");
    }
}