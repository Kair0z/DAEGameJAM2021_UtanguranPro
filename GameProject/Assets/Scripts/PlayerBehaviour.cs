using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerBehaviour : MonoBehaviour
{
    private CharacterController _controller;
    private Vector2 _input;

    [SerializeField] private float moveSpeed = 1.0f;

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

    }

    private void OnDash()
    {

    }
}
