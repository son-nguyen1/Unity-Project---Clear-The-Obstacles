using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScaler : MonoBehaviour
{
    [SerializeField] private float targetAspectWidth = 16f;
    [SerializeField] private float targetAspectHeight = 9f;

    [SerializeField] private float defaultOrthographicSize = 5f;

    private Camera m_Camera;

    private void Start()
    {
        m_Camera = Camera.main;

        UpdateCameraScale();
    }

    private void Update()
    {
        UpdateCameraScale();
    }

    /// <summary>
    /// Adjusts the camera size. If the screen is taller, it zooms out; if the screen is wider, it zooms in slightly.
    /// </summary>
    private void UpdateCameraScale()
    {
        float screenRatio = (float) Screen.width / Screen.height;
        float targetRatio = targetAspectWidth / targetAspectHeight;

        m_Camera.orthographicSize = defaultOrthographicSize * Mathf.Max(1f, screenRatio / targetRatio);
    }
}