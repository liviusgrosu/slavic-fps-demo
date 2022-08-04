using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInitialization : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    public static GameObject PlayerInstance;

    private void Start()
    {
        PlayerInstance = GameObject.FindWithTag("Player");

        if (PlayerInstance == null)
        {
            // TODO: Add spawn area
            Instantiate(playerPrefab);
        }
    }
}
