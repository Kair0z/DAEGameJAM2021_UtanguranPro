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

    [SerializeField] private NavMeshAgent _navMeshAgent;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private bool _posVelocity = false;

    [Header("Wander")]
    [SerializeField] private float _maxWanderRadius = 5.0f;
    [SerializeField] private float _wanderFrequency = 3.0f;
    [SerializeField] private float _wanderMovementSpeed = 8.0f;

    [Header("Flee")]
    [SerializeField] private float _fleeRadius = 3.0f;
    [SerializeField] private float _fleeMovementSpeed = 11.0f;

    private PreyState _state = PreyState.Wander;
    private Timer _wanderTimer = new Timer();

    private void Start()
    {
        _state = PreyState.Wander;
        _navMeshAgent.speed = _wanderMovementSpeed;
        _wanderTimer.Set(_wanderFrequency);
    }

    private void Update()
    {
        //turn texture somehow?
        if (_navMeshAgent.velocity.x >= 0f && _posVelocity == false)
        {
            //texture
            _spriteRenderer.flipX = false;
            _posVelocity = true;
        }
        if (_navMeshAgent.velocity.x < 0f && _posVelocity == true)
        {
            //texture
            _spriteRenderer.flipX = true;
            _posVelocity = false;
        }

        switch (_state)
        {
            case PreyState.Wander:
                _wanderTimer.OnPing(Time.deltaTime, DetermineNewWanderTarget);
                break;
            case PreyState.Flee:
                if (ReachedDestination())
                {
                    _state = PreyState.Wander;
                    _navMeshAgent.speed = _wanderMovementSpeed;
                }
                break;
            default:
                break;
        }

    }

    private void DetermineNewWanderTarget()
    {
        Vector3 wander = Random.insideUnitSphere * Random.Range(0f, _maxWanderRadius);
        NavMesh.SamplePosition(transform.position + wander, out NavMeshHit hit, _maxWanderRadius, 1);
        _navMeshAgent.SetDestination(hit.position);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _maxWanderRadius);
    }

    public void RecieveBark(float barkPower, GameObject barker)
    {
        Debug.Log("barked at duck");
        // Move away from barker + random // FLEE
        Vector3 direction = (transform.position - barker.transform.position).normalized + Random.insideUnitSphere;
        direction.Normalize();
        NavMesh.SamplePosition(transform.position + (direction * (_fleeRadius)) /*barkpower == distance from bark source*/, out NavMeshHit hit, 1000, 1);
        if (hit.hit)
        {
            _navMeshAgent.SetDestination(hit.position);
            _state = PreyState.Flee;
            _navMeshAgent.speed = _fleeMovementSpeed;
        }
    }

    private bool ReachedDestination()
    {
        if (Equals(_navMeshAgent.destination.x, transform.position.x) && Equals(_navMeshAgent.destination.z, transform.position.z))
        {
            return true;
        }
        return false;
    }

}
