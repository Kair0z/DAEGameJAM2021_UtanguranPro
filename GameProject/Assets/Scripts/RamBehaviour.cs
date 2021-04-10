using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//using System;
using Cinemachine.PostFX;

public class RamBehaviour : MonoBehaviour
{
    public enum RamState
    {
        Wander,
        Flee,
        Rage
    }

    //private Action OnPlayerBark;
    private RamState _state = RamState.Wander;

    [Header("Movement")]
    [SerializeField] private NavMeshAgent _navMesh;
    Vector3 _targetPosition;

    [Header("Wander")]
    [SerializeField] private float _maxWanderRadius = 5.0f;
    [SerializeField] private float _minWanderRadius = 1.0f;
    [SerializeField] private float _wanderSpeed = 3.0f;
    private Timer _wanderTimer = new Timer();

    [Header("Flee")]
    [SerializeField] private float _fleeSpeed = 10.0f;
    [SerializeField] private float _fleeMultiplier = 5.0f;

    [Header("Rage")]
    [SerializeField] private float _chargeSpeed = 10.0f;
    [SerializeField] private float _rageIncreaseAmount = 10.0f;
    private float _rageBar = 0;

    [SerializeField] private Cinemachine.CinemachineImpulseSource _cameraShake;

    private void Start()
    {
        _state = RamState.Wander;
        _wanderTimer.Set(3.0f);
        _navMesh.speed = _wanderSpeed;
        SetRandomDestination();
    }

    private void Update()
    {
        switch (_state)
        {
            case RamState.Wander:
                Wander();
                break;
            case RamState.Flee:
                Flee();
                break;
            case RamState.Rage:
                Rage();
                break;
            default:
                break;
        }
    }

    private void Move()
    {
        _navMesh.SetDestination(_targetPosition);
    }

    private void SetRandomDestination()
    {
        if (_cameraShake) _cameraShake.GenerateImpulse();

        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * UnityEngine.Random.Range(_minWanderRadius, _maxWanderRadius);
        randomDirection += transform.position;
        NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, _maxWanderRadius, 1);
        if (hit.hit)
        {
            _targetPosition = hit.position;
        }
    }

    public void RecieveBark(float barkPower, Vector3 barkPosition)
    {
        FillRageBar();

        _state = RamState.Flee;

        barkPower *= _fleeMultiplier;

        float randomnessFactor = 1.0f;
        Vector3 dirRandom = UnityEngine.Random.insideUnitSphere * randomnessFactor;

        Vector3 direction = (transform.position - barkPosition + dirRandom).normalized;
        direction += transform.position;

        NavMesh.SamplePosition(direction, out NavMeshHit hit, barkPower, 1);
        if (hit.hit)
        {
            _targetPosition = hit.position;
        }
    }

    private void Wander()
    {
        _wanderTimer.OnPing(Time.deltaTime, SetRandomDestination);
        
        _navMesh.speed = _wanderSpeed;
        Move();
    }

    private void Flee()
    {
        _navMesh.speed = _fleeSpeed;
        Move();

        if (Equals(transform.position.x, _targetPosition.x) && Equals(transform.position.z, _targetPosition.z))
        {
            _state = RamState.Wander;
        }
    }

    private void Rage()
    {
        _rageBar -= 0.2f;
        Charge();

        if (_rageBar <= 0.0f)
        {
            _state = RamState.Wander;
            _rageBar = 0.0f;
        }
    }

    private void Charge()
    {
        // set target location to grain field

        _navMesh.speed = _chargeSpeed;
        Move();
    }

    private void FillRageBar()
    {
        _rageBar += _rageIncreaseAmount;
        if (Equals(_rageBar, 100.0f))
        {
            _state = RamState.Rage;
        }
    }

    private void Trample()
    {

    }
}