using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LinePoints : MonoBehaviour
{
    private int points;
    private int clearedLinesGame; // all game
    private int clearedLinesDrop; // this drop
    [SerializeField] private int linesToLevelup = 5;
    [SerializeField] private float level = 1; //falling speed & points mod
    [SerializeField] private bool combo;
    [SerializeField] private int comboMod;

    private int maxFloorObjNum;

    private static int gamewidth = 6;
    private static int gameHeight = 12;
    [SerializeField] private Transform gameScene;
    [SerializeField] private TMP_Text pointsText;

    //GRID
    public static Transform[,,] grid = new Transform[gamewidth, gameHeight+5, gamewidth];


    void Start()
    {
        maxFloorObjNum = gamewidth * gamewidth;
    }

    public void AddDropPoints(int fallenBlocks)
    {
        points += fallenBlocks;
        UpdatePoints();
    }
    public void PointsCheck(float fallSpeed)
    {
        for (int i = gameHeight; i >= 0; i--)
        {
            if (LineCheck(i))
            {
                DestroyLine(i);
                ShiftDownCubesAbove(i);
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

            LossCheck();
    }
    private bool LineCheck(int y)
    {
        for (int x = 0; x < gamewidth; x++)
        {
            for (int z = 0; z < gamewidth; z++)
            {
                if (grid[x, y, z] == null)
                    return false;
            }
        }
        clearedLinesDrop++;
        SoundManager.PlaySound(SoundType.ClearLine, 1.2f);
        return true;
    }
    private void DestroyLine(int floorNum)
    {
        for (int x = 0; x < gamewidth; x++)
        {
            for (int z = 0; z < gamewidth; z++)
            {
                Transform cube = grid[x, floorNum, z];
                if (cube != null)
                {
                    Destroy(cube.gameObject);
                    grid[x, floorNum, z] = null;
                }
                else
                {
                    Debug.Log("Error: No cube to Destroy");
                }
            }
        }
    }
    private void LossCheck()
    {
        for (int x = 0; x < gamewidth; x++)
        {
            for (int z = 0; z < gamewidth; z++)
            {
                for (int y = gameHeight+4; y >= gameHeight; y--) // +4 because grid height is higher than game height
                {
                    if (grid[x, y, z])
                        SceneManager.LoadScene(0);
                }
            }
        }
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

    //grid
    public void AddToGrid(Transform cubePos)
    {
        int x = (int)cubePos.position.x;
        int y = (int)cubePos.position.y;
        int z = (int)cubePos.position.z;
        grid[x, y, z] = cubePos;
        //Debug.Log(cubePos.position);
        //Debug.Log(x + " " + y + " " + z);
    }
    private void ShiftDownCubesAbove(int floorNum)
    {
        for (int y = floorNum + 1; y < gameHeight + 5; y++)
        {
            for (int x = 0; x < gamewidth; x++)
            {
                for (int z = 0; z < gamewidth; z++)
                {
                    Transform cube = grid[x, y, z];
                    if (cube != null)
                    {
                        grid[x,y-1,z] = cube;
                        grid[x, y, z] = null;

                        cube.position += Vector3.down;
                    }

                }
            }
        }
        //Debug.Log(floorNum);
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
