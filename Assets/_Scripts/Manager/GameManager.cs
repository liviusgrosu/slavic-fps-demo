using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private bool _allowRestart;
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
        Respawner.TriggerRestart += RestartGameOver;
        PlayerHealth.PlayerDied += TriggerGameOver;
    }

    private void Update()
    {
        if (_allowRestart && Input.GetKeyDown(KeyCode.R))
        {
            Respawner.Instance.RespawnEverything();
        }
    }

    private void TriggerGameOver()
    {
        _allowRestart = true;
    }

    private void RestartGameOver()
    {
        _allowRestart = false;
    }
}
