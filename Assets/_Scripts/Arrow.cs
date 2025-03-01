using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float _speed = 10f;
    public float _displacementAmount = 1.0f;
    private float _lifetime = 10f;
    private float _currentLifetime;

    public LayerMask damageableLayers;
    public LayerMask ignoreLayers;
    public int Damage = 20;
    private Collider _collider;
    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    void FixedUpdate()
    {
        transform.position += -transform.forward * _speed * Time.deltaTime;
        _currentLifetime += Time.deltaTime;
        if (_currentLifetime >= _lifetime)
        {
            Destroy(gameObject);
        }

        if (Physics.Raycast(transform.position, -transform.forward, out var hit, 2f, ~ignoreLayers))
        {
            var collider = hit.collider;
            if (((1 << collider.gameObject.layer) & damageableLayers) != 0)
            {
                collider.GetComponent<IDamageable>().TakeDamage(Damage);
                Destroy(gameObject);
            }


            _speed = 0;
            _collider.enabled = false;
            transform.position = hit.point + transform.forward * _displacementAmount;
            // MEMO: Be careful when the scale of the object is not vector.identity since the arrow will scale 
            gameObject.transform.parent = collider.transform;
        }
    }
}
