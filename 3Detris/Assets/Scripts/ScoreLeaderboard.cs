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

public static class ScoreLeaderboard
{
    private const int MaxEntries = 100;
    private const string FileName = "leaderboard.json";
    private static string FilePath => Path.Combine(Application.persistentDataPath, FileName);

    public static void AddScore(string name, int score, BoardSize size, GameMode mode)
    {
        LeaderboardData data = LoadLeaderboard();
        data.scores.Add(new ScoreEntry { playerName = name, score = score, boardSize = size, gameMode = mode });

        data.scores = data.scores.OrderByDescending(e => e.score).Take(MaxEntries).ToList();

        SaveLeaderboard(data);
        //Debug.Log(data.scores[0].score);
    }

    private static void SaveLeaderboard(LeaderboardData data)
    {
        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(FilePath, json);
        }
        catch (IOException e)
        {
            Debug.LogError($"Failed to save leaderboard: {e.Message}");
        }
    }

    public static LeaderboardData LoadLeaderboard()
    {
        try
        {
            if (!File.Exists(FilePath)) return new LeaderboardData();
            string json = File.ReadAllText(FilePath);
            return JsonUtility.FromJson<LeaderboardData>(json);
        }
        catch (IOException e)
        {
            Debug.LogError($"Failed to load leaderboard: {e.Message}");
            return new LeaderboardData();
        }
    }
}