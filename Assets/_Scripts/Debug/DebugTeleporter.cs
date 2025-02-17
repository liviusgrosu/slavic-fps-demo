using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTeleporter : MonoBehaviour
{
    [SerializeField] private Transform endPoint;
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.CompareTag("Player"))
        {
            return;
        }

        var playerCentre = collision.transform.GetComponent<CapsuleCollider>().height / 2f;
        collision.transform.position = endPoint.position + new Vector3(0f, playerCentre, 0f);
    }
}
