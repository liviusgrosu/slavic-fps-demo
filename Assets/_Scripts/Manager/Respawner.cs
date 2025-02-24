using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Respawner : MonoBehaviour
{
    public static Respawner Instance;

    public static event Action TriggerRestart;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
            return;
        }
        Instance = this;
    }

    public void RespawnEverything()
    {
        TriggerRestart?.Invoke();
    }
}
