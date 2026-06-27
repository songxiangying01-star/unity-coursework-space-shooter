using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Button startButton;
    public Button optionButton;
    public Button shopButton;

    private UnityAction action;
    private Button musicButton;
    private AudioSource bgmSource;
    private GameObject optionPanel;
    private const string MusicEnabledKey = "MUSIC_ENABLED";

    void Start() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SetupBackgroundMusic();

        action = () => OnStartClick();
        startButton.onClick.AddListener(action);

        optionButton.onClick.AddListener(ToggleOptionPanel);

        SetupOptionPanel();
        SetupMusicButton();
    }

    public void OnButtonClick(string msg) {
        Debug.Log($"Click Button : {msg}");
    }

    void OnStartClick() {
        SceneManager.LoadScene("Level_01");
        SceneManager.LoadScene("Play", LoadSceneMode.Additive);
    }

    void SetupMusicButton() {
        if (optionButton == null || shopButton == null) {
            return;
        }

        musicButton = Instantiate(shopButton, shopButton.transform.parent);
        musicButton.name = "Button - Music";

        musicButton.onClick.RemoveAllListeners();
        musicButton.onClick.AddListener(ToggleBackgroundMusic);
        UpdateMusicButtonText();

        shopButton.gameObject.SetActive(false);
    }

    void SetupOptionPanel() {
        if (optionButton == null) {
            return;
        }

        Transform parent = optionButton.transform.parent;
        optionPanel = new GameObject("Option Panel");
        optionPanel.transform.SetParent(parent, false);

        RectTransform rect = optionPanel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0.0f, -190.0f);
        rect.sizeDelta = new Vector2(500.0f, 150.0f);

        Image panelImage = optionPanel.AddComponent<Image>();
        panelImage.color = new Color(0.0f, 0.08f, 0.12f, 0.78f);

        GameObject textObject = new GameObject("Option Text");
        textObject.transform.SetParent(optionPanel.transform, false);

        RectTransform textRect = textObject.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(18.0f, 12.0f);
        textRect.offsetMax = new Vector2(-18.0f, -12.0f);

        Text text = textObject.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 22;
        text.alignment = TextAnchor.MiddleLeft;
        text.color = new Color(0.78f, 0.96f, 1.0f, 1.0f);
        text.text = "Controls\nWASD: Move\nMouse: Turn / Aim\nLeft Click: Shoot\nGoal: Survive and defeat monsters";

        optionPanel.SetActive(false);
    }

    void SetupBackgroundMusic() {
        GameObject musicObject = GameObject.Find("BackgroundMusic");
        if (musicObject == null) {
            musicObject = new GameObject("BackgroundMusic");
            DontDestroyOnLoad(musicObject);
        }

        bgmSource = musicObject.GetComponent<AudioSource>();
        if (bgmSource == null) {
            bgmSource = musicObject.AddComponent<AudioSource>();
        }

        bgmSource.clip = CreateBackgroundMusicClip();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        bgmSource.volume = 0.35f;

        bool musicEnabled = PlayerPrefs.GetInt(MusicEnabledKey, 1) == 1;
        if (musicEnabled && !bgmSource.isPlaying) {
            bgmSource.Play();
        }
    }

    void ToggleBackgroundMusic() {
        bool musicEnabled = PlayerPrefs.GetInt(MusicEnabledKey, 1) == 1;
        musicEnabled = !musicEnabled;
        PlayerPrefs.SetInt(MusicEnabledKey, musicEnabled ? 1 : 0);
        PlayerPrefs.Save();

        if (musicEnabled) {
            bgmSource.Play();
        }
        else {
            bgmSource.Pause();
        }

        UpdateMusicButtonText();
        OnButtonClick(musicButton.name);
    }

    void ToggleOptionPanel() {
        if (optionPanel == null) {
            return;
        }

        optionPanel.SetActive(!optionPanel.activeSelf);
        OnButtonClick(optionButton.name);
    }

    void UpdateMusicButtonText() {
        if (musicButton == null) {
            return;
        }

        Text buttonText = musicButton.GetComponentInChildren<Text>();
        if (buttonText == null) {
            return;
        }

        bool musicEnabled = PlayerPrefs.GetInt(MusicEnabledKey, 1) == 1;
        buttonText.text = musicEnabled ? "MUSIC ON" : "MUSIC OFF";
    }

    AudioClip CreateBackgroundMusicClip() {
        const int sampleRate = 44100;
        const float duration = 8.0f;
        int sampleCount = Mathf.RoundToInt(sampleRate * duration);
        float[] samples = new float[sampleCount];
        float[] notes = { 220.0f, 261.63f, 329.63f, 392.0f };

        for (int i = 0; i < sampleCount; i++) {
            float t = i / (float)sampleRate;
            float note = notes[Mathf.FloorToInt(t * 2.0f) % notes.Length];
            float wave = Mathf.Sin(2.0f * Mathf.PI * note * t);
            float harmony = Mathf.Sin(2.0f * Mathf.PI * note * 1.5f * t) * 0.35f;
            float envelope = Mathf.Sin(Mathf.PI * (i / (float)sampleCount));
            samples[i] = (wave + harmony) * envelope * 0.08f;
        }

        AudioClip clip = AudioClip.Create("Generated Space BGM", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }
}
