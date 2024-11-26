using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RotateObstacle : MonoBehaviour
{
    /// <summary>
    /// There're 2 key types of rotation: Self-Rotation or Orbital Rotation.
    /// Use rigidbody2D to avoid detection issues with physics.
    /// </summary>
    private enum RotateType
    {
        SelfRotate,
        OrbitRotate,
    }

    [SerializeField] private RotateType rotateType;

    private GameManager gameManager;

    private Rigidbody2D obstacleRB2D;

    // Rotation
    [SerializeField] private float rotateSpeed = 750f;

    [SerializeField] private GameObject centralObject;

    private float distanceFromCentral;
    private float startRotateAngle;
    private float currentRotateAngle;

    private float yBoundary = -20f;

    private bool hasCollided = false;

    /// <summary>
    /// Self-rotating objects is applied an angular velocity in degrees per second, rotating by themselves.
    /// Orbiting objects calculate 3 elements: initial distance from the central object, in what direction and what angle.
    /// These objects move along a circular path, around a central object, at a pre-determined distance.
    /// </summary>
    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        obstacleRB2D = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        switch (rotateType)
        {
            case RotateType.SelfRotate:
                obstacleRB2D.angularVelocity = rotateSpeed;
                break;
            
            case RotateType.OrbitRotate:
                distanceFromCentral = Vector2.Distance(transform.position, centralObject.transform.position);
                Vector2 direction = (Vector2)transform.position - (Vector2)centralObject.transform.position;
                startRotateAngle = Mathf.Atan2(direction.x, direction.y);
                break;
        }
    }

    private void Update()
    {
        switch (rotateType)
        {
            case RotateType.OrbitRotate:
                DestroyOrbitalObstacle();
                break;
        }
    }

    private void FixedUpdate()
    {
        if (gameManager.isGameOver)
        {
            return;
        }

        if (!hasCollided)
        {
            switch (rotateType)
            {
                case RotateType.OrbitRotate:
                    HandleOrbitRotation();
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
    /// As the 'Game Over' event is triggered, any form of rotation is effectively stopped.
    /// Such as: self-rotation and orbital rotation.
    /// </summary>
    private void HandleOnGameOverEvent()
    {
        obstacleRB2D.angularVelocity = 0f;
        rotateSpeed = 0f;
    }

    private void HandleOrbitRotation()
    {
        // Increment the angle on the circular path (loop around 360 degrees, 2 PI)
        currentRotateAngle = currentRotateAngle + rotateSpeed * Time.fixedDeltaTime;

        // Calculate where the object is on the circular path
        float xPos = Mathf.Cos(currentRotateAngle + startRotateAngle) * distanceFromCentral; // How far along the X axis
        float yPos = Mathf.Sin(currentRotateAngle + startRotateAngle) * distanceFromCentral; // How far along the Y axis
        Vector2 targetPosition = (Vector2)centralObject.transform.position + new Vector2(xPos, yPos); // Next position on the circular path

        // Moves along the circular path
        obstacleRB2D.MovePosition(targetPosition);
    }

    private void OnCollisionEnter2D(Collision2D collider)
    {
        switch (rotateType)
        {
            case RotateType.SelfRotate:
                obstacleRB2D.angularVelocity = 0f;
                break;

            case RotateType.OrbitRotate:
                hasCollided = true;
                break;
        }
    }

    private void DestroyOrbitalObstacle()
    {
        if (transform.position.y < yBoundary)
        {
            Destroy(gameObject);
            return;
        }
    }
}