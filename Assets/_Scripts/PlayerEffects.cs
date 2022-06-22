using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffects : MonoBehaviour
{
    [SerializeField] private Transform mainCamera;
    
    [Header("Dashing")]
    [SerializeField] private Transform dashEffect;
    [SerializeField] private float dashEffectDistance = 1f;

    public void PerformDashEffect(Vector3 direction, float time)
    {
        dashEffect.position = mainCamera.position + (direction * dashEffectDistance);
        dashEffect.LookAt(mainCamera, transform.up);

        var main = dashEffect.GetComponent<ParticleSystem>().main;
        main.duration = time * 0.75f;
        
        dashEffect.GetComponent<ParticleSystem>().Play();
    }
}
