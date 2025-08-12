using System.Collections.Generic;
using System.IO;
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
    private const string FileName = "leaderboard.json";
    private static string FilePath => Path.Combine(Application.persistentDataPath, FileName);

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
        File.WriteAllText(FilePath, json);
    }

    public static LeaderboardData LoadLeaderboard()
    {
        if (!File.Exists(FilePath)) return new LeaderboardData();
        string json = File.ReadAllText(FilePath);
        return JsonUtility.FromJson<LeaderboardData>(json);
    }
}