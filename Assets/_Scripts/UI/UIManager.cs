using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private PlayerHealthUI _healthUI;
    private DashingCooldownUI _dashingCooldownUI;
    [SerializeField] private GameObject _deathScreen;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
            return;
        }

        Instance = this;

        _healthUI = GetComponentInChildren<PlayerHealthUI>();
        _dashingCooldownUI = GetComponentInChildren<DashingCooldownUI>();

        _deathScreen.SetActive(false);
    }

    private void Start()
    {
        PlayerHealth.HpEvent += UpdateHealth;
        PlayerHealth.PlayerDied += ShowDeathScreen;
        Respawner.TriggerRestart += ResetUI;
    }

    public void ShowDeathScreen()
    {
        _healthUI.gameObject.SetActive(false);
        _dashingCooldownUI.gameObject.SetActive(false);
        _deathScreen.SetActive(true);
    }

    private void UpdateHealth(int max, int current)
    {
        _healthUI.UpdateBar(max, current);
    }

    private void ResetUI()
    {
        _healthUI.gameObject.SetActive(true);
        _dashingCooldownUI.gameObject.SetActive(true);
        _deathScreen.SetActive(false);
    }
}
