using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float _enemyToPlayerTolerance = 30f;
    [SerializeField] private int _maxHP = 100;
    private int _currentHp;
    public static PlayerHealth Instance;
    public static event Action<int, int> HpEvent;
    public static event Action<bool> CanBlockEvent;
    private Transform _camera;

    public int HP
    {
        get { return _currentHp; }
        set
        {
            if (_currentHp != value)
            {
                _currentHp = value;
                HpEvent?.Invoke(_maxHP, _currentHp);
            }
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
            return;
        }

        Instance = this;

        HP = _maxHP;
    }

    private void Start()
    {
        _camera = Camera.main.transform;
    }

    public void TakeDamage(Transform enemy, int value)
    {
        // Enemy must face the player
        var enemyToPlayer = transform.position - enemy.position;
        var playerToEnemyAngle = Vector3.Angle(enemy.forward, enemyToPlayer);

        if (playerToEnemyAngle > _enemyToPlayerTolerance)
        {
            return;
        }

        // Player must face the enemy to be able to block
        if (!PlayerState.IsBlocking)
        {
            HP -= value;
        }

        if (Physics.Raycast(_camera.position, _camera.forward, out var hit, 2.0f, LayerMask.GetMask("Enemy Block Condition"), QueryTriggerInteraction.Collide))
        {
            CanBlockEvent?.Invoke(true);
        }
        else
        {
            HP -= value;
            CanBlockEvent?.Invoke(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown("1"))
        {
            HP -= 20;
        }
        if (Input.GetKeyDown("2"))
        {
            HP += 20;
        }
    }
}
