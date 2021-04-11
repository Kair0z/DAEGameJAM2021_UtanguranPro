using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BuildingBehaviour : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ram"))
        {
            _audioSource.Play();
            Animator anim = GetComponentInChildren<Animator>();
            if (anim) anim.SetTrigger("DESTROYED");
            transform.GetComponent<CapsuleCollider>().enabled = false;
        }
    }
}
