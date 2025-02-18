using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float _enemyToPlayerTolerance = 30f;
    [SerializeField] private int _hp = 100;
    public static PlayerHealth Instance;
    public static event Action<int> HpEvent;
    public static event Action<bool> CanBlockEvent;
    private Transform _camera;

    public int HP
    {
        get { return _hp; }
        set
        {
            if (_hp != value)
            {
                _hp = value;
                HpEvent?.Invoke(_hp);
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
    }

    private void Start()
    {
        _camera = Camera.main.transform;
    }

    public void TakeDamage(Transform enemy, int value)
    {
        var enemyToPlayer = transform.position - enemy.position;
        var playerToEnemyAngle = Vector3.Angle(enemy.forward, enemyToPlayer);

        if (playerToEnemyAngle > _enemyToPlayerTolerance)
        {
            return;
        }


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
