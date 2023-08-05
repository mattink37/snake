using System.Collections;
using UnityEngine;

public class SnakeScript : MonoBehaviour
{
    private Vector2 moveVector = Vector2.zero;
    private Rigidbody2D rb = null;
    private SpriteRenderer spriteRenderer = null;
    private bool pendingSegment = false;
    private GameObject[] snakeSegments = new GameObject[100];
    private int segmentCount = 0;
    private Coroutine gameLoop = null;

    public float moveInterval = 5f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        snakeSegments[segmentCount++] = gameObject;
        gameLoop = StartCoroutine(MoveSnake());
    }


    private IEnumerator MoveSnake()
    {
        while (true)
        {
            if (Input.GetKey(KeyCode.W) && moveVector != Vector2.down)
            {
                moveVector = Vector2.up;
            }
            else if (Input.GetKey(KeyCode.D) && moveVector != Vector2.left)
            {
                moveVector = Vector2.right;
            }
            else if (Input.GetKey(KeyCode.S) && moveVector != Vector2.up)
            {
                moveVector = Vector2.down;
            }
            else if (Input.GetKey(KeyCode.A) && moveVector != Vector2.right)
            {
                moveVector = Vector2.left;
            }
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
            StopCoroutine(gameLoop);
            for (var i = 0; i < snakeSegments.Length; i++)
            {
                Destroy(snakeSegments[i]);
                snakeSegments[i] = null;
            }
        }
    }

    private void Grow(Transform parentTransform)
    {
        snakeSegments[segmentCount++] = Instantiate(Resources.Load<GameObject>("SnakeBody"), parentTransform);
    }

}
