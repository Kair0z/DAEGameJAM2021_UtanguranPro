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
    private bool _resetWander = false;
    private bool _posVelocity = false;

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
    [SerializeField] private float _chargeSpeed = 15.0f;
    [SerializeField] private float _defaultRageIncrease = 10.0f;
    private float _rageBar = 0;

    [Header("Flee")]
    [SerializeField] private float _fleeMovementSpeed = 11.0f;
    [SerializeField] private float _fleeRadius = 100.0f;

    [SerializeField] private Cinemachine.CinemachineImpulseSource _cameraShake;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    [Header("Particles")]
    [SerializeField] private GameObject barkReceiveParticles;
    private void Start()
    {
        SetState(RamState.Wander);
        _wanderTimer.Set(_wanderFrequency);
    }

    private void Update()
    {
        //turn texture somehow?
        if (_navMesh.velocity.x >= 0f && _posVelocity == false)
        {
            //texture
            _spriteRenderer.flipX = false;
            _posVelocity = true;
        }
        if (_navMesh.velocity.x < 0f && _posVelocity == true)
        {
            //texture
            _spriteRenderer.flipX = true;
            _posVelocity = false;
        }

        switch (_state)
        {
            case RamState.Wander:
                // Reset wander when destination reached
                float time = Time.deltaTime;
                if (ReachedDestination()) _resetWander = true;
                if (_resetWander)
                {
                    _resetWander = false;
                    time = 100f;
                }
                // Udate WanderTimer & sometimes set new target
                _wanderTimer.OnPing(time, () =>
                {
                    if (!FindPosAwayFromEdge())
                    {
                        Vector3 wander = Random.insideUnitSphere * Random.Range(_minWanderRadius, _maxWanderRadius);
                        NavMesh.SamplePosition(transform.position + wander, out NavMeshHit hit, _maxWanderRadius, 1);
                        if (hit.hit) _navMesh.SetDestination(hit.position);
                    }
                });
                break;
            case RamState.Rage:
                break;
            case RamState.Flee:
                if (ReachedDestination())
                {
                    SetState(RamState.Wander);
                }
                break;
            case RamState.Caught:
                break;
            default:
                break;
        }
    }



    public void RecieveBark(float barkPower, GameObject barker)
    {
        //dont flee when raging
        if (_state != RamState.Wander)
        {
            Debug.Log("OI IM NOT WANDERING");
            return;
        }

        // Particles of notice!
        if (barkReceiveParticles)
        {
            // position particles in the middle of the sprite
            Vector3 newPos = new Vector3(transform.position.x - _spriteRenderer.transform.localScale.x, transform.position.y, transform.position.z);

            ParticleSystem p = Instantiate(barkReceiveParticles, newPos, transform.rotation, transform).GetComponent<ParticleSystem>();
            p.Play();
            Destroy(p.gameObject, p.main.startLifetime.constant);
        }

        bool isEnraged = IncreaseRage(_defaultRageIncrease);
        if (isEnraged)
        {
            // Move towards barker + random (untill collision) // CHASE
            Vector3 direction = (barker.transform.position - transform.position).normalized /*+ Random.insideUnitSphere*/;
            NavMesh.SamplePosition(transform.position + direction * 1000, out NavMeshHit hit, 1000, 1);
            if (hit.hit) _navMesh.SetDestination(hit.position);
        }
        else
        {
            // Move away from barker + random // FLEE
            Vector3 direction = (transform.position - barker.transform.position).normalized + Random.insideUnitSphere;
            direction.Normalize();
            NavMesh.SamplePosition(transform.position + (direction * (_fleeRadius - barkPower)) /*barkpower == distance from bark source*/, out NavMeshHit hit, 1000, 1);
            if (hit.hit) _navMesh.SetDestination(hit.position);
            SetState(RamState.Flee);
        }

        Debug.Log("OI");
    }

    void SetState(RamState newState)
    {
        _state = newState;

        Animator anim = GetComponentInChildren<Animator>();
        switch (newState)
        {
            case RamState.Rage:
                if (anim) anim.SetTrigger("Enrage");
                _navMesh.speed = _chargeSpeed;
                break;

            case RamState.Flee:
                if (anim) anim.SetTrigger("Enrage");
                _navMesh.speed = _wanderMovementSpeed;
                break;

            case RamState.Wander:
                if (anim) anim.SetTrigger("Calm");
                _resetWander = true;
                _navMesh.speed = _wanderMovementSpeed;
                break;
        }
    }

    bool IncreaseRage(float amount)
    {
        _rageBar += amount;
        if (_rageBar >= 100.0f)
        {
            SetState(RamState.Rage);
            _rageBar = 0f;
            return true;
        }

        return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (_state)
        {
            case RamState.Wander:
            case RamState.Flee:
            case RamState.Rage:
                if (collision.collider.tag == "SolidTerrain")
                {
                    // Go back to Wander
                    SetState(RamState.Wander);
                }
                break;
            case RamState.Caught:
                break;
            default:
                break;
        }

        // Camera shake
        _cameraShake.GenerateImpulse();

        Debug.Log("Collide!");
    }

    private bool ReachedDestination()
    {
        if (Equals(_navMesh.destination.x, transform.position.x) && Equals(_navMesh.destination.z, transform.position.z))
        {
            return true;
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _fleeRadius);
    }

    bool FindPosAwayFromEdge()
    {
        bool isEdgeFound = _navMesh.FindClosestEdge(out NavMeshHit hit);
        float threshHold = 0.5f;
        if (isEdgeFound && hit.distance < threshHold)
        {
            Vector3 direction = (transform.position - hit.position).normalized + Random.insideUnitSphere;
            direction.Normalize();
            NavMesh.SamplePosition(transform.position + direction * Random.Range(_minWanderRadius, _maxWanderRadius), out hit, _maxWanderRadius, 1);
            if (hit.hit) _navMesh.SetDestination(hit.position);
            return true;
        }
        return false;
    }
}
