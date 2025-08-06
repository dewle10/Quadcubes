using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class GridManager : MonoBehaviour
{
    private static GridManager instance;

    public static int gameWidth = 6;
    public static int gameHeight = 12;

    public static Transform[,,] grid = new Transform[gameWidth, gameHeight + 5, gameWidth];
    public static bool gameOver = false;

    static private Leaderboard leaderboard;

    private GameObject walls;
    private GameObject outOfBounds;

    private Transform center;
    [SerializeField] private float explosionForce = 0f;
    [SerializeField] private float explosionRadius = 100f;
    [SerializeField] private float upwardsModifier = 0.5f;
    private Vector3 explosionPosition;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        gameOver = false;
        leaderboard = gameObject.GetComponent<Leaderboard>();

        walls = GameObject.Find("Walls");
        outOfBounds = GameObject.Find("OutOfBounds col"); 

        center = GameObject.Find("Center").transform;
        explosionPosition = center.position;
        explosionPosition.y -= 2f;
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
                        gameOver = true;
                        instance.StartCoroutine(instance.GameOver());
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
    private IEnumerator GameOver()
    {
        SoundManager.StopMusic();
        SoundManager.PlaySound(SoundType.GameOver);
        yield return new WaitForSeconds(0.2f);
        walls.SetActive(false); 
        yield return new WaitForSeconds(0.2f);
        walls.SetActive(true); 
        yield return new WaitForSeconds(0.2f);
        walls.SetActive(false); 
        yield return new WaitForSeconds(0.2f);
        walls.SetActive(true); 
        yield return new WaitForSeconds(0.2f);
        walls.SetActive(false);

        for (int x = 0; x < gameWidth; x++)
        {
            for (int y = 0; y < gameHeight + 4; y++)
            {
                for (int z = 0; z < gameWidth; z++)
                {
                    if (grid[x, y, z])
                    {
                        yield return new WaitForSeconds(0.05f);
                        MeshRenderer renderer = grid[x, y, z].gameObject.GetComponentInChildren<MeshRenderer>();
                        renderer.material.color = Color.red;
                        SoundManager.PlaySound(SoundType.ColorChange);
                    }
                }
            }
        }
        outOfBounds.SetActive(false);

        yield return new WaitForSeconds(0.5f);
        for (int x = 0; x < gameWidth; x++)
        {
            for (int y = 0; y < gameHeight + 4; y++)
            {
                for (int z = 0; z < gameWidth; z++)
                {
                    if (grid[x, y, z])
                    {
                        grid[x, y, z].gameObject.AddComponent<Rigidbody>();
                    }
                }
            }
        }
        Explode();
        StartCoroutine(DebugResetGame());
        //Debug.Log("loss");
        //SceneManager.LoadScene(0);

        //leaderboard.AddScore("YOU", LinePoints.Score, (BoardSize)gameWidth, LinePoints.gameMode);
    }
    private void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(explosionPosition, explosionRadius);

        SoundManager.PlaySound(SoundType.Explode);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, explosionPosition, explosionRadius, upwardsModifier, ForceMode.Impulse);
            }
        }
    }
    private IEnumerator DebugResetGame()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(3);
    }
}
