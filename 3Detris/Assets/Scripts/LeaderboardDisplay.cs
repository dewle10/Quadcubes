using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class LeaderboardDisplay : MonoBehaviour
{
    [SerializeField] private GameMode mode;
    [SerializeField] private Transform content;
    [SerializeField] private GameObject scoreItem;
    public static GameMode displayMode = GameMode.Challange;
    [SerializeField] private TMP_FontAsset font;

    public void Refresh()
    {
        SoundManager.PlaySound(SoundType.ClickButton);
        foreach (Transform t in content) Destroy(t.gameObject);

        List<ScoreEntry> scores = ScoreLeaderboard.LoadLeaderboard().scores;
        if (scores.Count == 0)
        {
            Instantiate(scoreItem, content).GetComponentInChildren<TextMeshProUGUI>().text = "No Entries";
            return;
        }

        int rank = 1;
        foreach (ScoreEntry entry in scores)
        {
            if (entry.gameMode == displayMode)
            {
                GameObject lbRow = Instantiate(scoreItem, content);
                TMP_Text[] texts = lbRow.GetComponentsInChildren<TMP_Text>();
                texts[0].text = rank.ToString();
                texts[1].text = entry.playerName;
                texts[2].text = entry.score.ToString();
                texts[3].text = entry.boardSize.ToString();
                rank++;
            }
        }
    }
}
