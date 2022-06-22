using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffects : MonoBehaviour
{
    [Header("Dashing")]
    [SerializeField] private Transform dashEffect;
    [SerializeField] private float dashEffectDistance = 1f;
    [SerializeField] private float dashCameraFOV = 100f;
    private float _currentCameraFOV;

    [SerializeField] private Transform mainCamera;

    private void Awake()
    {
        PlayerController.DashEvent += ToggleDashing;
    }

    private void Start()
    {
        _currentCameraFOV = mainCamera.GetComponent<Camera>().fieldOfView;
    }
    
    private void Update()
    {
        dashEffect.position = mainCamera.position + (mainCamera.forward * dashEffectDistance);
        dashEffect.LookAt(mainCamera, transform.up);
    }

    private void ToggleDashing(bool state)
    {
        if (state)
        {
            mainCamera.GetComponent<Camera>().fieldOfView = dashCameraFOV;
            dashEffect.gameObject.SetActive(true);
        }
        else
        {
            mainCamera.GetComponent<Camera>().fieldOfView = _currentCameraFOV;
            dashEffect.gameObject.SetActive(false);
        }
    }
}
