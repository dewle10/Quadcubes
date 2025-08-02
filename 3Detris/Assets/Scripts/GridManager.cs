using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class GridManager : MonoBehaviour
{
    public static int gameWidth = 6;
    public static int gameHeight = 12;

    public static Transform[,,] grid = new Transform[gameWidth, gameHeight + 5, gameWidth];

    static private Leaderboard leaderboard;

    private void Start()
    {
        leaderboard = gameObject.GetComponent<Leaderboard>();
    }

    public static void AddToGrid(Transform cubeTran)
    {
        Vector3 pos = cubeTran.position;
        int x = (int)pos.x;
        int y = (int)pos.y;
        int z = (int)pos.z;
        grid[x, y, z] = cubeTran;
        //Debug.Log($"{cubeTran.gameObject} + {x} + {y} + {z}");
    }
    public static void ShiftDownCubesAbove(int floorNum)
    {
        for (int y = floorNum + 1; y < gameHeight + 5; y++)
        {
            for (int x = 0; x < gameWidth; x++)
            {
                for (int z = 0; z < gameWidth; z++)
                {
                    Transform cube = grid[x, y, z];
                    if (cube != null)
                    {
                        grid[x, y - 1, z] = cube;
                        grid[x, y, z] = null;

                        cube.position += Vector3.down;
                        RowColors.ChangeColor(cube.gameObject);
                    }

                }
            }
        }
        //Debug.Log(floorNum);
    }
    public static void LossCheck()
    {
        for (int x = 0; x < gameWidth; x++)
        {
            for (int z = 0; z < gameWidth; z++)
            {
                for (int y = gameHeight + 4; y >= gameHeight; y--) // +4 because grid height is higher than game height
                {
                    if (grid[x, y, z])
                    {
                        SceneManager.LoadScene(0);
                        //Debug.Log("loss");
                        leaderboard.AddScore("YOU", LinePoints.Score, (BoardSize)gameWidth, LinePoints.gameMode);
                        return;
                    }
                }
            }
        }
    }
    public static void DestroyLine(int floorNum)
    {
        for (int x = 0; x < gameWidth; x++)
        {
            for (int z = 0; z < gameWidth; z++)
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
    public static bool LineCheck(int y)
    {
        for (int x = 0; x < gameWidth; x++)
        {
            for (int z = 0; z < gameWidth; z++)
            {
                if (grid[x, y, z] == null)
                    return false;
            }
        }
        LinePoints.clearedLinesDrop++;
        SoundManager.PlaySound(SoundType.ClearLine, 1.2f);
        return true;
    }
}
