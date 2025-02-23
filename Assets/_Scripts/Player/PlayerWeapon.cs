using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public static PlayerWeapon Instance;

    public LayerMask damageableLayers;
    public int Damage = 20;
    private Collider _collider;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
            return;
        }
        Instance = this;

        _collider = GetComponent<Collider>();
        ToggleSwordCollider(0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & damageableLayers) != 0)
        {
            other.GetComponent<IDamageable>().TakeDamage(Damage);
        }
    }

    // Animation even can't do bool so we're stuck with ints
    // 0 = false
    // 1 = true
    public void ToggleSwordCollider(int state)
    {
        _collider.enabled = state == 1;
    }
}
