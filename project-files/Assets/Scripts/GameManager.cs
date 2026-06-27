using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public List<Transform> points = new List<Transform>();
    public List<GameObject> monsterPool = new List<GameObject>();

    public int maxMonsters = 10;
    public int targetKills = 5;

    public GameObject monster;

    public float createTime = 3.0f;

    public bool isGameOver;

    public bool IsGameOver {
        get {return isGameOver;}
        set {
            isGameOver = value;
            if (isGameOver) {
                EndGame("游戏结束\n玩家血量为 0\n关闭窗口退出");
            }
        }
    }

    public static GameManager instance = null;

    public TMP_Text scoreText;
    private int totScore = 0;
    private int killCount = 0;
    private GameObject gameOverPanel;
    private Text gameOverText;

    void Awake() {
        if (instance == null) {
            instance = this;
        }
        else if (instance != this) {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        Time.timeScale = 1.0f;
        LockCursorForGameplay();
        CreateMonsterPool();

        Transform spawnPointGroup = GameObject.Find("SpawnPointGroup")?.transform;

        foreach(Transform point in spawnPointGroup) {
            points.Add(point);
        }

        InvokeRepeating("CreateMonster", 2.0f, createTime);

        totScore = 0;
        PlayerPrefs.SetInt("TOT_SCORE", 0);
        PlayerPrefs.Save();
        DisplayScore(0);
        SetupGameOverPanel();
    }

    void CreateMonster() {
        if (isGameOver) {
            return;
        }

        int idx = Random.Range(0, points.Count);

        GameObject _monster = GetMonsterInPool();

        _monster?.transform.SetPositionAndRotation(points[idx].position, points[idx].rotation);
        _monster?.SetActive(true);
    }

    void CreateMonsterPool() {
        for(int i = 0; i < maxMonsters; i++) {
            var _monster = Instantiate<GameObject>(monster);

            _monster.name = $"Monster_{i:00}";

            _monster.SetActive(false);

            monsterPool.Add(_monster);
        }
    }

    public GameObject GetMonsterInPool() {
        foreach (var _monster in monsterPool) {
            if (_monster.activeSelf == false) {
                return _monster;
            }
        }
        return null;
    }

    public void DisplayScore(int score) {
        totScore += score;
        scoreText.text = $"<color=#00ff00>SCORE : </color> <color=#ff0000>{totScore:#,##0}</color>";
        PlayerPrefs.SetInt("TOT_SCORE", totScore);
    }

    public void RegisterMonsterKill() {
        if (isGameOver) {
            return;
        }

        killCount++;
        if (killCount >= targetKills) {
            EndGame("您已获胜！\n已击杀 5 个怪物\n关闭窗口退出");
        }
    }

    void EndGame(string message) {
        if (isGameOver && gameOverPanel != null && gameOverPanel.activeSelf) {
            return;
        }

        isGameOver = true;
        CancelInvoke("CreateMonster");
        UnlockCursorForUi();
        ShowGameOver(message);
        Time.timeScale = 0.0f;
    }

    void SetupGameOverPanel() {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null || gameOverPanel != null) {
            return;
        }

        gameOverPanel = new GameObject("Game Over Panel");
        gameOverPanel.transform.SetParent(canvas.transform, false);

        RectTransform rect = gameOverPanel.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image background = gameOverPanel.AddComponent<Image>();
        background.color = new Color(0.0f, 0.0f, 0.0f, 0.72f);

        GameObject textObject = new GameObject("Game Over Text");
        textObject.transform.SetParent(gameOverPanel.transform, false);

        RectTransform textRect = textObject.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        gameOverText = textObject.AddComponent<Text>();
        gameOverText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        gameOverText.alignment = TextAnchor.MiddleCenter;
        gameOverText.fontSize = 54;
        gameOverText.color = Color.white;
        gameOverText.text = "";

        gameOverPanel.SetActive(false);
    }

    void ShowGameOver(string message) {
        SetupGameOverPanel();

        if (gameOverText != null) {
            gameOverText.text = message;
        }

        if (gameOverPanel != null) {
            gameOverPanel.SetActive(true);
        }
    }

    void LockCursorForGameplay() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void UnlockCursorForUi() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
