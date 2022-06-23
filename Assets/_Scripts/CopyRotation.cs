using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyRotation : MonoBehaviour
{
    [SerializeField] private Transform anchorObject;
    private Vector3 _offset;

    private void Awake()
    {
        _offset = transform.rotation.eulerAngles;
    }

    private void Update()
    {
        transform.rotation = anchorObject.rotation;
        transform.rotation *= Quaternion.Euler(_offset.x, _offset.y, _offset.z);
    }
}
