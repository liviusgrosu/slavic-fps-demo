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
    [SerializeField]
    private TMP_Text _graceWindowText;

    private void Start()
    {
        PlayerController.IsGroundedEvent += UpdateGroundedText;
        PlayerController.IsJumpingEvent += UpdateJumpingText;
        PlayerController.GraceTimerEvent += UpdateGraceWindowText;
    }

    private void UpdateGroundedText(bool state)
    {
        _isGroundedText.text = $"Is Grounded: {DisplayBoolString(state)}";
    }

    private void UpdateJumpingText(bool state)
    {
        _isJumpingText.text = $"Is Jumping: {DisplayBoolString(state)}";
    }

    private void UpdateGraceWindowText(float time)
    {
        _graceWindowText.text = $"Grace Timer: {time:n2}";
    }

    private string DisplayBoolString(bool state)
    {
        return state ? $"<color=#26D73A>{state}</color>" : $"<color=#FF0000>{state}</color>"; 
    }
}
