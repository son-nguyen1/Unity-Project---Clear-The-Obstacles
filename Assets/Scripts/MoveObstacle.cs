using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MoveObstacle : MonoBehaviour
{
    /// <summary>
    /// There're 2 key types of movement: Static or Launch, with 3 directions: Left, Right, Middle.
    /// Use rigidbody2D to avoid detection issues with physics.
    /// </summary>
    private enum MoveType
    {
        StaticDown,
        StaticLeft,
        StaticRight,

        LaunchDownLeft,
        LaunchDownRight,
        LaunchDownMiddle,
    }

    [SerializeField] private MoveType moveType;

    private GameManager gameManager;

    private Rigidbody2D obstacleRB2D;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float launchSpeed = 7.5f;

    [SerializeField] private float yLaunchPosition = 3.5f;

    private float aboveBoundary = 7.5f;
    private float belowBoundary = -20f;
    private float leftBoundary = -11f;
    private float rightBoundary = 11f;

    private bool hasCollided = false;
    private bool hasLaunched = false;

    /// <summary>
    /// At 1st, the objects move downward, unaffected by gravity. Once a collision happens, movement stops, affected by gravity.
    /// Launch objects with a physical force at a pre-determined position. Remove them after flying out-of-bounds in any direction.
    /// </summary>
    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        obstacleRB2D = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        obstacleRB2D.gravityScale = 0f;
    }

    private void Update()
    {
        DestroyObstacle();
    }

    private void FixedUpdate()
    {
        if (gameManager.isGameOver)
        {
            return;
        }

        if (!hasCollided)
        {
            switch (moveType)
            {
                case MoveType.StaticDown:
                    HandleStaticMovement(Vector2.down);
                    break;

                case MoveType.StaticLeft:
                    HandleStaticMovement(Vector2.left);
                    break;

                case MoveType.StaticRight:
                    HandleStaticMovement(Vector2.right);
                    break;

                case MoveType.LaunchDownLeft:
                    HandleLaunchMovement(Vector2.left);
                    break;

                case MoveType.LaunchDownRight:
                    HandleLaunchMovement(Vector2.right);                    
                    break;

                case MoveType.LaunchDownMiddle:
                    HandleLaunchMovement(Vector2.down);
                    break;
            }
        }
    }

    private void OnEnable()
    {
        GameManager.OnGameOverEvent += HandleOnGameOverEvent;
    }

    private void OnDisable()
    {
        GameManager.OnGameOverEvent -= HandleOnGameOverEvent;
    }

    /// <summary>
    /// As the 'Game Over' event is triggered, any form of movement is effectively stopped.
    /// Such as: static movement, gravitational movement, launch force movement.
    /// </summary>
    private void HandleOnGameOverEvent()
    {
        moveSpeed = 0f;
        obstacleRB2D.gravityScale = 0f;
        obstacleRB2D.velocity = Vector2.zero;
    }

    private void HandleStaticMovement(Vector2 direction)
    {
        Vector2 moveDirection = (obstacleRB2D.position + direction * moveSpeed * Time.fixedDeltaTime);
        obstacleRB2D.MovePosition(moveDirection);
    }

    private void HandleLaunchMovement(Vector2 direction)
    {
        // Avoid the objects being launched repeatedly
        if (!hasLaunched)
        {
            if (Mathf.Abs(transform.position.y - yLaunchPosition) > 0.1f)
            {
                HandleStaticMovement(Vector2.down);
            }
            else
            {
                hasLaunched = true;

                // Avoid launch code from being overriden by existing movement codes
                obstacleRB2D.velocity = Vector2.zero;

                obstacleRB2D.gravityScale = 1f;
                obstacleRB2D.AddForce(direction * launchSpeed, ForceMode2D.Impulse);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collider)
    {
        hasCollided = true;
        obstacleRB2D.gravityScale = 1f;
    }

    private void DestroyObstacle()
    {
        switch (moveType)
        {
            case MoveType.StaticDown:
            case MoveType.LaunchDownLeft:
            case MoveType.LaunchDownRight:
            case MoveType.LaunchDownMiddle:
                if (transform.position.y < belowBoundary || transform.position.x < leftBoundary || transform.position.x > rightBoundary)
                {
                    Destroy(gameObject);
                }
                break;

            case MoveType.StaticLeft:
                if (transform.position.y > aboveBoundary || transform.position.y < belowBoundary || transform.position.x < leftBoundary)
                {
                    Destroy(gameObject);
                }
                break;

            case MoveType.StaticRight:
                if (transform.position.y > aboveBoundary || transform.position.y < belowBoundary || transform.position.x > rightBoundary)
                {
                    Destroy(gameObject);
                }
                break;
        }
    }
}