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
    private TMP_Text _isRunningText;
    [SerializeField]
    private TMP_Text _isOnSlope;
    [SerializeField]
    private TMP_Text _hasDashed;
    [SerializeField]
    private TMP_Text _graceWindowText;
    [SerializeField]
    private TMP_Text _dashTimeText;

    private void Start()
    {
        PlayerController.IsGroundedEvent += UpdateGroundedText;
        PlayerController.IsOnSlopeEvent += UpdateSlopeText;
        PlayerController.IsJumpingEvent += UpdateJumpingText;
        PlayerController.GraceTimerEvent += UpdateGraceWindowText;
        PlayerController.DashTimerEvent += UpdateDashTimeText;
    }

    private void UpdateGroundedText(bool state)
    {
        _isGroundedText.text = $"Is Grounded: {DisplayBoolString(state)}";
    }

    private void UpdateJumpingText(bool state)
    {
        _isJumpingText.text = $"Is Jumping: {DisplayBoolString(state)}";
    }

    private void UpdateSlopeText(bool state)
    {
        _isOnSlope.text = $"Is On Slope: {DisplayBoolString(state)}";
    }
    
    private void UpdateRunningText(bool state)
    {
        _isRunningText.text = $"Is Running: {DisplayBoolString(state)}";
    }

    private void UpdateDashedText(bool state)
    {
        _hasDashed.text = $"Has Dashed: {DisplayBoolString(state)}";
    }

    private void UpdateGraceWindowText(float time)
    {
        _graceWindowText.text = $"Grace Timer: {time:n2}";
    }
    
    private void UpdateDashTimeText(float time)
    {
        _dashTimeText.text = $"Dash Timer: {time:n2}";
    }

    private string DisplayBoolString(bool state)
    {
        return state ? $"<color=#26D73A>{state}</color>" : $"<color=#FF0000>{state}</color>"; 
    }
}
