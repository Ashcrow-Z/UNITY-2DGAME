using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class HUDController : MonoBehaviour
{
    [Header("Text Elements")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI enemyCountText;
    public TextMeshProUGUI levelText;

    [Header("Health Display")]
    public Transform heartsContainer;
    public GameObject heartPrefab;

    [Header("Stamina Bar")]
    public RectTransform staminaBarFillRT;
    public Image staminaBarFillImage;

    private List<GameObject> heartIcons = new List<GameObject>();
    private PlayerController cachedPlayerController;

    void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChanged += UpdateScore;
            GameManager.Instance.OnLivesChanged += UpdateLives;
            UpdateScore(GameManager.Instance.Score);
            UpdateLives(GameManager.Instance.Lives);
        }

        PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += UpdateHearts;
            UpdateHearts(playerHealth.CurrentHealth);
        }
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChanged -= UpdateScore;
            GameManager.Instance.OnLivesChanged -= UpdateLives;
        }
    }

    void Update()
    {
        if (GameManager.Instance != null)
        {
            UpdateTimer(GameManager.Instance.ElapsedTime);
            UpdateLevel(GameManager.Instance.CurrentLevel);
        }

        if (LevelManager.Instance != null)
        {
            UpdateEnemyCount();
        }

        UpdateStaminaBar();
    }

    void UpdateScore(int score)
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    void UpdateTimer(float elapsed)
    {
        if (timerText != null)
        {
            int minutes = (int)(elapsed / 60f);
            int seconds = (int)(elapsed % 60f);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    void UpdateLives(int lives)
    {
        if (livesText != null)
            livesText.text = "Lives: " + lives;
    }

    void UpdateHearts(int currentHealth)
    {
        foreach (var heart in heartIcons)
        {
            if (heart != null) Destroy(heart);
        }
        heartIcons.Clear();

        if (heartsContainer == null || heartPrefab == null) return;

        for (int i = 0; i < currentHealth; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartsContainer);
            heartIcons.Add(heart);
        }
    }

    void UpdateLevel(int level)
    {
        if (levelText != null)
            levelText.text = "Level " + level;
    }

    void UpdateStaminaBar()
    {
        if (staminaBarFillRT == null) return;

        if (cachedPlayerController == null)
            cachedPlayerController = FindObjectOfType<PlayerController>();
        if (cachedPlayerController == null) return;

        float pct = cachedPlayerController.StaminaPercent;

        staminaBarFillRT.anchorMax = new Vector2(pct, 1f);

        if (staminaBarFillImage != null)
        {
            staminaBarFillImage.color = pct > 0.5f
                ? Color.Lerp(new Color(1f, 0.85f, 0.2f), new Color(0.2f, 0.9f, 0.4f), (pct - 0.5f) * 2f)
                : Color.Lerp(new Color(0.9f, 0.25f, 0.2f), new Color(1f, 0.85f, 0.2f), pct * 2f);
        }
    }

    void UpdateEnemyCount()
    {
        if (enemyCountText != null)
        {
            int defeated = LevelManager.Instance.EnemiesDefeated;
            int total = LevelManager.Instance.TotalEnemiesForLevel;
            enemyCountText.text = "Enemies: " + defeated + "/" + total;
        }
    }
}
