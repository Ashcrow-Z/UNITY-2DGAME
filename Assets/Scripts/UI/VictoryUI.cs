using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VictoryUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TMP_InputField nameInput;
    public Button submitButton;
    public Button homeButton;

    private bool submitted;

    void OnEnable()
    {
        submitted = false;

        if (scoreText != null && GameManager.Instance != null)
            scoreText.text = "Final Score: " + GameManager.Instance.Score;

        if (submitButton != null)
        {
            submitButton.onClick.AddListener(OnSubmitClicked);
            submitButton.interactable = true;
        }
        if (homeButton != null) homeButton.onClick.AddListener(OnHomeClicked);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopBGM();
            AudioManager.Instance.PlaySFX(AudioManager.Instance.victorySFX);
        }
    }

    void OnDisable()
    {
        if (submitButton != null) submitButton.onClick.RemoveListener(OnSubmitClicked);
        if (homeButton != null) homeButton.onClick.RemoveListener(OnHomeClicked);
    }

    void OnSubmitClicked()
    {
        if (submitted) return;
        PlayClickSound();

        string playerName = "Player";
        if (nameInput != null && !string.IsNullOrEmpty(nameInput.text))
            playerName = nameInput.text;

        if (LeaderboardManager.Instance != null && GameManager.Instance != null)
        {
            LeaderboardManager.Instance.AddEntry(
                playerName,
                GameManager.Instance.Score,
                GameManager.Instance.ElapsedTime
            );
        }

        submitted = true;
        if (submitButton != null) submitButton.interactable = false;
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
