﻿using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ArcherEnemyBehaviour : MonoBehaviour
{
    public enum State
    {
        Idle,
        Engage,
        Attack,
        Check,
        Return
    }

    [Header("General")]
    [Tooltip("Turn on/off the behaviour")]
    [SerializeField] private bool _toggle = true;
    [Tooltip("Angle until rotation is complete")]
    [SerializeField] private float _rotationTolerance;

    [Header("Idle State")]
    [Tooltip("FOV of enemy")]
    [SerializeField] private float _fov;
    [Tooltip("How far the player needs to be from the enemy to engage")]
    [SerializeField] private float _engageDistance;
    [Tooltip("How far the player needs to be from the enemy to disengage (MUST be larger then engage distance)")]
    [SerializeField] private float _disengageDistance;
    [Tooltip("How fast the enemy will rotate back to the starting direction they were facing")]
    [SerializeField] private float _startingRotationSpeed = 250f;

    [Header("Check State")]
    [Tooltip("How long the enemy will wait before returning to idle state")]
    [SerializeField] private float _checkStateTime = 2f;

    [Header("Attack State")]
    [Tooltip("How fast the enemy will rotate to the player after finishing an attack")]
    [SerializeField] private float _toPlayerRotateAttackSpeed = 250f;
    [Tooltip("How far the player needs to be for the enemy to get get back to attack range")]
    [SerializeField] private float _maxAttackDistance;


    private State _currentState = State.Idle;
    private Transform _player;
    private NavMeshAgent _agent;
    private EnemyBowAttackingBehaviour _enemyAttackingBehaviour;

    private Vector3 _startingPosition;
    private float _startingStoppingDistance;
    private Quaternion _startingRotation;
    private float _checkStateElapsedTime;
    // TODO: might be inefficent to do this as remaining distance is already calcuated
    // However remaining distance starts at 0 for the first frame
    // Maybe we just don't have the player right in front of the enemy at start
    private float _getDistanceFromPlayer => Vector3.Distance(transform.position, _player.position);

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _enemyAttackingBehaviour = transform.GetComponentInChildren<EnemyBowAttackingBehaviour>();
        _startingStoppingDistance = _agent.stoppingDistance;
        _startingRotation = transform.rotation;
        _currentState = State.Idle;
    }

    private void Start()
    {
        Respawner.TriggerRestart += RestartBehaviour;

        _startingPosition = transform.position;
        _player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (!_toggle)
        {
            return;
        }

        CheckIfPlayerInFov();

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
            case State.Check:
                CheckState();
                break;
            case State.Return:
                ReturnState();
                break;
        }
    }

    private void IdleState()
    {
        if (Quaternion.Angle(transform.rotation, _startingRotation) > _rotationTolerance)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, _startingRotation, _startingRotationSpeed * Time.deltaTime);
        }
    }

    private void EngageState()
    {
        _agent.SetDestination(_player.position);

        // Adding tolerance since sometimes the enemy will be stuck in a weird state and not move because the agent has a tolerance for stopping
        if (_getDistanceFromPlayer <= _agent.stoppingDistance + 0.1f)
        {
            _agent.velocity = Vector3.zero;
            _currentState = State.Attack;
        }

        if (_getDistanceFromPlayer > _disengageDistance)
        {
            _agent.isStopped = true;
            _checkStateElapsedTime = 0f;
            _currentState = State.Check;
        }
    }

    private void AttackState()
    {
        if (!_enemyAttackingBehaviour.IsAttacking())
        {
            Vector3 direction = (_player.position - transform.position).normalized;
            direction.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _toPlayerRotateAttackSpeed * Time.deltaTime);

            if (Quaternion.Angle(transform.rotation, targetRotation) < _rotationTolerance)
            {
                //_enemyAttackingBehaviour.Attack();
            }

            if (_getDistanceFromPlayer > _maxAttackDistance)
            {
                _agent.velocity = _agent.desiredVelocity;
                _currentState = State.Engage;
            }
        }
    }

    private void CheckState()
    {
        _checkStateElapsedTime += Time.deltaTime;

        if (_checkStateElapsedTime >= _checkStateTime)
        {
            _agent.isStopped = false;
            _agent.velocity = _agent.desiredVelocity;
            _agent.stoppingDistance = 0f;
            _checkStateElapsedTime = 0f;
            _currentState = State.Return;
        }
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

    private void CheckIfPlayerInFov()
    {
        if (_currentState == State.Attack)
        {
            return;
        }

        if (Vector3.Distance(transform.position, _player.position) <= _engageDistance)
        {
            var enemyToPlayer = _player.position - transform.position;

            if (Vector3.Angle(enemyToPlayer, transform.forward) <= _fov)
            {
                _agent.isStopped = false;
                _currentState = State.Engage;
            }
        }
    }

    private void RestartBehaviour()
    {
        _currentState = State.Idle;

        transform.position = _startingPosition;
        transform.rotation = _startingRotation;
        _agent.stoppingDistance = _startingStoppingDistance;
    }
}
