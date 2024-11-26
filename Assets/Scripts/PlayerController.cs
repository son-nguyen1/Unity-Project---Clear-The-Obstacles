using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    private GameManager gameManager;

    private Rigidbody2D playerRB2D;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        playerRB2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!gameManager.isGameOver || !gameManager.isGameFinish)
        {
            HandleMovement();
        }
    }

    private void HandleMovement()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
        playerRB2D.MovePosition(mousePos);
    }
}