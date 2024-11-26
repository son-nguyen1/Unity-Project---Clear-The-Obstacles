using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private LevelManager levelManager;
    private GameManager gameManager;

    private FadeObject fadeObject;

    // UI Objects
    [SerializeField] private GameObject gameTitle;
    [SerializeField] private GameObject menuButton;
    [SerializeField] private GameObject highScore;
    [SerializeField] private GameObject dailyRewardsUI;
    [SerializeField] private GameObject gemTracker;
    [SerializeField] private GameObject coinTracker;
    [SerializeField] private GameObject scoreTracker;
    [SerializeField] private GameObject levelTracker;
    [SerializeField] private GameObject gameRestart;
    [SerializeField] private GameObject deathTimer;
    [SerializeField] private GameObject restartButton;
    [SerializeField] private GameObject gameCongrats;

    // UI Texts
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI deathTimerText;

    // UI Visuals
    [SerializeField] private Image deathTimerVisual;

    // Coroutines
    private Coroutine scoreTrackCoroutine;
    private Coroutine deathTimerCoroutine;

    private int currentScore = 0;
    private int currentLevel;

    private float visibleAlpha = 1f;
    private float transparentAlpha = 0f;

    private float startDeathTime = 10;
    private float currentDeathTime;

    private bool isScoreTrackRunning = false;
    private bool isDeathTimerRunning = false;

    private void Awake()
    {
        levelManager = GetComponent<LevelManager>();
        gameManager = GetComponent<GameManager>();

        fadeObject = FindObjectOfType<FadeObject>();
    }

    /// <summary>
    /// On launched, the Title Screen appears, with: Game Title, Menu Button and High Score quickly fade in.
    /// Score Tracker and Level Tracker are transparent, not visible.
    /// </summary>
    private void Start()
    {
        StartCoroutine(EnableUIObjects(new List<GameObject> { gameTitle, menuButton, highScore, gemTracker, coinTracker }, true, false, transparentAlpha, visibleAlpha));
        LoadHighScore();
    }

    private void Update()
    {
        if (!gameManager.isGameFinish)
        {
            RunLevelTracker();
        }

        UpdateAndSaveHighScore();
    }

    private void OnEnable()
    {
        GameManager.OnStartButtonClickEvent += HandleOnStartButtonClickEvent;
        GameManager.OnBonusButtonClickEvent += HandleOnBonusButtonClickEvent;
        GameManager.OnCloseButtonClickEvent += HandleOnCloseButtonClickEvent;
        GameManager.OnEndlessButtonClickEvent += HandleOnEndlessButtonClickEvent;

        GameManager.OnGameOverEvent += HandleOnGameOverEvent;
        GameManager.OnContinueButtonClickEvent += HandleOnContinueButtonClickEvent;
    }    

    private void OnDisable()
    {
        GameManager.OnStartButtonClickEvent -= HandleOnStartButtonClickEvent;
        GameManager.OnBonusButtonClickEvent -= HandleOnBonusButtonClickEvent;
        GameManager.OnCloseButtonClickEvent -= HandleOnCloseButtonClickEvent;
        GameManager.OnEndlessButtonClickEvent -= HandleOnEndlessButtonClickEvent;

        GameManager.OnGameOverEvent -= HandleOnGameOverEvent;
        GameManager.OnContinueButtonClickEvent -= HandleOnContinueButtonClickEvent;
    }

    /// <summary>
    /// As the 'On Start Button Click' event is triggered, Game Title, Menu Button and High Score quickly fade out.
    /// The cursor disappears. Level Tracker quickly fade in, and start Climb mode.
    /// </summary>
    private void HandleOnStartButtonClickEvent()
    {
        Cursor.visible = false;
        StartCoroutine(EnableUIObjects(new List<GameObject> { gameTitle, menuButton, highScore, gemTracker, coinTracker }, false, true, visibleAlpha, transparentAlpha));
        StartCoroutine(EnableUIObjects(new List<GameObject> { levelTracker }, true, false, transparentAlpha, visibleAlpha));

        scoreTrackCoroutine = StartCoroutine(RunScoreTracker());
    }

    /// <summary>
    /// As the 'On Bonus Button Click' event is triggered, Bonus Panel quickly fades in.
    /// </summary>
    private void HandleOnBonusButtonClickEvent()
    {
        StartCoroutine(EnableUIObjects(new List<GameObject> { dailyRewardsUI }, true, false, transparentAlpha, visibleAlpha));
    }

    /// <summary>
    /// As the 'On Close Button Click' event is triggered, Bonus Panel quickly fades out.
    /// </summary>
    private void HandleOnCloseButtonClickEvent()
    {
        StartCoroutine(EnableUIObjects(new List<GameObject> { dailyRewardsUI }, false, true, visibleAlpha, transparentAlpha));
    }

    /// <summary>
    /// As the 'On Endless Button Click' event is triggered, Game Title, Menu Button and High Score quickly fade out.
    /// The cursor disappears. Score Tracker quickly fade in, and start Endless mode.
    /// </summary>
    private void HandleOnEndlessButtonClickEvent()
    {
        Cursor.visible = false;

        StartCoroutine(EnableUIObjects(new List<GameObject> { gameTitle, menuButton, highScore, gemTracker, coinTracker }, false, true, visibleAlpha, transparentAlpha));
        StartCoroutine(EnableUIObjects(new List<GameObject> { scoreTracker }, true, false, transparentAlpha, visibleAlpha));

        scoreTrackCoroutine = StartCoroutine(RunScoreTracker());
    }

    /// <summary>
    /// As the 'Game Over' event is triggered, Score Tracker and Level Tracker stop. The cursor appears.
    /// Game Restart, Death Timer, Restart Buttons appear. Timer starts. Reset the flag of the coroutine to be fully stopped, to be restarted later.
    /// </summary>
    private void HandleOnGameOverEvent()
    {
        Cursor.visible = true;
        StartCoroutine(EnableUIObjects(new List<GameObject> { gameRestart, deathTimer, restartButton }, true, false, transparentAlpha, visibleAlpha));

        isScoreTrackRunning = false;
        if (scoreTrackCoroutine != null) { StopCoroutine(scoreTrackCoroutine); }

        deathTimerCoroutine = StartCoroutine(RunDeathTimer());
    }

    /// <summary>
    /// As the 'On Continue Button Click' event is triggered, Score Tracker and Level Tracker restart. The cursor disappears.
    /// Timer stops. Game Restart, Death Timer, Restart Buttons disappear. Reset the flag of the coroutine to be fully stopped, tobe restarted later.
    /// </summary>
    private void HandleOnContinueButtonClickEvent()
    {
        Cursor.visible = false;
        StartCoroutine(EnableUIObjects(new List<GameObject> { gameRestart, deathTimer, restartButton }, false, true, visibleAlpha, transparentAlpha));

        isDeathTimerRunning = false;
        if (deathTimerCoroutine != null) { StopCoroutine(deathTimerCoroutine); }

        scoreTrackCoroutine = StartCoroutine(RunScoreTracker());
    }

    /// <summary>
    /// On called, Score Tracker and Level Tracker stop and disappear. Congrats and Final Score appear.
    /// Reset the flag of the coroutine to be fully stopped, and restarted later.
    /// </summary>
    public void HandleOnGameFinishEvent()
    {
        Cursor.visible = true;
        StartCoroutine(EnableUIObjects(new List<GameObject> { levelTracker }, false, true, visibleAlpha, transparentAlpha));
        StartCoroutine(EnableUIObjects(new List<GameObject> { gameCongrats }, true, false, transparentAlpha, visibleAlpha));

        isScoreTrackRunning = false;
        if (scoreTrackCoroutine != null) { StopCoroutine(scoreTrackCoroutine); }
    }

    /// <summary>
    /// Ensure objects fully finish their fade effect, then become active or inactive.
    /// Inactive objects instantly turn active, transparently, then gradually turn visible. And vice versa.
    /// </summary>
    private IEnumerator EnableUIObjects(List<GameObject> uiObjects, bool shouldBeActiveOnStart, bool shouldBeInactiveOnEnd, float startAlpha, float targetAlpha)
    {
        List<Coroutine> fadeCoroutines = new List<Coroutine>();

        foreach (var obj in uiObjects)
        {
            if (shouldBeActiveOnStart) obj.SetActive(true);

            fadeCoroutines.Add(StartCoroutine(fadeObject.HandleFadeEffect(obj, startAlpha, targetAlpha)));
        }

        foreach (var coroutine in fadeCoroutines)
        {
            yield return coroutine;
        }

        foreach (var obj in uiObjects)
        {
            if (shouldBeInactiveOnEnd)
            {
                obj.SetActive(false);
            }
        }
    }

    private IEnumerator RunScoreTracker()
    {
        if (isScoreTrackRunning)
            yield break; // Avoid the coroutine being triggered multiple times        

        isScoreTrackRunning = true; // Manually reset this flag, because while flag changes abruptly

        while (!gameManager.isGameOver)
        {
            scoreText.text = "Score: " + currentScore;
            currentScore++;
            yield return new WaitForSeconds(0.25f);
        }

        isScoreTrackRunning = false; // Reset the flag, for future restart
    }

    private IEnumerator RunDeathTimer()
    {
        if (isDeathTimerRunning)
            yield break; // Avoid the coroutine being triggered multiple times        

        isDeathTimerRunning = true; // Manually reset this flag, because while flag changes abruptly
        currentDeathTime = startDeathTime;

        while (gameManager.isGameOver && currentDeathTime > 0)
        {
            deathTimerText.text = Mathf.CeilToInt(currentDeathTime).ToString();
            deathTimerVisual.fillAmount = currentDeathTime / startDeathTime;

            yield return null;
            currentDeathTime -= Time.deltaTime;              
        }

        if (gameManager.isGameOver && currentDeathTime <= 0)
        {
            deathTimerText.text = "0";
            deathTimerVisual.fillAmount = 0;
            isDeathTimerRunning = false; // Reset the flag, for future restart

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void RunLevelTracker()
    {
        currentLevel = levelManager.levelConfigIndex + 1;
        levelText.text = "Level: " + currentLevel + "/10";
    }

    private void UpdateAndSaveHighScore()
    {
        int currentHighScore = PlayerPrefs.GetInt("HighScore", 0);

        if (currentScore > currentHighScore)
        {
            PlayerPrefs.SetInt("HighScore", currentScore);
            PlayerPrefs.Save();

            highScoreText.text = currentScore.ToString();
        }
        else
        {
            highScoreText.text = currentHighScore.ToString();
        }
    }

    private void LoadHighScore()
    {
        int currentHighScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreText.text = currentHighScore.ToString();
    }
}