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
    [SerializeField] private float _wanderFrequency = 1.0f;

    [Header("Steering")]
    [SerializeField] private float _playerAwareness = 50.0f;
    [SerializeField] private Transform _centerTarget;
    [SerializeField] private float _wallAvoidanceMultiplier = 1.0f;

    private Vector3 _steerFromCage = new Vector3();
    private Vector3 _steerFromPlayers = new Vector3();
    private Vector3 _steerToCenter = new Vector3();
    private Vector3 _steerWander = new Vector3();
    private Vector3 _steerSum = new Vector3();
    private Timer _wanderTimer = new Timer();

    [Header("Rage")]
    [SerializeField] private float _chargeSpeed = 10.0f;
    [SerializeField] private float _rageIncreaseAmount = 10.0f;
    private float _rageBar = 0;

    [SerializeField] private Cinemachine.CinemachineImpulseSource _cameraShake;

    private void Start()
    {
        _state = RamState.Wander;
        _wanderTimer.Set(_wanderFrequency);
        _navMesh.speed = _wanderSpeed;
        CalculateWanderTarget();
    }

    private void Update()
    {
        switch (_state)
        {
            case RamState.Wander:
                Wander();
                break;
            case RamState.Rage:
                Rage();
                break;
            default:
                break;
        }
    }

    private void MoveToTarget()
    {
        _navMesh.SetDestination(_targetPosition);
    }

    

    public void RecieveBark(float barkPower, Vector3 barkPosition)
    {
        FillRageBar();

        return;

        float randomnessFactor = 1.0f;
        Vector3 dirRandom = UnityEngine.Random.insideUnitSphere * randomnessFactor;

        Vector3 direction = (transform.position - barkPosition + dirRandom).normalized * barkPower;
        direction += transform.position;

        NavMesh.SamplePosition(direction, out NavMeshHit hit, barkPower, 1);
        if (hit.hit)
        {
            _targetPosition = hit.position;
        }
    }


    private void CalculateWanderTarget()
    {
        // Wanderforce:
        _steerWander = Random.insideUnitSphere * Random.Range(_minWanderRadius, _maxWanderRadius);

        // Avoid walls force:
        _steerToCenter = (_centerTarget.position - transform.position).normalized * _wallAvoidanceMultiplier;

        // Avoid Players force:
        _steerFromPlayers = new Vector3();
        PlayerBehaviour[] players = FindObjectsOfType<PlayerBehaviour>();
        foreach (PlayerBehaviour player in players) // BAD
        {
            float noticePlayerThreshold = 50.0f;

            float distance = (transform.position - player.transform.position).magnitude;
            float distanceNormalized = Mathf.Clamp(distance / noticePlayerThreshold, 0.0f, 1.0f);
            _steerFromPlayers += ((transform.position - player.transform.position) / distance) / distanceNormalized;
        }
        _steerFromPlayers /= players.Length;

        // Barked at force:
        Vector3 barkedForce = new Vector3();

        _steerSum = (_steerWander + _steerFromPlayers + _steerToCenter + barkedForce) / 4.0f;
        NavMesh.SamplePosition(transform.position + _steerSum, out NavMeshHit hit, _maxWanderRadius, 1);
        if (hit.hit)
        {
            _targetPosition = hit.position;
        }
    }
    private void Wander()
    {
        _wanderTimer.OnPing(Time.deltaTime, CalculateWanderTarget);
        
        _navMesh.speed = _wanderSpeed;
        MoveToTarget();
    }

    private void Rage()
    {
        _rageBar -= Time.deltaTime;
        Charge();

        if (_rageBar <= 0.0f)
        {
            _state = RamState.Wander;
            _rageBar = 0.0f;

            Animator anim = GetComponentInChildren<Animator>();
            if (anim) anim.SetTrigger("Calm");
        }
    }

    private void Charge()
    {
        // set target location to grain field

        _navMesh.speed = _chargeSpeed;
        MoveToTarget();
    }

    private void FillRageBar()
    {
        _rageBar += _rageIncreaseAmount;
        if (_rageBar >= 100.0f)
        {
            _state = RamState.Rage;
            Animator anim = GetComponentInChildren<Animator>();
            if (anim) anim.SetTrigger("Enrage");

            Debug.Log("ENRAGE");
        }
    }

    private void Trample()
    {

    }

    private void OnDrawGizmosSelected()
    {
        // Draw Steering Lines...
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + _steerToCenter);
        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position, transform.position + _steerFromCage);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + _steerFromPlayers);
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + _steerWander);
    }
}