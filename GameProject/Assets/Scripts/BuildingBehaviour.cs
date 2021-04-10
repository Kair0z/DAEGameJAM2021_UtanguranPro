using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BuildingBehaviour : MonoBehaviour
{
    private SpriteRenderer _sprite;
    private Collider _trigger;

    private bool _isBroken;

    void Start()
    {
        _trigger = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ram") && !_isBroken)
        {
            _isBroken = true;
            Debug.Log("RAM HIT DIS SHIT!");
            Animator anim = GetComponentInChildren<Animator>();
            if (anim) anim.SetTrigger("BREAK");

        }
    }
}
