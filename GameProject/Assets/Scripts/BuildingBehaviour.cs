using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BuildingBehaviour : MonoBehaviour
{

    private Collider _trigger;

    void Start()
    {
        _trigger = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ram"))
        {
            Debug.Log("RAM HIT DIS SHIT!");
        }
    }
}
