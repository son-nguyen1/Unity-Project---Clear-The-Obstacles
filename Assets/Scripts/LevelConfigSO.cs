using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Level Configuration System / Levels Database")]
public class LevelConfigSO : ScriptableObject
{
    /// <summary>
    /// Each level varies by different waves, with varied obstacles, so each need to be configurated.
    /// Create a Scriptable Object for each level, and customize each level in the Unity Editor.
    /// </summary>

    public GameObject backgroundPrefab;
    public Vector2 backgroundPosition;

    public GameObject levelPrefab;
    public string levelDisplayText;

    public List<WaveConfig> waveConfig;

    [System.Serializable]
    public class WaveConfig
    {
        public List<GameObject> obstaclePrefab;
        public int obstacleCount;

        public Vector2 obstaclePosition;
        public Quaternion obstacleRotation;

        public float horizontalSpacing;
        public float verticalSpacing;
    }
}