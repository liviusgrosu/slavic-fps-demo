using UnityEngine;
public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private Animator _playerArms;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            _playerArms.SetTrigger("Vault");
        }
    }
}