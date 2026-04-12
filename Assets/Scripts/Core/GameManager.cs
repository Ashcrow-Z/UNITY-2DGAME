using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { MainMenu, Playing, Paused, GameOver, Victory }

    public GameState CurrentState { get; private set; } = GameState.MainMenu;
    public int CurrentLevel { get; private set; } = 1;
    public int Score { get; private set; }
    public int Lives { get; private set; }
    public float ElapsedTime { get; private set; }

    public event Action<GameState> OnGameStateChanged;
    public event Action<int> OnScoreChanged;
    public event Action<int> OnLivesChanged;

    [Header("Game Settings")]
    public int startingLives = 3;

    private bool timerRunning;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartGame()
    {
        CurrentLevel = 1;
        Score = 0;
        Lives = startingLives;
        ElapsedTime = 0f;
        timerRunning = false;
        OnScoreChanged?.Invoke(Score);
        OnLivesChanged?.Invoke(Lives);
        SceneManager.LoadScene("GameLevel");
    }

    public void BeginPlaying()
    {
        SetState(GameState.Playing);
        Time.timeScale = 1f;
        timerRunning = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space))
        {
            if (CurrentState == GameState.Playing || CurrentState == GameState.Paused)
                TogglePause();
        }

        if (timerRunning && CurrentState == GameState.Playing)
        {
            ElapsedTime += Time.deltaTime;
        }
    }

    public void TogglePause()
    {
        if (CurrentState == GameState.Playing)
        {
            Time.timeScale = 0f;
            SetState(GameState.Paused);
        }
        else if (CurrentState == GameState.Paused)
        {
            Time.timeScale = 1f;
            SetState(GameState.Playing);
        }
    }

    public void AddScore(int points)
    {
        Score += points;
        OnScoreChanged?.Invoke(Score);
    }

    public void LoseLife()
    {
        Lives--;
        OnLivesChanged?.Invoke(Lives);
        if (Lives <= 0)
        {
            TriggerGameOver();
        }
    }

    public void OnAllEnemiesDefeated()
    {
        if (CurrentLevel == 1)
        {
            CurrentLevel = 2;
            timerRunning = false;
            SceneManager.LoadScene("GameLevel");
        }
        else
        {
            TriggerVictory();
        }
    }

    public void TriggerGameOver()
    {
        timerRunning = false;
        Time.timeScale = 0f;
        SetState(GameState.GameOver);
    }

    public void TriggerVictory()
    {
        timerRunning = false;
        Time.timeScale = 0f;
        SetState(GameState.Victory);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        Lives = startingLives;
        Score = 0;
        ElapsedTime = 0f;
        CurrentLevel = 1;
        OnScoreChanged?.Invoke(Score);
        OnLivesChanged?.Invoke(Lives);
        SceneManager.LoadScene("GameLevel");
    }

    public void RestartCurrentLevel()
    {
        Time.timeScale = 1f;
        Lives = startingLives;
        ElapsedTime = 0f;
        OnLivesChanged?.Invoke(Lives);
        SceneManager.LoadScene("GameLevel");
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SetState(GameState.MainMenu);
        SceneManager.LoadScene("MainMenu");
    }

    void SetState(GameState newState)
    {
        CurrentState = newState;
        OnGameStateChanged?.Invoke(newState);
    }
}
