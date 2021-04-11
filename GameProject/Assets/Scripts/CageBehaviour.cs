using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Collider))]
public class CageBehaviour : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    IEnumerator DelayLoadScene()
    {
        yield return new WaitForSeconds(5);
        SceneTravel.GoToScene("EndMenu");
    }

    private void OnTriggerEnter(Collider other)
    {
        RamBehaviour ram = other.GetComponent<RamBehaviour>();
        if (ram)
        {
            _audioSource.Play();

            ram.SetState(RamBehaviour.RamState.Caught);
            ram.GetComponent<NavMeshAgent>().SetDestination(transform.position);

            GetComponent<Animator>().SetTrigger("Open");

            StartCoroutine("DelayLoadScene");
        }
    }
}
