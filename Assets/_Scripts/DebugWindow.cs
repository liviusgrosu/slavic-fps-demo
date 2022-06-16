using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugWindow : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _isGroundedText;
    [SerializeField]
    private TMP_Text _isJumpingText;

    private void Start()
    {
        PlayerController.IsGroundedEvent += UpdateGroundedText;
        PlayerController.IsJumpingEvent += UpdateJumpingText;
    }

    private void UpdateGroundedText(bool state)
    {
        _isGroundedText.text = $"Is Grounded: {DisplayBoolString(state)}";
    }

    private void UpdateJumpingText(bool state)
    {
        _isJumpingText.text = $"Is Jumping: {DisplayBoolString(state)}";
    }

    private string DisplayBoolString(bool state)
    {
        return state ? $"<color=#26D73A>{state}</color>" : $"<color=#FF0000>{state}</color>"; 
    }
}
