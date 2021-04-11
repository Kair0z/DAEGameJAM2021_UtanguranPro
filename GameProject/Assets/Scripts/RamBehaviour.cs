using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Cinemachine.PostFX;

public class RamBehaviour : MonoBehaviour
{
    public enum RamState
    {
        Wander,
        Rage,
        Flee,
        Flex,
        Caught,
        Idle
    }

    private RamState _state = RamState.Wander;
    private bool _resetWander = false;
    private bool _posVelocity = false;

    [Header("Movement")]
    [SerializeField] private NavMeshAgent _navMeshAgent;
    [SerializeField] private List<GameObject> _cages = new List<GameObject>();
    //Vector3 _targetPosition;
    private List<BoxCollider> _cageColliders = new List<BoxCollider>();

    [Header("Wander")]
    [SerializeField] private float _maxWanderRadius = 5.0f;
    [SerializeField] private float _minWanderRadius = 1.0f;
    [SerializeField] private float _wanderMovementSpeed = 8.0f;
    [SerializeField] private float _wanderFrequency = 1.0f;
    [SerializeField] private float _wanderEdgeMinDistance = 2.0f;
    private Timer _wanderTimer = new Timer();

    [Header("Rage")]
    [SerializeField] private float _chargeSpeed = 15.0f;
    [SerializeField] private float _rageIncreaseMin = 15.0f;
    [SerializeField] private float _rageIncreaseMax = 25.0f;
    [SerializeField] private float _flexTime = 3.0f;
    private float _rageBar = 0f;
    private float _flexTimer = 0f;
    private GameObject _theLastBarker;

    [Header("Flee")]
    [SerializeField] private float _fleeMovementSpeed = 11.0f;
    [SerializeField] private float _fleeRadius = 100.0f;

    [SerializeField] private Cinemachine.CinemachineImpulseSource _cameraShake;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    [Header("Particles")]
    [SerializeField] private GameObject barkReceiveParticles;
    [SerializeField] private ParticleSystem floofPoofParticles;
    [SerializeField] private ParticleSystem scaredParticles;
    [SerializeField] private ParticleSystem angryParticles;

    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioSource _sourceGallop;
    [SerializeField] private AudioSource _sourceTerrain;
    [SerializeField] private AudioClip[] _clipsFlee;
    [SerializeField] private AudioClip[] _clipsFlex;
    [SerializeField] private AudioClip[] _clipsWander;


