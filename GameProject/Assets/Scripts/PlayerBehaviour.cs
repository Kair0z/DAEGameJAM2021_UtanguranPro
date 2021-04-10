using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

[RequireComponent(typeof(Rigidbody))]
public class PlayerBehaviour : MonoBehaviour
{
    enum State
    {
        JustFineImGoodThanksForAskingHehe,
        Dazed
    }
    private Rigidbody _rigidbody;
    private Vector2 _input;
    private bool _hasBarked = false;
    private bool _hasDashed = false;
    private Timer _barkCooldown = new Timer();
    private Timer _dashCooldown = new Timer();

    private State _state = State.JustFineImGoodThanksForAskingHehe;
    [SerializeField] private float dazedTime = 2.0f;
    private Timer _dazedTimer = new Timer();

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 1.0f;
    [SerializeField] private float moveMaxSpeed = 100.0f;

    [Header("Bark")]
    [SerializeField] private float barkCooldown = 1.0f;
    [SerializeField] private float barkMaxRadius;

    [Header("Dash")]
    [SerializeField] private float dashCooldown = 2.0f;
    [SerializeField] private float dashPower = 1.0f;

    [Header("Particles")]
    [SerializeField] private GameObject shoutParticles = null;

    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    private List<AudioClip> _audioClips = new List<AudioClip>();


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.maxAngularVelocity = 100000.0f;
        _dazedTimer.Set(dazedTime);
        
    }

    private void Start()
    {
        _dashCooldown.Set(dashCooldown);
        _barkCooldown.Set(barkCooldown);
    }

    private void Update()
    {
        float speed = moveSpeed;
        if (_state == State.Dazed)
        {
            speed *= 0.5f;
            _dazedTimer.OnPing(Time.deltaTime, () => { _state = State.JustFineImGoodThanksForAskingHehe; });
        }
        else
        {
            _rigidbody.AddForce(new Vector3(_input.x, 0.0f, _input.y) * speed, ForceMode.Acceleration);
            _rigidbody.velocity = Vector3.ClampMagnitude(_rigidbody.velocity, moveMaxSpeed);
        }
        

        if (_hasBarked)
            _barkCooldown.OnPing(Time.deltaTime, ResetBark);

        if (_hasDashed)
            _dashCooldown.OnPing(Time.deltaTime, ResetDash);
    }

    public void SetAudioClips(List<AudioClip> clips)
    {
        _audioClips = clips;
    }

    private void PlayRandomBarkClip()
    {
        _audioSource.clip = _audioClips[UnityEngine.Random.Range(0, _audioClips.Count)];
        _audioSource.Play();
    }

    private void ResetBark()
    {
        _hasBarked = false;
    }

    private void ResetDash()
    {
        _hasDashed = false;
    }

    #region Input
    private void OnMove(InputValue value)
    {
        _input = value.Get<Vector2>();
    }
    private void OnBark()
    {
        if (!_hasBarked)
        {
            _hasBarked = true;

            PlayRandomBarkClip();

            RaycastHit[] hits = Physics.SphereCastAll(transform.position, barkMaxRadius, Vector3.right, 0.0f, LayerMask.GetMask("BarkScan"), QueryTriggerInteraction.Collide);
            foreach (RaycastHit hit in hits)
            {
                float distance = Vector3.Distance(hit.point, transform.position);

                GameObject owner = hit.collider.gameObject;
                RamBehaviour ramBehaviour = owner.GetComponent<RamBehaviour>();
                if (ramBehaviour)
                {
                    ramBehaviour.RecieveBark(distance, gameObject);
                }
            }

            // TEMP... or is it?
            //_rigidbody.AddRelativeTorque(new Vector3(0, 1, 0) * 10.0f, ForceMode.Impulse);
            // it is ...
            //yup RIP spinmaster3000

            Animator anim = GetComponentInChildren<Animator>();
            if (anim) anim.SetTrigger("Bark");

            var p = Instantiate(shoutParticles);
            p.transform.position = transform.position;
            p.GetComponent<ParticleSystem>().Play();
            Destroy(p, p.GetComponent<ParticleSystem>().main.startLifetime.constant);
        }
    }
    private void OnDash()
    {
        if (_input == Vector2.zero) return;

        if (!_hasDashed)
        {
            _hasDashed = true;
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.AddForce(new Vector3(_input.x, 0.0f, _input.y) * dashPower, ForceMode.Impulse);
            _dazedTimer.Set(0.3f);
            _state = State.Dazed;
        }
    }

    private void OnPauseGame()
    {
        InGameManager mng = FindObjectOfType<InGameManager>();
        if (mng) mng.PauseGame(!mng.GamePaused);
    }
    #endregion

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<RamBehaviour>())
        {
            Vector3 toMe = transform.position - collision.gameObject.transform.position ;
            _rigidbody.AddForce(100.0f * new Vector3(toMe.x, 0.0f, toMe.z), ForceMode.Impulse);
            _rigidbody.AddRelativeTorque(new Vector3(0, 1, 0) * 10.0f, ForceMode.Impulse);
            _dazedTimer.Set(dazedTime);
            _state = State.Dazed;
            Debug.Log("AUCH");
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, barkMaxRadius);
    }
}
