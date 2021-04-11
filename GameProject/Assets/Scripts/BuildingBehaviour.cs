using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BuildingBehaviour : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Cinemachine.CinemachineImpulseSource _cameraShake;

    private bool _isBroken = false;

    private void OnTriggerEnter(Collider other)
    {
        if(_isBroken)
        {
            return;
        }
        if (other.CompareTag("ram"))
        {
            _audioSource.Play();
            Animator anim = GetComponentInChildren<Animator>();
            if (anim) anim.SetTrigger("DESTROYED");
            _cameraShake.GenerateImpulse();
            transform.GetComponent<CapsuleCollider>().enabled = false;
        }
    }
}
