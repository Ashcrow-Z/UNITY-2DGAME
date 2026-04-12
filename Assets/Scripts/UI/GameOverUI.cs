using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public Button retryButton;
    public Button homeButton;

    void OnEnable()
    {
        if (scoreText != null && GameManager.Instance != null)
            scoreText.text = "Score: " + GameManager.Instance.Score;

        if (retryButton != null) retryButton.onClick.AddListener(OnRetryClicked);
        if (homeButton != null) homeButton.onClick.AddListener(OnHomeClicked);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopBGM();
            AudioManager.Instance.PlaySFX(AudioManager.Instance.defeatSFX);
        }
    }

    void OnDisable()
    {
        if (retryButton != null) retryButton.onClick.RemoveListener(OnRetryClicked);
        if (homeButton != null) homeButton.onClick.RemoveListener(OnHomeClicked);
    }

    void OnRetryClicked()
    {
        PlayClickSound();
        if (GameManager.Instance != null)
            GameManager.Instance.RestartLevel();
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
