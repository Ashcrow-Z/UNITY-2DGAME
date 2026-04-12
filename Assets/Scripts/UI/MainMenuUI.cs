using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject settingsPanel;
    public GameObject leaderboardPanel;

    [Header("Main Buttons")]
    public Button startButton;
    public Button settingsButton;
    public Button leaderboardButton;
    public Button quitButton;

    void Start()
    {
        ShowMainPanel();

        if (startButton != null) startButton.onClick.AddListener(OnStartClicked);
        if (settingsButton != null) settingsButton.onClick.AddListener(OnSettingsClicked);
        if (leaderboardButton != null) leaderboardButton.onClick.AddListener(OnLeaderboardClicked);
        if (quitButton != null) quitButton.onClick.AddListener(OnQuitClicked);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMenuMusic();
    }

    void OnStartClicked()
    {
        PlayClickSound();
        if (GameManager.Instance != null)
            GameManager.Instance.StartGame();
    }

    void OnSettingsClicked()
    {
        PlayClickSound();
        ShowSettingsPanel();
    }

    void OnLeaderboardClicked()
    {
        PlayClickSound();
        ShowLeaderboardPanel();
    }

    void OnQuitClicked()
    {
        PlayClickSound();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void ShowMainPanel()
    {
        if (mainPanel != null) mainPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (leaderboardPanel != null) leaderboardPanel.SetActive(false);
    }

    void ShowSettingsPanel()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    void ShowLeaderboardPanel()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (leaderboardPanel != null) leaderboardPanel.SetActive(true);
    }

    public void OnBackFromSettings()
    {
        PlayClickSound();
        ShowMainPanel();
    }

    public void OnBackFromLeaderboard()
    {
        PlayClickSound();
        ShowMainPanel();
    }

    void PlayClickSound()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonClickSFX);
    }
}
