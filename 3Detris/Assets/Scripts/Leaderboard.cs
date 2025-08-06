using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ScoreEntry
{
    public string playerName;
    public int score;
    public BoardSize boardSize;
    public GameMode gameMode;
}
[System.Serializable]
public class LeaderboardData
{
    public List<ScoreEntry> scores = new();
}

public class Leaderboard : MonoBehaviour
{
    static readonly string prefKey = "Leaderboard";

    public void AddScore(string name, int score, BoardSize size, GameMode mode)
    {
        LeaderboardData data = LoadLeaderboard();
        data.scores.Add(new ScoreEntry { playerName = name, score = score, boardSize = size, gameMode = mode });

        data.scores = data.scores.OrderByDescending(e => e.score).ToList();

        if (data.scores.Count > 100)
            data.scores = data.scores.Take(100).ToList();

        SaveLeaderboard(data);
        //Debug.Log(data.scores[0].score);
    }

    private void SaveLeaderboard(LeaderboardData data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(prefKey, json);
        PlayerPrefs.Save();
    }

    private LeaderboardData LoadLeaderboard()
    {
        if (!PlayerPrefs.HasKey(prefKey)) return new LeaderboardData();
        string json = PlayerPrefs.GetString(prefKey);
        return JsonUtility.FromJson<LeaderboardData>(json);
    }
    static public List<ScoreEntry> GetScores()
    {
        if (!PlayerPrefs.HasKey(prefKey)) 
        {
            return new LeaderboardData().scores;
        }
        string json = PlayerPrefs.GetString(prefKey);
        return JsonUtility.FromJson<LeaderboardData>(json).scores;
    }
}