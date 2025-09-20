using TMPro;
using UnityEngine;

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

public class ScoreManager : MonoBehaviour
{
    private static ScoreManager instance;

    static public int Points { get; private set; }
	static public GameMode gameMode;

    private int clearedLinesDrop; // this drop
    [SerializeField] private int clearedLinesGame; // all game
	private int maxCubePerFloor;

    [SerializeField] private int linesToLevelup = 5;
    private int linesToLevelupCount;
	[SerializeField] private float level = 1; //falling speed & points mod

    [SerializeField] private int baseLinePoints = 10;
    [SerializeField] private int comboBonus = 5;
    private bool combo;
    [SerializeField] private int comboMod;

    [SerializeField] private Transform gameScene;
    [SerializeField] private TMP_Text pointsText;
    [SerializeField] private TMP_Text levelText;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
    void Start()
    {
        maxCubePerFloor = GridManager.gameWidth * GridManager.gameWidth;
        Points = 0;
        UpdatePoints();
    }

    public static void AddDropPoints(int fallenBlocks)
    {
        Points += fallenBlocks;
        instance.UpdatePoints();
    }
    public static void PointsCheck(float fallSpeed)
    {
        for (int i = GridManager.gameHeight; i >= 0; i--)
        {
            if (GridManager.LineCheck(i))
            {
                instance.clearedLinesDrop++;
                GridManager.DestroyLine(i);
                GridManager.ShiftDownCubesAbove(i);
            }
        }
        if(instance.clearedLinesDrop >= 1)
        {
            SetPoints(fallSpeed);
            instance.combo = true;
            instance.clearedLinesDrop = 0;
        }
        else
            instance.combo = false;
    }
    private static void SetPoints(float levelPointsMod)
    {
        int LinesPointsMod = instance.GetLinesPointsMod();
        Points += instance.maxCubePerFloor * LinesPointsMod * (int)(instance.baseLinePoints * levelPointsMod); // level/fall speed = pointsMod

        if (instance.combo)
        {
            instance.comboMod++;
            Points += instance.maxCubePerFloor * instance.comboMod * (int)(instance.comboBonus * levelPointsMod);
        }
        else
        {
            instance.comboMod = 0;
        }

        instance.UpdatePoints();

        instance.clearedLinesGame += instance.clearedLinesDrop;
        instance.linesToLevelupCount += instance.clearedLinesDrop;
		if (gameMode == GameMode.Challange && instance.linesToLevelupCount - instance.linesToLevelup >= 0)
        {
            instance.level += 1f;
            instance.levelText.text = instance.level.ToString();
            instance.linesToLevelupCount -= instance.linesToLevelup;
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
        pointsText.text = Points.ToString();
    }
    public static float GetFallingSpeed()
    {
        return instance.level * 2;
    }
}