using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class MouseDistance : MonoBehaviour
{
    [DllImport("user32.dll")]
    private static extern bool SetCursorPos(int x, int y);

    private Camera m_Camera;

    private Vector3 playerWorldPosition;
    private Vector3 playerScreenPosition;

    private bool isMouseReturned = false;

    private void Start()
    {
        m_Camera = Camera.main;
    }

    private void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 10f;

        Vector3 mousePositionNormalized = new Vector3(
            mousePosition.x / Screen.width, 
            mousePosition.y / Screen.height, 
            0f);

        Vector3 mouseWorldPosition = m_Camera.ScreenToWorldPoint(new Vector3
            (mousePositionNormalized.x * Screen.width,
             mousePositionNormalized.y * Screen.height, 
             0f));

        Vector2 rayOrigin = new Vector2(mouseWorldPosition.x, mouseWorldPosition.y);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.zero, 10f);

        if (hit.collider != null && hit.collider.GetComponent<MoveObject>())
        {
            if (!isMouseReturned)
            {
                isMouseReturned = true;
                ReturnMouseToPlayerPosition();
            }
        }
        else
        {
            isMouseReturned = false;
        }
    }

    /// <summary>
    /// The player is in the scene, while the mouse is on the screen. Both uses different metrics for position, float and int.
    /// 1st, convert where the player is, in the scene => on the screen. Then, match where the cursor is to the player, when it moves further away.
    /// </summary>
    private void ReturnMouseToPlayerPosition()
    {
        playerWorldPosition = transform.position;
        playerScreenPosition = m_Camera.WorldToScreenPoint(playerWorldPosition);

        float distanceMouseFromPlayer = Vector2.Distance(new Vector2(Input.mousePosition.x, Input.mousePosition.y), new Vector2(playerScreenPosition.x, playerScreenPosition.y));
        
        SetCursorPos((int)playerScreenPosition.x, Screen.height - (int)playerScreenPosition.y);        
        isMouseReturned = false;        
    }
}