using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static event System.Action OnStartButtonClickEvent;
    public static event System.Action OnSoundButtonClickEvent;
    public static event System.Action OnBonusButtonClickEvent;
    public static event System.Action OnClaimButtonClickEvent;
    public static event System.Action OnCloseButtonClickEvent;
    public static event System.Action OnEndlessButtonClickEvent;
    public static event System.Action OnContinueButtonClickEvent;

    public static event Action OnGameOverEvent;

    private LevelManager levelManager;
    private UIManager uiManager;

    // Core Elements
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject lanternPrefab;

    private GameObject playerInstance;
    private GameObject lanternInstance;

    // Background
    [SerializeField] private GameObject introBackground;
    [SerializeField] private GameObject mountainBackground;
    [SerializeField] private GameObject spaceBackground;

    private GameObject spaceInstance;

    // Buttons
    [SerializeField] private Button startButton;
    [SerializeField] private Button soundButton;
    [SerializeField] private Button rewardsButton;
    [SerializeField] private Button claimButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button endlessButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button rejectButton;
    [SerializeField] private Button returnButton;

    // Postions
    private Vector2 playerPosition = Vector2.zero;
    private Vector2 lanternPosition = new Vector2(0f, -3.4f);
    private Vector2 spacePosition = new Vector2(0f, 13f);

    private float ySpaceBoundary = 0f;

    [HideInInspector]
    public bool isGameOver = false;

    [HideInInspector]
    public bool isGameFinish = false;

    private bool isCongratsOn = false;

    private void Awake()
    {
        levelManager = GetComponent<LevelManager>();
        uiManager = GetComponent<UIManager>();
    }

    /// <summary>
    /// At launch, background already exists  for performance, but doesn't move yet.
    /// Each button has a listener that run its own execution and trigger an event, when clicked.
    /// </summary>
    private void Start()
    {
        EnableBackgroundMovement(false);

        startButton.onClick.AddListener(OnStartButtonClick);
        soundButton.onClick.AddListener(OnSoundButtonClick);
        rewardsButton.onClick.AddListener(OnBonusButtonClick);
        claimButton.onClick.AddListener(OnClaimButtonClick);
        closeButton.onClick.AddListener(OnCloseButtonClick);
        endlessButton.onClick.AddListener(OnEndlessButtonClick);
        continueButton.onClick.AddListener(OnContinueButtonClick);
        rejectButton.onClick.AddListener(OnRejectButtonClick);
        returnButton.onClick.AddListener(OnReturnHomeButtonClick);
    }

    private void Update()
    {
        CheckAndStopSpaceBackground(false);
    }

    /// <summary>
    /// On clicked, disable the button to prevent multiple clicks. Trigger the event for subscribers.
    /// Background moves down. Start the 1st level, with these Gameplay Elements: Player and Lantern.
    /// </summary>
    private void OnStartButtonClick()
    {
        EnableBackgroundMovement(true);
        startButton.interactable = false;

        SpawnCoreElements();

        levelManager.SetGameMode(LevelManager.GameMode.Climb);
        levelManager.StartTheFirstLevel();

        OnStartButtonClickEvent?.Invoke();
    }

    /// <summary>
    /// On clicked, disable the button to prevent multiple clicks. Trigger the event for subscribers.
    /// </summary>
    private void OnSoundButtonClick()
    {
        OnSoundButtonClickEvent?.Invoke();
    }

    //// <summary>
    /// On clicked, disable the button to prevent multiple clicks. Trigger the event for subscribers.
    /// </summary>
    private void OnEndlessButtonClick()
    {
        EnableBackgroundMovement(true);
        endlessButton.interactable = false;

        SpawnCoreElements();

        levelManager.SetGameMode(LevelManager.GameMode.Endless);
        levelManager.StartTheFirstLevel();

        OnEndlessButtonClickEvent?.Invoke();
    }

    /// <summary>
    /// On clicked, disable the button to prevent multiple clicks.
    /// Open the Daily Rewards panel. Trigger the event for subscribers.
    /// </summary>
    private void OnBonusButtonClick()
    {
        rewardsButton.interactable = false;
        claimButton.interactable = true;
        closeButton.interactable = true;

        OnBonusButtonClickEvent?.Invoke();
    }

    /// <summary>
    /// On clicked, disable the button to prevent multiple clicks. Trigger the event for subscribers.
    /// </summary>
    private void OnClaimButtonClick()
    {
        OnClaimButtonClickEvent?.Invoke();
    }

    private void OnCloseButtonClick()
    {
        rewardsButton.interactable = true;
        closeButton.interactable = false;

        OnCloseButtonClickEvent?.Invoke();
    }

    /// <summary>
    /// On game over, enable the button again. The 'game over' flag becomes true to stop movements and gameplay.
    /// Player and Lantern disappear. Trigger the event for subscribers.
    /// </summary>
    public void OnGameOver()
    {
        isGameOver = true;
        continueButton.interactable = true;

        EnableCoreElements(false);

        OnGameOverEvent?.Invoke();
    }

    /// <summary>
    /// On clicked, disable the button again. The 'game over' flag becomes false to resume movements and gameplay.
    /// Player and Lantern appear. Trigger the event for subscribers.
    /// </summary>
    private void OnContinueButtonClick()
    {
        isGameOver = false;
        isGameFinish = false;
        continueButton.interactable = false;

        EnableCoreElements(true);

        OnContinueButtonClickEvent?.Invoke();
    }

    /// <summary>
    /// On clicked, the 'game finish' flag becomes true. Level and score tracker stop.
    /// Stop any new levels. Space background appears and moves down.
    /// </summary>
    public void OnGameFinish()
    {
        isGameFinish = true;
        returnButton.interactable = true;

        EnableSpaceBackground();
    }

    private void OnRejectButtonClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnReturnHomeButtonClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void SpawnCoreElements()
    {
        playerInstance = Instantiate(playerPrefab, playerPosition, Quaternion.identity);
        lanternInstance = Instantiate(lanternPrefab, lanternPosition, Quaternion.identity);
    }

    private void EnableCoreElements(bool isEnabled)
    {
        playerInstance.gameObject.SetActive(isEnabled);
        playerInstance.transform.position = playerPosition;

        lanternInstance.gameObject.SetActive(isEnabled);
        lanternInstance.transform.position = lanternPosition;
    }

    private void EnableBackgroundMovement(bool isEnabled)
    {
        introBackground.GetComponent<MoveObject>().enabled = isEnabled;
        mountainBackground.GetComponent<MoveObject>().enabled = isEnabled;
    }

    private void EnableSpaceBackground()
    {
        spaceInstance = Instantiate(spaceBackground, spacePosition, Quaternion.identity);
    }

    private void CheckAndStopSpaceBackground(bool isEnabled)
    {
        if (spaceInstance != null && spaceInstance.transform.position.y <= ySpaceBoundary)
        {
            spaceInstance.GetComponent<MoveObject>().enabled = isEnabled;

            playerInstance.gameObject.SetActive(false);

            if (uiManager != null && !isCongratsOn)
            {
                uiManager.HandleOnGameFinishEvent();
                isCongratsOn = true;
            }            
        }
    }

    public void DestroySpaceBackgroundAfterGameOver()
    {
        if (spaceInstance != null)
        {
            Destroy(spaceInstance);
        }
    }
}