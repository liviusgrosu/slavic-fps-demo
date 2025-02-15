using UnityEngine;

public class PlayerAttacking : MonoBehaviour
{
    private void Update()
    {
        if (PlayerState.IsVaulting)
        {
            return;
        }
        
        if (InputQueueSystem.Instance.AttackInputQueue.GetNextInput() == "Light Attack" && !PlayerState.IsAttacking)
        {
            PlayerState.IsAttacking = true;
            
            InputQueueSystem.Instance.AttackInputQueue.DequeueInput();
            if (!PlayerState.IsGrounded)
            {
                PlayerAnimationController.Instance.PlayAerialAttackAnimation();
            }
            else
            {
                PlayerAnimationController.Instance.PlayLightAttackAnimation();
            }
        }
        else if (InputQueueSystem.Instance.AttackInputQueue.GetNextInput() == "Heavy Attack" && !PlayerState.IsAttacking)
        {
            PlayerState.IsAttacking = true;
            
            InputQueueSystem.Instance.AttackInputQueue.DequeueInput();
            PlayerAnimationController.Instance.PlayHeavyAttackAnimation();
        }
    }

    public void AttackPieceFinished()
    {
        PlayerState.IsAttacking = false;
    }
}