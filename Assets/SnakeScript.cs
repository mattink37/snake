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
    private Boolean pendingSegment = false;

    public float moveInterval = 0.5f;

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

    private IEnumerator MoveSnake()
    {
        while (true)
        {
            var snakeyBits = GameObject.FindGameObjectsWithTag("SnakeSegment");
            Transform previousParentTransform = null;
            foreach (GameObject snakeyBit in snakeyBits)
            {
                if (previousParentTransform == null) {
                    previousParentTransform = snakeyBit.transform;
                    snakeyBit.GetComponent<Rigidbody2D>().position += moveVector * spriteRenderer.bounds.size.x;
                } else {
                    snakeyBit.transform.position = previousParentTransform.position;
                    previousParentTransform = snakeyBit.transform;
                }
            }
            if (pendingSegment) {
                Grow();
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
        Debug.Log("Collision");
        if (collider2D.CompareTag("Food"))
        {
            Destroy(GameObject.FindWithTag("Food"));
            pendingSegment = true;
        }
        else if (collider2D.CompareTag("SnakeSegment"))
        {
            Debug.Log("Collided with snake!");
        }
    }

    private void Grow() 
    {
        GameObject caboose = Instantiate(Resources.Load<GameObject>("SnakeBody"));
        caboose.transform.SetParent(this.transform, false);
        caboose.transform.SetSiblingIndex(0);
    }

}
