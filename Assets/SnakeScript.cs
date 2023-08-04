using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SnakeScript : MonoBehaviour
{
    private SnakeControls input = null;
    private Vector2 moveVector = Vector2.zero;
    private Rigidbody2D rb = null;
    private SpriteRenderer spriteRenderer = null;
    private Coroutine gameStart = null;

    public float moveInterval = 0.5f;

    private void Awake()
    {
        input = new SnakeControls();
        rb = GetComponentInChildren<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnEnable()
    {
        input.Enable();
        input.Player.Move.performed += OnMovementPerformed;
    }

    private void OnDisable()
    {
        input.Disable();
        input.Player.Move.performed -= OnMovementPerformed;
    }

    private IEnumerator MoveSnake()
    {
        while (true)
        {
            rb.position += moveVector * spriteRenderer.bounds.size.x;
            yield return new WaitForSeconds(moveInterval);
        }
    }

    private void FixedUpdate()
    {
        if (gameStart == null && moveVector != Vector2.zero)
        {
            gameStart = StartCoroutine(MoveSnake());
        }
    }

    private void OnMovementPerformed(InputAction.CallbackContext value)
    {
        moveVector = value.ReadValue<Vector2>();
    }
}
