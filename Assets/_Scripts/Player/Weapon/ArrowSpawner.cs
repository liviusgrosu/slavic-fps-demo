using System;
using UnityEngine;

public class ArrowSpawner : MonoBehaviour
{
    public GameObject _arrowPrefab;

    public void SpawnArrow()
    {
        Vector3 directionToPlayer = transform.position - PlayerController.Instance.transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer.normalized);
        Instantiate(_arrowPrefab, transform.position, lookRotation);
    }
}
