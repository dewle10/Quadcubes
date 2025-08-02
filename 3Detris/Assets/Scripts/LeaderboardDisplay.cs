using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class LeaderboardDisplay : MonoBehaviour
{
    [SerializeField] private GameMode mode;
    [SerializeField] private BoardSize sizeFilter;
    [SerializeField] private Transform content;
    [SerializeField] private GameObject scoreItem;

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        foreach (Transform t in content) Destroy(t.gameObject);

        List<ScoreEntry> scores = Leaderboard.GetScores();
        if (scores.Count == 0)
        {
            Instantiate(scoreItem, content).GetComponentInChildren<TextMeshProUGUI>().text = "No Entries";
            return;
        }

        int rank = 1;
        foreach (ScoreEntry entry in scores)
        {
            GameObject lbRow = Instantiate(scoreItem, content);
            TMP_Text[] texts = lbRow.GetComponentsInChildren<TMP_Text>();
            texts[0].text = rank.ToString();
            texts[1].text = entry.playerName;
            texts[2].text = entry.score.ToString();
            texts[3].text = entry.boardSize.ToString();
            rank++;
        }
        Debug.Log("refresh");
    }
}
