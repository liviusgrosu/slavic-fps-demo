using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIK : MonoBehaviour
{
    [SerializeField] private int chainLength = 2;
    [SerializeField] private Transform target;
    [SerializeField] private Transform pole;
    [Header("Solver Parameters")]
    [SerializeField] private int iterations = 10;
    [SerializeField] private float delta = 0.001f;
    [Range(0, 1)]
    [SerializeField] private float snapBackStrength = 1f;


    private float[] _bonesLength;
    private float _completeLength;
    private Transform[] _bones;
    private Vector3[] _positions;
    private Vector3[] _startDirectionSuccess;
    private Quaternion[] _startRotationBone;
    private Quaternion _startRotationTarget;
    private Transform _root;


    void Awake()
    {
        Init();
    }

    void Init()
    {
        _bones = new Transform[chainLength + 1];
        _positions = new Vector3[chainLength + 1];
        _bonesLength = new float[chainLength];
        _startDirectionSuccess = new Vector3[chainLength + 1];
        _startRotationBone = new Quaternion[chainLength + 1];

        _root = transform;
        for (int i = 0; i <= chainLength; i++)
        {
            if (_root == null)
            {
                throw new UnityException("The chain value is longer than the ancestor chain");
            }
            _root = _root.parent;
        }

        if (target == null)
        {
            throw new UnityException("Target must exist");
        }
        _startRotationTarget = GetRotationRootSpace(target);


        Transform current = transform;
        _completeLength = 0;
        for (int i = _bones.Length - 1; i >= 0; i--)
        {
            _bones[i] = current;
            _startRotationBone[i] = GetRotationRootSpace(current);

            if (i == _bones.Length - 1)
            {
                _startDirectionSuccess[i] = GetPositionRootSpace(target) - GetPositionRootSpace(current);
            }
            else
            {
                _startDirectionSuccess[i] = GetPositionRootSpace(_bones[i + 1]) - GetPositionRootSpace(current);
                _bonesLength[i] = _startDirectionSuccess[i].magnitude;
                _completeLength += _bonesLength[i];
            }

            current = current.parent;
        }



    }
    
    void LateUpdate()
    {
        ResolveIK();
    }

    private void ResolveIK()
    {
        if (target == null)
        {
            return;
        }

        if (_bonesLength.Length != chainLength)
        {
            Init();
        }

        for (int i = 0; i < _bones.Length; i++)
        {
            _positions[i] = GetPositionRootSpace(_bones[i]);
        }

        Vector3 targetPosition = GetPositionRootSpace(target);
        Quaternion targetRotation = GetRotationRootSpace(target);

        if ((targetPosition - GetPositionRootSpace(_bones[0])).sqrMagnitude >= _completeLength * _completeLength)
        {
            Vector3 direction = (targetPosition - _positions[0]).normalized;
            for (int i = 1; i < _positions.Length; i++)
            {
                _positions[i] = _positions[i - 1] + direction * _bonesLength[i - 1];
            }
        }
        else
        {
            for (int i = 0; i < _positions.Length - 1; i++)
            {
                _positions[i + 1] = Vector3.Lerp(_positions[i + 1], _positions[i] + _startDirectionSuccess[i], snapBackStrength);
            }

            for (int iteration = 0; iteration < iterations; iteration++)
            {
                for (int i = _positions.Length - 1; i > 0; i--)
                {
                    if (i == _positions.Length - 1)
                    {
                        _positions[i] = targetPosition; //set it to target
                    }
                    else
                    {
                        _positions[i] = _positions[i + 1] + (_positions[i] - _positions[i + 1]).normalized * _bonesLength[i]; //set in line on distance
                    }
                }

                for (int i = 1; i < _positions.Length; i++)
                {
                    _positions[i] = _positions[i - 1] + (_positions[i] - _positions[i - 1]).normalized * _bonesLength[i - 1];
                }
                
                if ((_positions[_positions.Length - 1] - targetPosition).sqrMagnitude < delta * delta)
                {
                    break;
                }
            }
        }

        if (pole != null)
        {
            Vector3 polePosition = GetPositionRootSpace(pole);
            for (int i = 1; i < _positions.Length - 1; i++)
            {
                Plane plane = new Plane(_positions[i + 1] - _positions[i - 1], _positions[i - 1]);
                Vector3 projectedPole = plane.ClosestPointOnPlane(polePosition);
                Vector3 projectedBone = plane.ClosestPointOnPlane(_positions[i]);
                float angle = Vector3.SignedAngle(projectedBone - _positions[i - 1], projectedPole - _positions[i - 1], plane.normal);
                _positions[i] = Quaternion.AngleAxis(angle, plane.normal) * (_positions[i] - _positions[i - 1]) + _positions[i - 1];
            }
        }

        for (int i = 0; i < _positions.Length; i++)
        {
            if (i == _positions.Length - 1)
            {
                SetRotationRootSpace(_bones[i], Quaternion.Inverse(targetRotation) * _startRotationTarget * Quaternion.Inverse(_startRotationBone[i]));
            }
            else
            {
                SetRotationRootSpace(_bones[i], Quaternion.FromToRotation(_startDirectionSuccess[i], _positions[i + 1] - _positions[i]) * Quaternion.Inverse(_startRotationBone[i]));
            }
            SetPositionRootSpace(_bones[i], _positions[i]);
        }
    }

    private Vector3 GetPositionRootSpace(Transform current)
    {
        if (_root == null)
        {
            return current.position;
        }
        else
        {
            return Quaternion.Inverse(_root.rotation) * (current.position - _root.position);
        }
    }

    private void SetPositionRootSpace(Transform current, Vector3 position)
    {
        if (_root == null)
        {
            current.position = position;
        }
        else
        {
            current.position = _root.rotation * position + _root.position;
        }
    }

    private Quaternion GetRotationRootSpace(Transform current)
    {
        if (_root == null)
        {
            return current.rotation;
        }
        else
        {
            return Quaternion.Inverse(current.rotation) * _root.rotation;
        }
    }

    private void SetRotationRootSpace(Transform current, Quaternion rotation)
    {
        if (_root == null)
        {
            current.rotation = rotation;
        }
        else
        {
            current.rotation = _root.rotation * rotation;
        }
    }
}