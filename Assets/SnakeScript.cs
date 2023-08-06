using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SnakeScript : MonoBehaviour
{
    private Vector2 moveVector = Vector2.zero;
    private Rigidbody2D rb = null;
    private SpriteRenderer spriteRenderer = null;
    private bool pendingSegment = false;
    private GameObject[] snakeSegments = new GameObject[100];
    private int segmentCount = 0;
    private Coroutine currentCoroutine = null;
    private SnakeControls input = null;
    private Vector2 previousDir;

    public float moveInterval = 5f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        snakeSegments[segmentCount++] = gameObject;
        currentCoroutine = StartCoroutine(GameLoopCoroutine());
        input = new SnakeControls();
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

    private void OnMovementPerformed(InputAction.CallbackContext value)
    {
        Vector2 temp = value.ReadValue<Vector2>();
        if (temp == -previousDir)
        {
            return;
        }
        if (temp.x != 0 && temp.y != 0)
        {
            if (previousDir.x != 0)
            {
                moveVector = new(0, MathF.Round(temp.y));
            }
            else
            {
                moveVector = new(MathF.Round(temp.x), 0);
            }
        }
        else
        {
            moveVector = temp;
        }
        previousDir = moveVector;
    }

    private void MoveSnake()
    {
        for (int i = segmentCount - 1; i >= 0; i--)
        {
            if (i == 0)
            {
                // move the head
                snakeSegments[i].GetComponent<Rigidbody2D>().position += moveVector * spriteRenderer.bounds.size.x;
            }
            else
            {
                // other segments just swap positions
                snakeSegments[i].transform.position = snakeSegments[i - 1].transform.position;
            }
        }

        // Grow the snake if necessary
        if (pendingSegment)
        {
            Grow(snakeSegments[0].transform);
            pendingSegment = false;
        }
    }

    private void KillSnake()
    {
        OnDisable();
        for (var i = 0; i < segmentCount; i++)
        {
            GameObject snakeBits = (GameObject)Instantiate(Resources.Load("SnakeBits"));
            Debug.Log(snakeBits);
            snakeBits.transform.position = snakeSegments[i].transform.position;
            snakeBits.GetComponent<ParticleSystem>().Play();
            Destroy(snakeSegments[i]);
            snakeSegments[i] = null;
        }
        segmentCount = 0;
    }


    private IEnumerator GameLoopCoroutine()
    {
        while (true)
        {
            MoveSnake();
            yield return new WaitForSeconds(moveInterval);
        }
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
        else if (collider2D.CompareTag("Wall"))
        {
            StopCoroutine(currentCoroutine);
            KillSnake();
        }
    }

    private void Grow(Transform parentTransform)
    {
        snakeSegments[segmentCount++] = Instantiate(Resources.Load<GameObject>("SnakeBody"), parentTransform);
    }
}
