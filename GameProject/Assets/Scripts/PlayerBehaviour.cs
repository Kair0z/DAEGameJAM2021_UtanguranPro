using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

[RequireComponent(typeof(Rigidbody))]
public class PlayerBehaviour : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Vector2 _input;
    private Vector2 _dashInput;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 1.0f;

    [Header("Bark")]
    [SerializeField] private float barkMaxRadius;

    [Header("Dash")]
    [SerializeField] private float dashPower = 1.0f;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _rigidbody.AddForce(new Vector3(_input.x, 0.0f, _input.y) * moveSpeed, ForceMode.Force);
    }

    private void OnMove(InputValue value)
    {
        _input = value.Get<Vector2>();
    }

    private void OnBark()
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, 1000.0f, Vector3.right, 0.0f, LayerMask.GetMask("BarkScan"), QueryTriggerInteraction.Collide);
        foreach (RaycastHit hit in hits)
        {
            Debug.Log("HIT SOMETHING!" + hit.collider.gameObject.name);
        }
    }

    private void OnDash()
    {
        if (_input == Vector2.zero) return;

        _rigidbody.AddForce(new Vector3(_input.x, 0.0f, _input.y) * dashPower, ForceMode.Impulse);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, barkMaxRadius);
    }
}
