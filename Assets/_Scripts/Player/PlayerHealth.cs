using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float _blockAngleTolerance = 50f;
    public static PlayerHealth Instance;
    public static event Action<int> HpEvent;

    [SerializeField] private int _hp = 100;
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

    public void TakeDamage(Vector3 swordPos, int value)
    {
        if (!PlayerState.IsBlocking)
        {
            HP -= value;
            return;
        }
        // TODO: This should probably be the enemies root position rather then the weapon
        // If its the weapons it could be completely off and the angle might trigger damage
        // Might be okay with just doing this however
        var camTransform = Camera.main.transform;
        var swordDir = swordPos - camTransform.position;
        var angle = Vector3.Angle(camTransform.forward, swordDir);
        if (angle > _blockAngleTolerance)
        {
            HP -= value;
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
