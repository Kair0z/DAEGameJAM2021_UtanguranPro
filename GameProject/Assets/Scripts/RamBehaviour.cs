using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

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
    //private Timer _rageTimer = new Timer();

    // Movement
    [SerializeField] private NavMeshAgent _navMesh;
    [SerializeField] private float _maxWalkRadius = 5.0f;
    [SerializeField] private float _minWalkRadius = 1.0f;
    Vector3 _targetPosition;
    private Timer _wanderTimer = new Timer();

    //private float _rageBar = 0;

    private void Start()
    {
        _state = RamState.Wander;
        _wanderTimer.Set(3.0f);
        SetRandomDestination();
    }

    private void Update()
    {
        switch (_state)
        {
            case RamState.Wander:
                _wanderTimer.OnPing(Time.deltaTime, SetRandomDestination);
                Move();
                break;
            case RamState.Flee:
                Move();
                if (Equals(transform.position.x, _targetPosition.x) && Equals(transform.position.z, _targetPosition.z))
                {
                    _state = RamState.Wander;
                }
                break;
            case RamState.Rage:
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
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * UnityEngine.Random.Range(_minWalkRadius, _maxWalkRadius);
        randomDirection += transform.position;
        NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, _maxWalkRadius, 1);
        if (hit.hit)
        {
            _targetPosition = hit.position;
        }
    }

    public void RecieveBark(float barkPower, Vector3 barkPosition)
    {
        _state = RamState.Flee;

        float randomnessFactor = 0.5f;
        Vector3 dirRandom = UnityEngine.Random.insideUnitSphere * randomnessFactor;

        Vector3 direction = (transform.position - barkPosition + dirRandom).normalized;
        direction += transform.position;

        NavMesh.SamplePosition(direction, out NavMeshHit hit, barkPower, 1);
        if (hit.hit)
        {
            _targetPosition = hit.position;
        }
    }

    private void Flee()
    {

    }

    private void Charge()
    {

    }

    private void Eat()
    {

    }

    private void Trample()
    {

    }
}