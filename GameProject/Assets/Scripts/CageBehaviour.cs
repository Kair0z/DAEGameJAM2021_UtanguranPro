using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

[RequireComponent(typeof(Collider))]
public class CageBehaviour : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    private bool ramCaught = false;

    public Action<RamBehaviour> OnRamCaught = (RamBehaviour ramCaught) => { };
    

    private void OnTriggerEnter(Collider other)
    {
        if (ramCaught) return;

        RamBehaviour ram = other.GetComponent<RamBehaviour>();
        if (ram)
        {
            _audioSource.Play();

            ram.SetState(RamBehaviour.RamState.Caught);
            ram.GetComponent<NavMeshAgent>().SetDestination(transform.position);
            GetComponent<Animator>().SetTrigger("Open");
            OnRamCaught(ram);

            ramCaught = true;
        }
    }
}
