using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Collider))]
public class CageBehaviour : MonoBehaviour
{
    private Collider _trigger;

    private void Awake()
    {
        _trigger = GetComponent<Collider>();
    }

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
            ram.SetState(RamBehaviour.RamState.Caught);
            ram.GetComponent<NavMeshAgent>().SetDestination(transform.position);

            GetComponent<Animator>().SetTrigger("Open");

            StartCoroutine("DelayLoadScene");
        }
    }
}
