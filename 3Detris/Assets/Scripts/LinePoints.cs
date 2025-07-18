using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LinePoints : MonoBehaviour
{
    private int points;
    private int clearedLinesGame; // all game
    static public int clearedLinesDrop; // this drop
    [SerializeField] private int linesToLevelup = 5;
    [SerializeField] private float level = 1; //falling speed & points mod
    [SerializeField] private bool combo;
    [SerializeField] private int comboMod;

    private int maxFloorObjNum;

    [SerializeField] private Transform gameScene;
    [SerializeField] private TMP_Text pointsText;
    void Start()
    {
        maxFloorObjNum = GridManager.gamewidth * GridManager.gamewidth;
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

            GridManager.LossCheck();
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

        clearedLinesGame++;
        if (clearedLinesGame % linesToLevelup == 0)
            level += 0.5f;
    }
    private int GetLinesPointsMod()
    {
        switch (clearedLinesDrop)
        {
            case 1: return 1;   // single
            case 2: return 4;   // double
            case 3: return 7;   // triple
            case 4: return 12;   // Quad
            default: return 1;  // no lines, or more than 4
        }
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
