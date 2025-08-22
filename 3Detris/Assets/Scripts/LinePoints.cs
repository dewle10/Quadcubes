using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameMode
{
    Casual,
    Challange
}
public enum BoardSize
{
    X4 = 4,
    X5 = 5,
    X6 = 6
}

public class LinePoints : MonoBehaviour
{
    static private int points;
    private int clearedLinesGame; // all game
    static public int clearedLinesDrop; // this drop
    [SerializeField] private int linesToLevelup = 5;
    private int linesToLevelupCount;
	[SerializeField] private float level = 1; //falling speed & points mod
    [SerializeField] private bool combo;
    [SerializeField] private int comboMod;
	static public GameMode gameMode;

	private int maxFloorObjNum;

    [SerializeField] private Transform gameScene;
    [SerializeField] private TMP_Text pointsText;
    [SerializeField] private TMP_Text levelText;

    static public int Score { get => points; }

	void Start()
    {
        maxFloorObjNum = GridManager.gameWidth * GridManager.gameWidth;
    }

    public void AddDropPoints(int fallenBlocks)
    {
        points += fallenBlocks;
        UpdatePoints();
    }
    public void PointsCheck(float fallSpeed)
    {
        for (int i = GridManager.gameHeight; i >= 0; i--)
        {
            if (GridManager.LineCheck(i))
            {
                GridManager.DestroyLine(i);
                GridManager.ShiftDownCubesAbove(i);
            }
        }
        if(clearedLinesDrop >= 1)
        {
            SetPoints(fallSpeed);
            combo = true;
            clearedLinesDrop = 0;
        }
        else
            combo = false;
    }
    private void SetPoints(float levelPointsMod)
    {
        int LinesPointsMod = GetLinesPointsMod();
        points += maxFloorObjNum * LinesPointsMod * (int)(10 * levelPointsMod); // level/fall speed = pointsMod
        //Debug.Log(maxFloorObjNum * LinesPointsMod * (int)(10 * levelPointsMod));

        if (combo)
        {
            comboMod++;
            points += maxFloorObjNum * comboMod * (int)(5 * levelPointsMod);

            //Debug.Log(maxFloorObjNum * comboMod * (int)(5 * levelPointsMod) + " combo");
        }
        else
        {
            comboMod = 0;
        }

        UpdatePoints();

        clearedLinesGame += clearedLinesDrop;
		linesToLevelupCount += clearedLinesDrop;
		if (gameMode == GameMode.Challange && linesToLevelupCount - linesToLevelup >= 0)
        {
            level += 1f;
			levelText.text = level.ToString();
            linesToLevelupCount -= linesToLevelup;
		}
    }
    private int GetLinesPointsMod()
    {
        return clearedLinesDrop switch
        {
            1 => 1,// single
            2 => 4,// double
            3 => 7,// triple
            4 => 12,// Quad
            _ => 1,// no lines, or more than 4
        };
    }
    private void UpdatePoints()
    {
        pointsText.text = points.ToString();
    }
    public float GetFallingSpeed()
    {
        return level;
    }
    //private void OnDrawGizmos()
    //{
    //    if (!Application.isPlaying) return;

    //    Gizmos.color = Color.red;
    //    for (int i = 0; i < gameHeight; i++)
    //    {
    //        Vector3 center = new Vector3(
    //            groundPos.position.x,
    //            groundPos.position.y + i + 0.5f,
    //            groundPos.position.z
    //        );
    //        Gizmos.DrawWireCube(center, checkSize);
    //    }
    //}
}
