using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashingCooldownUI : MonoBehaviour
{
    [Range(0, 1)]
    [SerializeField] private float fadeInMultiplier = 1f; 
    private Image _radialImage;
    private float _alpha;

    private void Awake()
    {
        _radialImage = GetComponent<Image>();
        _alpha = _radialImage.color.a;
    }

    private void Start()
    {
        PlayerController.DashCooldownTimerEvent += UpdateRadialProgress;
        PlayerController.DashTimerEvent += UpdateFadeIn;
    }

    private void UpdateFadeIn(float current, float max)
    {
        _radialImage.fillAmount = 1;
        Color c = _radialImage.color;
        _radialImage.color = new Color(c.r, c.g, c.b, Mathf.Clamp01(current / (max * fadeInMultiplier)) * _alpha);
    }
    
    private void UpdateRadialProgress(float current, float max)
    {
        _radialImage.fillAmount = 1 - current / max;
    }
}