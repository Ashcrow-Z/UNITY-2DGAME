using UnityEngine;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    public Button resumeButton;
    public Button restartButton;
    public Button homeButton;

    void OnEnable()
    {
        if (resumeButton != null) resumeButton.onClick.AddListener(OnResumeClicked);
        if (restartButton != null) restartButton.onClick.AddListener(OnRestartClicked);
        if (homeButton != null) homeButton.onClick.AddListener(OnHomeClicked);
    }

    void OnDisable()
    {
        if (resumeButton != null) resumeButton.onClick.RemoveListener(OnResumeClicked);
        if (restartButton != null) restartButton.onClick.RemoveListener(OnRestartClicked);
        if (homeButton != null) homeButton.onClick.RemoveListener(OnHomeClicked);
    }

    void OnResumeClicked()
    {
        PlayClickSound();
        if (GameManager.Instance != null)
            GameManager.Instance.TogglePause();
    }

    void OnRestartClicked()
    {
        PlayClickSound();
        if (GameManager.Instance != null)
            GameManager.Instance.RestartCurrentLevel();
    }

    void OnHomeClicked()
    {
        PlayClickSound();
        if (GameManager.Instance != null)
            GameManager.Instance.GoToMainMenu();
    }

    void PlayClickSound()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonClickSFX);
    }
}
