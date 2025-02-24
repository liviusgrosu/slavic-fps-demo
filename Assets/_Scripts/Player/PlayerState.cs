public static class PlayerState
{
    public static bool IsGrounded;
    public static bool IsAttacking;
    public static bool IsBlocking;
    public static bool IsVaulting;
    public static bool InCombat => IsAttacking || IsBlocking;
    public static bool IsDead => PlayerHealth.Instance.IsDead();
}
