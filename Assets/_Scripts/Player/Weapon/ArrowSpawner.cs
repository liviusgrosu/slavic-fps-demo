using System;
using UnityEngine;

public class ArrowSpawner : MonoBehaviour
{
    public GameObject _arrowPrefab;

    public void SpawnArrow()
    {
        Instantiate(_arrowPrefab, transform.position, transform.rotation);
    }
}
