using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    /// <summary>
    /// There're 2 types of object: World Object and UI Object, because of Transform and Rect Transform.
    /// </summary>
    private enum ObjectType
    {
        WorldObject,
        UIObject,
    }

    [SerializeField] private ObjectType objectType;

    private GameManager gameManager;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float uiSpeed = 300f;

    private float startMoveSpeed;
    private float startUISpeed;

    private float yObjectBoundary = -50f;

    /// <summary>
    /// The objects gradually move downward, so their functions are called in Fixed Update.
    /// Continuously check their position, to remove once they reach an out-of-bound point on the Y axis.
    /// </summary>
    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        startMoveSpeed = moveSpeed;
        startUISpeed = uiSpeed;
    }

    private void Update()
    {
        DestroyObject();
    }

    private void FixedUpdate()
    {
        if (gameManager.isGameOver)
        {
            return;
        }

        else
        {
            switch (objectType)
            {
                case ObjectType.WorldObject:
                    HandleWorldMovement();
                    break;

                case ObjectType.UIObject:
                    HandleUIMovement();
                    break;
            }
        }
    }

    private void OnEnable()
    {
        GameManager.OnGameOverEvent += HandleOnGameOverEvent;
        GameManager.OnContinueButtonClickEvent += HandleOnContinueButtonClickEvent;
    }

    private void OnDisable()
    {
        GameManager.OnGameOverEvent -= HandleOnGameOverEvent;
        GameManager.OnContinueButtonClickEvent -= HandleOnContinueButtonClickEvent;
    }

    /// <summary>
    /// As the 'Game Over' event is triggered, any form of movement is effectively stopped.
    /// </summary>
    private void HandleOnGameOverEvent()
    {
        moveSpeed = 0f;
        uiSpeed = 0f;
    }

    private void HandleOnContinueButtonClickEvent()
    {
        moveSpeed = startMoveSpeed;
        uiSpeed = startUISpeed;
    }

    private void HandleWorldMovement()
    {
        transform.Translate(Vector2.down * moveSpeed * Time.deltaTime);
    }

    /*private void HandleStarMovement()
    {
        transform.Translate(Vector2.down * moveSpeed * Time.deltaTime);

        if (transform.position.y <= 0f)
        {
            moveSpeed = 0f;
            enabled = false;
        }
    }*/

    private void HandleUIMovement()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.Translate(Vector2.down * uiSpeed * Time.deltaTime);
    }

    private void DestroyObject()
    {
        if (transform.position.y < yObjectBoundary)
        {
            Destroy(gameObject);
        }
    }
}