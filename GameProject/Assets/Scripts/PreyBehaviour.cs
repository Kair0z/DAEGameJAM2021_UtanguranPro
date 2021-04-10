using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PreyBehaviour : MonoBehaviour
{
    private enum PreyState
    {
        Wander,
        Flee
    }

    Vector3 _targetPosition;

    [SerializeField] private NavMeshAgent navMeshAgentComp = null;

    [Header("Wander")]
    [SerializeField] private float wanderRadius = 5.0f;
    [SerializeField] private float wanderTime = 3.0f;

    [Header("Flee")]
    [SerializeField] private float _fleeMultiplier = 3.0f;

    private PreyState _state = PreyState.Wander;
    private Timer _wanderTimer = new Timer();

    private void Start()
    {
        _state = PreyState.Wander;
        _wanderTimer.Set(wanderTime);
    }

    private void Update()
    {
        switch (_state)
        {
            case PreyState.Wander:
                _wanderTimer.OnPing(Time.deltaTime, DetermineNewWanderTarget);
                break;
            case PreyState.Flee:
                Flee();
                break;
            default:
                break;
        }

    }

    private void DetermineNewWanderTarget()
    {
        Vector2 point = Random.insideUnitCircle.normalized * Random.Range(0, wanderRadius);
        navMeshAgentComp.SetDestination(new Vector3(point.x, 0f, point.y));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);
    }

    public void RecieveBark(float barkPower, Vector3 barkPosition)
    {
        _state = PreyState.Flee;

        barkPower *= _fleeMultiplier;

        float randomnessFactor = 1.0f;
        Vector3 dirRandom = UnityEngine.Random.insideUnitSphere * randomnessFactor;

        Vector3 direction = (transform.position - barkPosition + dirRandom).normalized * barkPower;
        direction += transform.position;

        NavMesh.SamplePosition(direction, out NavMeshHit hit, barkPower, 1);
        _targetPosition = hit.position;
        navMeshAgentComp.SetDestination(_targetPosition);
    }

    private void Flee()
    {
        if (Equals(transform.position.x, _targetPosition.x) && Equals(transform.position.z, _targetPosition.z))
        {
            _state = PreyState.Wander;
        }
    }

}