    private void Start()
    {
        for (int i = 0; i < _cages.Count; i++)
        {
            _cageColliders.Add(_cages[i].GetComponent<BoxCollider>());
        }

;       SetState(RamState.Idle);
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
                        if (hit.hit)
                        {
                            if (IsPosInCage(hit.position))
                            {
                                Vector3 fuckU = transform.position - hit.position;

                                NavMesh.SamplePosition(hit.position + (fuckU * 2f), out NavMeshHit hitShit, _maxWanderRadius, 1);

                                _navMeshAgent.SetDestination(hitShit.position);
                            }
                            else
                            {
                                _navMeshAgent.SetDestination(hit.position);
                            }
                        }
                    }
                });
                break;
            case RamState.Rage:
                if(ReachedDestination())
                {
                    SetState(RamState.Wander);
                }
                break;
            case RamState.Flex:
                _flexTimer += Time.deltaTime;
                if(_flexTimer >= _flexTime)
                {
                    _flexTimer = 0f;
                    SetState(RamState.Rage);
                }
                break;
            case RamState.Flee:
                if (ReachedDestination())
                {
                    SetState(RamState.Wander);
                }
                break;
            case RamState.Caught:
                break;
            case RamState.Idle:
                break;
            default:
                break;
        }
    }

    public void RecieveBark(float barkPower, GameObject barker)
    {
        if (_state == RamState.Caught) return;

        //dont flee when raging or fleeing or flexing
        if (_state == RamState.Rage || _state == RamState.Flee || _state == RamState.Flex)
        {
            return;
        }

        bool isFlexing = IncreaseRage(Random.Range(_rageIncreaseMin, _rageIncreaseMax));
        if (isFlexing)
        {
            _theLastBarker = barker;
        }
        else
        {
            // Move away from barker + random // FLEE
            Vector3 direction = (transform.position - barker.transform.position).normalized + Random.insideUnitSphere;
            direction.Normalize();
            NavMesh.SamplePosition(transform.position + (direction * (_fleeRadius)) /*barkpower == distance from bark source*/, out NavMeshHit hit, 1000, 1);
            if (hit.hit) _navMeshAgent.SetDestination(hit.position);
            SetState(RamState.Flee);
        }
    }

    public void PlayFloofPoof()
    {
        floofPoofParticles.Play();
    }

    public void SetState(RamState newState)
    {
        if (_state == RamState.Rage && newState != RamState.Rage) // stop galloping after rage
        {
            _sourceGallop.loop = false;
            _sourceGallop.Stop();
        }

        _state = newState;

        Animator anim = GetComponentInChildren<Animator>();
        switch (newState)
        {
            case RamState.Rage:
                _sourceGallop.Play();
                _sourceGallop.loop = true;
                angryParticles.Play();
                if (anim) anim.SetTrigger("Enrage");
                SetRageTarget();
                floofPoofParticles.Play();
                _navMeshAgent.speed = _chargeSpeed;
                break;

            case RamState.Flex:
                PlayRandomClip(_clipsFlex);
                PlayFloofPoof();
                angryParticles.Play();
                if (anim) anim.SetTrigger("FLEX");
                _navMeshAgent.speed = 0f;
                break;

            case RamState.Flee:
                PlayRandomClip(_clipsFlee);
                if (anim) anim.SetTrigger("Enrage");
                scaredParticles.Play();
                _navMeshAgent.speed = _fleeMovementSpeed;
                break;

            case RamState.Wander:
                PlayRandomClip(_clipsWander);
                if (anim) anim.SetTrigger("Calm");
                _resetWander = true;
                _navMeshAgent.speed = _wanderMovementSpeed;
                break;
        }
    }

    private void PlayRandomClip(AudioClip[] clip)
    {
        if (clip.Length == 1)
        {
            _audioSource.clip = clip[0];
            _audioSource.Play();
        }
        else
        {
            _audioSource.clip = clip[UnityEngine.Random.Range(0, clip.Length)];
            _audioSource.Play();
        }
    }


    bool IncreaseRage(float amount)
    {
        _rageBar += amount;
        if (_rageBar >= 100.0f)
        {
            SetState(RamState.Flex);
            _rageBar = 0f;
            return true;
        }

        return false;
    }

    private void SetRageTarget()
    {
        // Move towards barker or random (untill collision) // CHASE
        Vector3 direction;
        if (_theLastBarker)
        {
            direction = (_theLastBarker.transform.position - transform.position);
        }
        else
        {
            direction = Random.insideUnitSphere;
        }
        NavMesh.SamplePosition(transform.position + direction * 30, out NavMeshHit hit, 1000, 1);
        if (hit.hit)
        {
            if (IsPosInCage(hit.position))
            {
                direction = Random.insideUnitSphere;
                NavMesh.SamplePosition(transform.position + direction * 30, out NavMeshHit fuckU, 1000, 1);
                _navMeshAgent.SetDestination(fuckU.position);
            }
            else
            {
                _navMeshAgent.SetDestination(hit.position);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (_state)
        {
            case RamState.Wander:
                break;
            case RamState.Flee:
            case RamState.Rage:
                if (collision.collider.tag == "SolidTerrain")
                {
                    _sourceTerrain.Play();
                    // Go back to Wander
                    SetState(RamState.Wander);
                }
                break;
            case RamState.Caught:
                break;
            case RamState.Idle:
                if (collision.collider.tag == "Player")
                {
                    SetState(RamState.Wander);
                }
                break;
            default:
                break;
        }

        // Camera shake
        _cameraShake.GenerateImpulse();

    }

    private bool ReachedDestination()
    {
        if (Equals(_navMeshAgent.destination.x, transform.position.x) && Equals(_navMeshAgent.destination.z, transform.position.z))
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
        bool isEdgeFound = _navMeshAgent.FindClosestEdge(out NavMeshHit hit);
        if (isEdgeFound && hit.distance < _wanderEdgeMinDistance)
        {
            Vector3 direction = (transform.position - hit.position).normalized + Random.insideUnitSphere;
            direction.Normalize();
            NavMesh.SamplePosition(transform.position + direction * Random.Range(_minWanderRadius, _maxWanderRadius), out hit, _maxWanderRadius, 1);
            if (hit.hit) _navMeshAgent.SetDestination(hit.position);
            return true;
        }
        return false;
    }

    bool IsPosInCage(Vector3 pos)
    {
        for (int i = 0; i < _cageColliders.Count; i++)
        {
            if(_cageColliders[i].bounds.Contains(pos))
            {
                return true;
            }
        }

        return false;
    }
}
