using UnityEngine;

public class UIManager : MonoBehaviour
{
    private PlayerHealthUI healthUI;

    private void Awake()
    {
        PlayerHealth.HpEvent += UpdateHealth;
        healthUI = GetComponentInChildren<PlayerHealthUI>();
    }

    private void UpdateHealth(int max, int current)
    {
        healthUI.UpdateBar(max, current);
    }
}
