using UnityEngine;

public class PlayerBowAnimationController : MonoBehaviour
{
    [SerializeField] private string _idleStateName;
    [SerializeField] private GameObject _arrow;
    private Animator _playerArms;

    private void Awake()
    {
        _playerArms = GetComponent<Animator>();
    }

    public void PlayFireAnimation()
    {
        _playerArms.SetTrigger("Fire");
    }

    // 1 = true
    // 0 = false
    public void ToggleArrowRenderer(int state)
    {
        _arrow.SetActive(state == 1);
    }

    public bool IsIdling()
    {
        return _playerArms.GetCurrentAnimatorStateInfo(0).IsName(_idleStateName);
    }
}
