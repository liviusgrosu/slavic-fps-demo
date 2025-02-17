using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public int Health = 40;

    public void TakeDamage(int value)
    {
        Health -= value;
        if (Health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
