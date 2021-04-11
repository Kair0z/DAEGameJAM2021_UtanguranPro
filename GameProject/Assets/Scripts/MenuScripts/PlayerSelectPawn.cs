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

    [Header("OnSelect")]
    [SerializeField] private Sprite selectSprite = null;
    [SerializeField] private Sprite deselectSprite = null;
    [SerializeField] private Animator spriteAnimator = null;

    [SerializeField] private float selectedDrag = 4.0f;
    private float defaultDrag = 0.0f;

    private void Start()
    {
        GetComponentInChildren<SpriteRenderer>().sprite = deselectSprite;
        _state = State.Free;
    }
    private void Update()
    {
        GetComponent<Rigidbody2D>().AddForce(_input * movementSpeed, ForceMode2D.Force);
    }

    private void OnMove(InputValue input)
    {
        if (_state != State.Free)
        {
            _input = Vector2.zero;
            return;
        }
        _input = input.Get<Vector2>();
    }

    // Input catch:
    private void OnSelect()
    {
        if (_state == State.Free) Lock();
        else Free();
    }

    private void Free()
    {
        return;
        if (!IsInField()) return;

        //PlayerSelectManager manager = FindObjectOfType<PlayerSelectManager>();
        //if (manager) manager.SignalPlayerLock(GetComponent<PlayerInput>());

        GetComponent<Rigidbody2D>().angularDrag = defaultDrag;
        GetComponentInChildren<SpriteRenderer>().sprite = deselectSprite;
        

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

        GetComponentInChildren<SpriteRenderer>().sprite = selectSprite;

        GetComponent<Rigidbody2D>().angularDrag = 0.05f;
        defaultDrag = GetComponent<Rigidbody2D>().drag;
        GetComponent<Rigidbody2D>().drag = selectedDrag;
        spriteAnimator.SetTrigger("Join");

        _state = State.Locked;

        return;
    }
}
