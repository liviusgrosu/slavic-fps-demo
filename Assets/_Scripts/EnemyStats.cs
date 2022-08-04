using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Configuration", menuName = "ScriptableObject/Enemy Configuration")]
public class EnemyStats : ScriptableObject
{
    [Header("General")]
    public float maxHealth = 100f;
    public float attackDamage = 10f;
    public float attackDelay = 0.1f;

    [Header("Nav Mesh")]
    public int areaMask = -1;
    public float radius = 0.5f;
    public float stoppingDistance = 0.5f;
    
    [Header("Movement")]
    public float movementSpeed = 1f;
    public float pursueRadius = 10f;
    public float engageRadius = 2f;
}
