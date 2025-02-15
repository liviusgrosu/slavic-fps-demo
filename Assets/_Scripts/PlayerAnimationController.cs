using System;
using UnityEngine;
public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private Animator _playerArms;
    public static PlayerAnimationController Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        PlayerController.VaultingEvent += PlayVaultingAnimation;
    }

    private void PlayVaultingAnimation()
    {
        _playerArms.SetTrigger("Vault");
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
}