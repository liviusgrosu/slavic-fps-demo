using UnityEngine;

public class PlayerSword : MonoBehaviour
{
    public LayerMask damageableLayers;
    public int Damage = 20;

    void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & damageableLayers) != 0)
        {
            other.GetComponent<IDamageable>().TakeDamage(Damage);
        }
    }
}
