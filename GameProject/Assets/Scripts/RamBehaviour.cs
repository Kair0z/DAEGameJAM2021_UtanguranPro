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
        Rage,
        Flee,
        Caught
    }

    //private Action OnPlayerBark;
    private RamState _state = RamState.Wander;

    [Header("Movement")]
    [SerializeField] private NavMeshAgent _navMesh;
    Vector3 _targetPosition;

    [Header("Wander")]
    [SerializeField] private float _maxWanderRadius = 5.0f;
    [SerializeField] private float _minWanderRadius = 1.0f;
    [SerializeField] private float _wanderMovementSpeed = 8.0f;
    [SerializeField] private float _wanderFrequency = 1.0f;
    private Timer _wanderTimer = new Timer();

    [Header("Rage")]
    [SerializeField] private float _chargeSpeed = 10.0f;
    [SerializeField] private float _defaultRageIncrease = 10.0f;
    private float _rageBar = 0;

    [SerializeField] private Transform _levelCenter;
    [SerializeField] private Cinemachine.CinemachineImpulseSource _cameraShake;

    private void Start()
    {
        _state = RamState.Wander;
        _wanderTimer.Set(_wanderFrequency);
        _navMesh.speed = _wanderMovementSpeed;
    }

    private void Update()
    {
        // Udate WanderTimer & sometimes set new target
        _wanderTimer.OnPing(Time.deltaTime, () =>
        {
            if (Random.Range(-1.0f, 1.0f) > 0.0f)
            {
                // Go across center
                Vector3 toCenter = (_levelCenter.position - transform.position).normalized;
                NavMesh.SamplePosition(transform.position + toCenter * 2, out NavMeshHit hit, 10000, 1);
                if (hit.hit) _navMesh.SetDestination(hit.position);
            }
            else
            {
                // Go wander
                Vector3 wander = Random.insideUnitSphere * Random.Range(_minWanderRadius, _maxWanderRadius);
                NavMesh.SamplePosition(transform.position + wander, out NavMeshHit hit, _maxWanderRadius, 1);
                if (hit.hit) _navMesh.SetDestination(hit.position);
            }
           
        });
    }

    

    public void RecieveBark(float barkPower, GameObject barker)
    {
        bool isEnraged = IncreaseRage(_defaultRageIncrease);
        if (isEnraged)
        {
            // Move towards barker + random (untill collision) // CHASE
            Vector3 direction = (barker.transform.position - transform.position).normalized;
            NavMesh.SamplePosition(transform.position + direction * 1000, out NavMeshHit hit, 1000, 1);
            if (hit.hit) _navMesh.SetDestination(hit.position);
        }
        else
        {
            // Move away from barker + random // FLEE
            Vector3 direction = (transform.position - barker.transform.position).normalized;
            NavMesh.SamplePosition(transform.position + direction * 1000, out NavMeshHit hit, 1000, 1);
            if (hit.hit) _navMesh.SetDestination(hit.position);
        }

        Debug.Log("OI");
    }

    void SetState(RamState newState)
    {
        switch (_state)
        {
            case RamState.Rage: 
                _wanderFrequency -= 0.2f;
                _rageBar = 0.0f;
                break;
        }
        _state = newState;

        Animator anim = GetComponentInChildren<Animator>();
        switch (newState)
        {
            case RamState.Rage:
                if (anim) anim.SetTrigger("Enrage");
                break;

            case RamState.Flee:
                if (anim) anim.SetTrigger("Enrage");
                break;

            case RamState.Wander:
                if (anim) anim.SetTrigger("Calm");
                break;
        }
    }

    bool IncreaseRage(float amount)
    {
        _rageBar += amount;
        if (_rageBar >= 100.0f)
        {
            SetState(RamState.Rage);
            return true;
        }

        return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Go back to Wander
        SetState(RamState.Wander);

        // Camera shake
        _cameraShake.GenerateImpulse();
    }

    private void OnDrawGizmosSelected()
    {
    }
}