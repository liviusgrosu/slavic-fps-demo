using UnityEngine;

public class PlayerBowAnimationController : MonoBehaviour
{
    private Animator _playerArms;
    [SerializeField] private GameObject _arrow;

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
}
