using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class SnakeScript : MonoBehaviour
{
    private SnakeControls input = null;
    private Vector2 moveVector = Vector2.zero;
    private Rigidbody2D rb = null;
    private SpriteRenderer spriteRenderer = null;
    private Coroutine gameStart = null;
    private bool pendingSegment = false;

    public float moveInterval = 5f;

    private void Awake()
    {
        input = new SnakeControls();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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

    private Vector3[] previousPositions;

    private IEnumerator MoveSnake()
    {
        while (true)
        {
            // Find all active GameObjects in the scene with the "SnakeSegment" tag
            GameObject[] snakeSegments = GameObject.FindGameObjectsWithTag("SnakeSegment");

            // Initialize previousPositions array
            if (previousPositions == null || previousPositions.Length != snakeSegments.Length)
            {
                previousPositions = new Vector3[snakeSegments.Length];
            }

            for (int i = 0; i < snakeSegments.Length; i++)
            {
                if (snakeSegments.Length < 10)
                {
                    pendingSegment = true;
                }

                if (i == 0)
                {
                    // Store previous position
                    previousPositions[i] = snakeSegments[i].transform.position;

                    // Move first segment
                    snakeSegments[i].GetComponent<Rigidbody2D>().position += moveVector * spriteRenderer.bounds.size.x;
                }
                else
                {
                    // Store previous position
                    previousPositions[i] = snakeSegments[i].transform.position;

                    // Move segment to previous position of previous segment
                    snakeSegments[i].transform.position = previousPositions[i - 1];
                }
            }

            // Grow the snake if necessary
            if (pendingSegment)
            {
                Grow(snakeSegments[^1].transform);
                pendingSegment = false;
            }

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

    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.CompareTag("Food"))
        {
            Destroy(GameObject.FindWithTag("Food"));
            Instantiate(Resources.Load("Food"));
            pendingSegment = true;
        }
        else if (collider2D.CompareTag("SnakeSegment"))
        {
            Debug.Log("Collided with snake!");
        }
    }

    private void Grow(Transform parentTransform)
    {
        Instantiate(Resources.Load<GameObject>("SnakeBody"), parentTransform);
    }

}
