using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LeaderboardUI : MonoBehaviour
{
    public Transform entriesContainer;
    public GameObject entryPrefab;
    public Button backButton;

    void OnEnable()
    {
        PopulateLeaderboard();
        if (backButton != null)
            backButton.onClick.AddListener(OnBackClicked);
    }

    void OnDisable()
    {
        if (backButton != null)
            backButton.onClick.RemoveListener(OnBackClicked);
    }

    void PopulateLeaderboard()
    {
        if (entriesContainer == null || entryPrefab == null) return;

        foreach (Transform child in entriesContainer)
        {
            Destroy(child.gameObject);
        }

        if (LeaderboardManager.Instance == null) return;

        List<LeaderboardManager.LeaderboardEntry> entries = LeaderboardManager.Instance.GetEntries();

        for (int i = 0; i < entries.Count; i++)
        {
            GameObject entryObj = Instantiate(entryPrefab, entriesContainer);
            entryObj.SetActive(true);
            TextMeshProUGUI text = entryObj.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
            {
                int minutes = (int)(entries[i].time / 60f);
                int seconds = (int)(entries[i].time % 60f);
                text.text = string.Format("#{0}  {1}  -  {2} pts  -  {3:00}:{4:00}",
                    i + 1, entries[i].playerName, entries[i].score, minutes, seconds);
            }
        }
    }

    void OnBackClicked()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonClickSFX);

        MainMenuUI mainMenu = GetComponentInParent<MainMenuUI>();
        if (mainMenu != null) mainMenu.OnBackFromLeaderboard();
    }
}
