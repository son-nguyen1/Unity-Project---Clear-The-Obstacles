using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public enum GameMode { Climb, Endless };
    public GameMode currentGameMode;

    private GameManager gameManager;

    [SerializeField] private Canvas uiCanvas;

    [SerializeField] private List<LevelConfigSO> levelConfigSO;
    public int levelConfigIndex = 0; // 0: Level 1; 1: Level 2; 2: Level 3

    [SerializeField] private GameObject introPrefab;
    [SerializeField] private GameObject cloudPrefab;

    private Vector2 introPosition = new Vector2(0f, -1f);
    private Vector2 cloudPosition = new Vector2(0f, 7f);

    private GameObject cloudInstance;
    private GameObject backgroundInstance;
    private GameObject levelDisplayTextInstance;
    private List<GameObject> obstacleWaveList = new List<GameObject>();

    private float yBackgroundBoundary = -28f;

    private int lastLevelIndex = -1;

    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
    }

    private void Update()
    {
        if (!gameManager.isGameFinish)
        {
            switch (currentGameMode)
            {
                case GameMode.Climb:
                    HandleClimbMode();
                    break;
                
                case GameMode.Endless:
                    HandleEndlessMode();
                    break;
            }
        }
    }

    public void SetGameMode(GameMode gameMode)
    {
        currentGameMode = gameMode;
    }

    private void OnEnable()
    {
        GameManager.OnContinueButtonClickEvent += HandleOnContinueButtonClickEvent;
    }

    private void OnDisable()
    {
        GameManager.OnContinueButtonClickEvent -= HandleOnContinueButtonClickEvent;
    }

    /// <summary>
    /// As the 'On Continue Button Click' event is triggered.
    /// In Climb mode, restart from the last level. In Endless mode, restart from a random level.
    /// </summary>
    private void HandleOnContinueButtonClickEvent()
    {
        DestroyCurrentLevel();

        switch (currentGameMode)
        {
            case GameMode.Climb:
                RestartFromLastLevel();
                break;

            case GameMode.Endless:
                int randomLevelIndex = GetRandomLevelIndex();
                SpawnTheLevel(randomLevelIndex);
                break;
        }

        SpawnIntroBackground();
    }

    /// <summary>
    /// Start at level 1 as current level. Each level has Cloud, Background, Level Text, and Obstacle Waves, packaged in Scriptable Objects.
    /// Next level appears once the background reaches a pre-determined position, close to the end of the current level.
    /// </summary>
    public void StartTheFirstLevel()
    {
        levelConfigIndex = 0;
        SpawnTheLevel(levelConfigIndex);
    }

    /// <summary>
    /// In Climb mode, the game will run until player beat the last level, so each levels appear 1-by-1.
    /// </summary>
    private void HandleClimbMode()
    {
        if (backgroundInstance != null && backgroundInstance.transform.position.y < yBackgroundBoundary)
        {
            if (levelConfigIndex < levelConfigSO.Count)
            {
                levelConfigIndex++;
                SpawnTheLevel(levelConfigIndex);
            }
            else
            {
                gameManager.OnGameFinish(); // Flag becomes true, prevent any new levels
            }
        }
    }

    /// <summary>
    /// All elements of current failed level are discarded. Fresh, new Intro and Elements from last level appear.
    /// </summary>
    private void RestartFromLastLevel()
    {
        if (levelConfigIndex == 0)
        {
            levelConfigIndex = 0;
            SpawnTheLevel(levelConfigIndex);
        }
        else
        {
            levelConfigIndex--;
            SpawnTheLevel(levelConfigIndex);
        }
    }

    /// <summary>
    /// In Endless mode, the game will keep running until player loses, so levels appear randomly.
    /// </summary>
    private void HandleEndlessMode()
    {
        if (backgroundInstance != null && backgroundInstance.transform.position.y < yBackgroundBoundary)
        {
            int randomLevelIndex = GetRandomLevelIndex();
            SpawnTheLevel(randomLevelIndex);            
        }
    }

    /// <summary>
    /// Next level won't be similar to the last level. Keep randomizing until new index is different from last one.
    /// That index, then become the last index to prepare for next randomize.
    /// </summary>
    /// <returns></returns>
    private int GetRandomLevelIndex()
    {
        int newLevelIndex;

        do
        {
            newLevelIndex = Random.Range(0, levelConfigSO.Count);
        }
        
        while (newLevelIndex == lastLevelIndex);

        lastLevelIndex = newLevelIndex;
        return newLevelIndex;
    }

    private void SpawnTheLevel(int levelIndex)
    {
        if (levelIndex >= levelConfigSO.Count)
        {
            Debug.LogWarning("No More Levels");
            return;
        }

        SpawnLevelElements(levelIndex);
    }

    /// <summary>
    /// Each level package has Cloud, Background, Level Text, and Wave Config, which consist of many obstacles.
    /// The cloud helps create an upward movement illusion. Loop through each Wave Config, spawn the 1st obstacle.
    /// </summary>
    private void SpawnLevelElements(int levelIndex)
    {
        SpawnCloudAndBackground(levelIndex);
        SpawnLevelDisplayText(levelIndex);
        SpawnObstacleWave(levelIndex);
    }

    private void SpawnCloudAndBackground(int levelIndex)
    {
        if (cloudPrefab != null)
        {
            cloudInstance = Instantiate(cloudPrefab, cloudPosition, Quaternion.identity);
        }

        backgroundInstance = Instantiate(levelConfigSO[levelIndex].backgroundPrefab, levelConfigSO[levelIndex].backgroundPosition, Quaternion.identity);
    }

    private void SpawnLevelDisplayText(int levelIndex)
    {
        levelDisplayTextInstance = Instantiate(levelConfigSO[levelIndex].levelPrefab, uiCanvas.transform);
        TextMeshProUGUI textOnCurrentLevel = levelDisplayTextInstance.GetComponent<TextMeshProUGUI>();

        textOnCurrentLevel.text = levelConfigSO[levelIndex].levelDisplayText;
    }

    private void SpawnObstacleWave(int levelIndex)
    {
        foreach (var wave in levelConfigSO[levelIndex].waveConfig)
        {
            for (int i = 0; i < wave.obstacleCount; i++)
            {
                Vector2 waveSpawnPosition = wave.obstaclePosition + new Vector2(i * wave.horizontalSpacing, i * wave.verticalSpacing);
                GameObject obstacleWaves = Instantiate(wave.obstaclePrefab[0], waveSpawnPosition, wave.obstacleRotation);

                obstacleWaveList.Add(obstacleWaves);
            }
        }
    }

    /// <summary>
    /// This part is for when clicking the continue button, after game over.
    /// </summary>
    private void SpawnIntroBackground()
    {
        Instantiate(introPrefab, introPosition, Quaternion.identity);
    }

    private void DestroyCurrentLevel()
    {
        Destroy(cloudInstance);
        Destroy(backgroundInstance);
        Destroy(levelDisplayTextInstance);

        foreach (GameObject obstacles in obstacleWaveList)
        {
            if (obstacles != null)
            {
                Destroy(obstacles);
            }
        }

        obstacleWaveList.Clear();

        gameManager.DestroySpaceBackgroundAfterGameOver();
    }
}