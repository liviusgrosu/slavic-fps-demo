using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Tooltip("Angle in which the enemy has to face in order to consider their damage to the player")]
    [SerializeField] private float _enemyToPlayerTolerance = 30f;
    [SerializeField] private int _maxHP = 100;
    private int _currentHp;
    public static PlayerHealth Instance;
    public static event Action<int, int> HpEvent;
    public static event Action PlayerDied;

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
                if (_currentHp <= 0)
                {
                    PlayerDied?.Invoke();
                }
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
        Respawner.TriggerRestart += ResetHealth;

        _camera = Camera.main.transform;
    }

    public void TakeDamage(int value)
    {
        // Generic function so we can always be hit
        HP -= value;
        SoundManager.Instance.PlaySoundFXClip($"Blood Impact {UnityEngine.Random.Range(1, 3)}", transform);
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
            SoundManager.Instance.PlaySoundFXClip($"Blood Impact {UnityEngine.Random.Range(1, 3)}", transform);
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

    public bool IsDead()
    {
        return HP <= 0;
    }

    private void ResetHealth()
    {
        HP = _maxHP;
    }
}
