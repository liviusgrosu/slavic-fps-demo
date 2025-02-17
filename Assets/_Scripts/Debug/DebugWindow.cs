using TMPro;
using UnityEngine;

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
    private TMP_Text _isVaultingText;
    [SerializeField]
    private TMP_Text _hasDashed;
    [SerializeField]
    private TMP_Text _graceWindowText;
    [SerializeField]
    private TMP_Text _rigidbodySpeedText;
    [SerializeField]
    private TMP_Text _vaultingTimeText;
    [SerializeField]
    private TMP_Text _isAttackingText;
    [SerializeField]
    private TMP_Text _isBlockingText;
    [SerializeField]
    private TMP_Text _hpText;

    private void Start()
    {
        PlayerController.IsGroundedEvent += UpdateGroundedText;
        PlayerController.IsOnSlopeEvent += UpdateSlopeText;
        PlayerController.IsJumpingEvent += UpdateJumpingText;
        PlayerController.GraceTimerEvent += UpdateGraceWindowText;
        PlayerController.RigidbodySpeedEvents += UpdatePlayerSpeed;
        PlayerController.IsVaultingEvent += UpdateVaultingText;
        PlayerController.VaultTimeEvent += UpdateVaultTimeText;
        PlayerAttacking.IsAttackingEvent += UpdateIsAttackingText;
        PlayerAttacking.IsBlockingEvent += UpdateIsBlockingText;
        PlayerHealth.HpEvent += UpdateHpText;
    }

    private void UpdateGroundedText(bool state)
    {
        _isGroundedText.text = $"Grounded: {DisplayBoolString(state)}";
    }

    private void UpdateJumpingText(bool state)
    {
        _isJumpingText.text = $"Jumping: {DisplayBoolString(state)}";
    }

    private void UpdateSlopeText(bool state)
    {
        _isOnSlope.text = $"On Slope: {DisplayBoolString(state)}";
    }
    private void UpdateVaultingText(bool state)
    {
        _isVaultingText.text = $"Vaulting: {DisplayBoolString(state)}";
    }

    private void UpdateVaultTimeText(float current, float max)
    {
        _vaultingTimeText.text = $"Vault curr: {current:F2}, max: {max:F2}";
    }

    private void UpdateRunningText(bool state)
    {
        _isRunningText.text = $"Running: {DisplayBoolString(state)}";
    }

    private void UpdateGraceWindowText(float time)
    {
        _graceWindowText.text = $"Grace Timer: {time:n2}";
    }
    private void UpdatePlayerSpeed(Vector3 speed)
    {
        _rigidbodySpeedText.text = $"Speed X: <color=#ff0000>{speed.x:F1}</color>, " +
                                    $"Y: <color=#26D73A>{speed.y:F1}</color>, " +
                                    $"Z: <color=#0004ff>{speed.z:F1}</color>";
    }

    private string DisplayBoolString(bool state)
    {
        return state ? $"<color=#26D73A>{state}</color>" : $"<color=#FF0000>{state}</color>"; 
    }

    private void UpdateIsAttackingText(bool state)
    {
        _isAttackingText.text = $"Is Attacking: {DisplayBoolString(state)}";
    }

    private void UpdateIsBlockingText(bool state)
    {
        _isBlockingText.text = $"Is Blocking: {DisplayBoolString(state)}";
    }

    private void UpdateHpText(int hp)
    {
        _hpText.text = $"HP: {hp}";
    }
}
