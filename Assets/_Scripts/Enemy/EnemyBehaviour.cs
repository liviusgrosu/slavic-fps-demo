﻿using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    public enum State
    {
        Idle,
        Engage,
        Attack,
        Return
    }

    [Header("General")]
    [Tooltip("Angle until rotation is complete")]
    [SerializeField] private float _rotationTolernace;

    [Header("Idle State")]
    [Tooltip("FOV of enemy")]
    [SerializeField] private float _fov;
    [Tooltip("How far the player needs to be from the enemy to engage")]
    [SerializeField] private float _engageDistance;
    [Tooltip("How fast the enemy will rotate back to the starting direction they were facing")]
    [SerializeField] private float _startingRotationSpeed = 250f;

    [Header("Attack State")]
    [Tooltip("How fast the enemy will rotate to the player after finishing an attack")]
    [SerializeField] private float _toPlayerRotateAttackSpeed = 250f;
    [Tooltip("Delay between each attack chain")]
    [SerializeField] private float _delayAttackTime = 1f;

    private State _currentState = State.Idle;
    private Transform _player;
    private NavMeshAgent _agent;
    private EnemyWeaponBehaviour _swordAnimator;

    private Vector3 _startingPosition;
    private float _startingStoppingDistance;
    private Quaternion _startingRotation;
    // TODO: might be inefficent to do this as remaining distance is already calcuated
    // However remaining distance starts at 0 for the first frame
    // Maybe we just don't have the player right in front of the enemy at start
    private float _getDistanceFromPlayer => Vector3.Distance(transform.position, _player.position);

    private bool _canAttack = true;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _swordAnimator = transform.GetComponentInChildren<EnemyWeaponBehaviour>();
        _startingStoppingDistance = _agent.stoppingDistance;
        _startingRotation = transform.rotation;
        _currentState = State.Idle;
    }

    private void Start()
    {
        _startingPosition = transform.position;
        _player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        switch (_currentState)
        {
            case State.Idle:
                IdleState();
                break;
            case State.Engage:
                EngageState();
                break;
            case State.Attack:
                AttackState();
                break;
            case State.Return:
                ReturnState();
                break;
        }
    }

    private void IdleState()
    {
        if (Quaternion.Angle(transform.rotation, _startingRotation) > _rotationTolernace)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, _startingRotation, _startingRotationSpeed * Time.deltaTime);
        }

        if (Vector3.Distance(transform.position, _player.position) <= _engageDistance)
        {
            var enemyToPlayer = _player.position - transform.position;

            if (Vector3.Angle(enemyToPlayer, transform.forward) <= _fov)
            {
                _currentState = State.Engage;
            }
        }
    }

    private void EngageState()
    {
        _agent.SetDestination(_player.position);

        // Adding tolerance since sometimes the enemy will be stuck in a weird state and not move because the agent has a tolernce for stopping
        if (_getDistanceFromPlayer <= _agent.stoppingDistance + 0.1f)
        {
            _agent.velocity = Vector3.zero;
            _currentState = State.Attack;
        }

        if (_getDistanceFromPlayer > _engageDistance)
        {
            _agent.velocity = _agent.desiredVelocity;
            _agent.stoppingDistance = 0f;
            _currentState = State.Return;
        }
    }

    private void AttackState()
    {
        //TODO: Consider reversing this if statement
        if (!_swordAnimator.IsAttacking())
        {
            Vector3 direction = (_player.position - transform.position).normalized;
            direction.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _toPlayerRotateAttackSpeed * Time.deltaTime);

            if (Quaternion.Angle(transform.rotation, targetRotation) < _rotationTolernace &&
                _canAttack)
            {
                _swordAnimator.PlayAttack();
                _canAttack = false;
                StartCoroutine(ResetAttackCooldown(_delayAttackTime));
            }

            if (_getDistanceFromPlayer > _agent.stoppingDistance)
            {
                _agent.velocity = _agent.desiredVelocity;
                _currentState = State.Engage;
            }
        }
    }

    private void CheckState()
    {
        // This state will make the enemy check left, right, forward, and delay before going to return state
    }

    private void ReturnState()
    {
        _agent.SetDestination(_startingPosition);
        if (Vector3.Distance(transform.position, _startingPosition) < 0.15f)
        {
            _agent.stoppingDistance = _startingStoppingDistance;
            _currentState = State.Idle;
        }
    }

    private IEnumerator ResetAttackCooldown(float delay)
    {
        yield return new WaitForSeconds(delay);
        _canAttack = true;
    }
}
