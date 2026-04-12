using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance { get; private set; }

    [System.Serializable]
    public class LeaderboardEntry
    {
        public string playerName;
        public int score;
        public float time;
    }

    [System.Serializable]
    private class LeaderboardData
    {
        public List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
    }

    private LeaderboardData data;
    private string savePath;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        savePath = Path.Combine(Application.persistentDataPath, "leaderboard.json");
        LoadLeaderboard();
    }

    public void AddEntry(string playerName, int score, float time)
    {
        data.entries.Add(new LeaderboardEntry
        {
            playerName = playerName,
            score = score,
            time = time
        });

        data.entries = data.entries
            .OrderByDescending(e => e.score)
            .ThenBy(e => e.time)
            .Take(10)
            .ToList();

        SaveLeaderboard();
    }

    public List<LeaderboardEntry> GetEntries()
    {
        return data.entries;
    }

    void SaveLeaderboard()
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
    }

    void LoadLeaderboard()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            data = JsonUtility.FromJson<LeaderboardData>(json);
        }
        else
        {
            data = new LeaderboardData();
        }
    }
}
