using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerBehaviour : MonoBehaviour
{
    private CharacterController _controller;
    private Vector2 _input;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 1.0f;

    [Header("Bark")]
    [SerializeField] private float barkMaxRadius;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Vector3 deltaMove = moveSpeed * Time.deltaTime * new Vector3(_input.x, 0.0f, _input.y);
        _controller.Move(deltaMove);
    }

    private void OnMove(InputValue value)
    {
        _input = value.Get<Vector2>();
    }

    private void OnBark()
    {
        RaycastHit spherecastHit = new RaycastHit();
        if (Physics.SphereCast(transform.position, 10000.0f, transform.forward, out spherecastHit, 0.0f, LayerMask.GetMask("PlayerBarkScan"), QueryTriggerInteraction.Collide))
        {
            Debug.Log("HIT SOMETHING!" + spherecastHit.collider.gameObject.name);
        }
        
    }

    private void OnDash()
    {

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, barkMaxRadius);
    }
}
