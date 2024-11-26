using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private Image soundOn;
    [SerializeField] private Image soundOff;

    private AudioSource audioSource;

    private const string Current_Volume = "CurrentVolume";

    private bool isSoundOn = true;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        // Key initialization, use defaults if they don't exist
        isSoundOn = PlayerPrefs.GetInt(Current_Volume, 1) == 1; // Flag is true when value is 1

        UpdateCurrentVolume();
    }

    private void Update()
    {
        HandleSoundButtonVisual();
    }

    private void OnEnable()
    {
        GameManager.OnSoundButtonClickEvent += HandleOnSoundButtonClickEvent;
    }

    private void OnDisable()
    {
        GameManager.OnSoundButtonClickEvent -= HandleOnSoundButtonClickEvent;
    }

    /// <summary>
    /// As the 'On Sound Button Click' event is triggered, the flag turns opposite.
    /// The volume changes and is saved between sessions.
    /// </summary>
    private void HandleOnSoundButtonClickEvent()
    {
        isSoundOn = !isSoundOn;
        UpdateCurrentVolume();

        PlayerPrefs.SetInt(Current_Volume, isSoundOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void UpdateCurrentVolume()
    {
        audioSource.volume = isSoundOn ? 1 : 0;
    }

    private void HandleSoundButtonVisual()
    {
        soundOn.gameObject.SetActive(isSoundOn);
        soundOff.gameObject.SetActive(!isSoundOn);
    }
}