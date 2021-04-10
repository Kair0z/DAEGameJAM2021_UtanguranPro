using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerSelectPawn : MonoBehaviour
{
    enum State
    {
        Free,
        Locked
    }

    private State _state = State.Free;
    private Vector2 _input;

    [SerializeField] private float movementSpeed = 1.0f;

    private void Start()
    {
    }

    private void Update()
    {
        if (_state == State.Free) 
            GetComponent<Rigidbody2D>().AddForce(_input * movementSpeed, ForceMode2D.Force);
    }

    private void OnMove(InputValue input)
    {
        _input = input.Get<Vector2>();
    }

    private void OnSelect()
    {
        if (_state == State.Free) Lock();
        else Free();
    }

    private void Free()
    {
        if (!IsInField()) return;

        PlayerSelectManager manager = FindObjectOfType<PlayerSelectManager>();
        if (manager) manager.SignalPlayerLock(GetComponent<PlayerInput>());

        GetComponent<Rigidbody2D>().angularDrag = 10.0f;

        Debug.Log("Freed!");
        _state = State.Free;
    }

    bool IsInField()
    {
        CircleCollider2D myCollider = GetComponent<CircleCollider2D>();
        foreach (Collider2D other in Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), myCollider.radius))
        {
            if (other.tag == "SecretSurprise")
            {
                return true;
            }
        }

        return false;
    }
    private void Lock()
    {
        if (!IsInField()) return;

        PlayerSelectManager manager = FindObjectOfType<PlayerSelectManager>();
        if (manager) manager.SignalPlayerLock(GetComponent<PlayerInput>());

        GetComponent<Rigidbody2D>().angularDrag = 0.05f;

        _state = State.Locked;

        return;
    }
}
