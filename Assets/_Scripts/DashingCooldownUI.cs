using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashingCooldownUI : MonoBehaviour
{
    private Image _radialImage;

    private void Awake()
    {
        _radialImage = GetComponent<Image>();
    }

    private void Start()
    {
        PlayerController.DashCooldownEvent += UpdateRadialProgress;
    }

    private void UpdateRadialProgress(float current, float max)
    {
        _radialImage.fillAmount = 1 - current / max;
    }
}