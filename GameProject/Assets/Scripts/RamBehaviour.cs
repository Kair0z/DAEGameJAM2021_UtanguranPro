using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RamBehaviour : MonoBehaviour
{
    public enum RamState
    {
        Idle,
        Wandering,
        Rage
    }

    //private RamState _state = RamState.Idle;
    //private Timer _rageTimer = new Timer();

    // Movement
    [SerializeField]
    private NavMeshAgent _navMesh;
    private float _walkRadius = 5.0f;
    Vector3 _targetPosition;
    private Timer _wanderTimer = new Timer();

    //private float _rageBar = 0;

    private void Start()
    {
        //_state = RamState.Wandering;
        _wanderTimer.Set(3.0f);
        SetDestination();
    }

    private void Do()
    {

    }

    private void Update()
    {
        _wanderTimer.OnPing(Time.deltaTime, SetDestination);
        Wander();
    }

    private void Wander()
    {
        _navMesh.SetDestination(_targetPosition);
    }

    private void SetDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * _walkRadius;
        randomDirection += transform.position;
        NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, _walkRadius, 1);
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