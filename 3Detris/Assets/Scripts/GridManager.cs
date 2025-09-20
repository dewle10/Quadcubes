using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

public class GridManager : MonoBehaviour
{
    private static GridManager instance;

    public static int gameWidth = 6;
    public static int gameHeight = 12;
    private const int BufferHeight = 5;

    private static Transform[,,] grid = new Transform[gameWidth, gameHeight + BufferHeight, gameWidth];
    private static List<Transform> placedCubes = new();
    public static bool gameOver = false;

    private GameObject walls;
    private GameObject outOfBounds;

    private Transform center;
    [SerializeField] private float explosionForce = 0f;
    [SerializeField] private float explosionRadius = 100f;
    [SerializeField] private float upwardsModifier = 0.5f;
    private Vector3 explosionPosition;

    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private bool isDemo;

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
        if (isDemo) gameWidth = 5;
    }

    private void Start()
    {
        gameOver = false;

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
        placedCubes.Add(cubeTran);
    }
    public static void ShiftDownCubesAbove(int floorNum)
    {
        for (int y = floorNum + 1; y < gameHeight + BufferHeight; y++)
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
    }
    public static void LossCheck()
    {
        for (int x = 0; x < gameWidth; x++)
        {
            for (int z = 0; z < gameWidth; z++)
            {
                for (int y = gameHeight + BufferHeight - 1; y >= gameHeight; y--)
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
                    cube.GetComponentInChildren<VisualEffects>().PlayLCParticle();

                    Destroy(cube.gameObject);
                    grid[x, floorNum, z] = null;
                    placedCubes.Remove(cube);
                }
                else
                    Debug.LogWarning("Error: No cube to Destroy");
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
        SoundManager.PlaySound(SoundType.ClearLine, 1.2f);
        return true;
    }
    private IEnumerator GameOver()
    {
        SoundManager.StopMusic();
        SoundManager.PlaySound(SoundType.GameOver);
        ScoreLeaderboard.AddScore("YOU", ScoreManager.Points, (BoardSize)gameWidth, ScoreManager.gameMode);

        //Walls blinking
        for (int i = 0; i < 5; i++)
        {
            walls.SetActive(i % 2 != 0);
            yield return new WaitForSeconds(0.2f);
        }

        //Change all to red color
        float waitTime = 0.05f;
        float changeAfter = 3f;
        float newWaitTime = 0.03f;
        float startTime = Time.time;

        for (int y = 0; y < gameHeight + BufferHeight; y++)
        {
            for (int x = 0; x < gameWidth; x++)
            {
                for (int z = 0; z < gameWidth; z++)
                {
                    if (grid[x, y, z])
                    {
                        if (Time.time >= startTime + changeAfter)
                            waitTime = newWaitTime;

                        yield return new WaitForSeconds(waitTime);

                        MeshRenderer renderer = grid[x, y, z].gameObject.GetComponentInChildren<MeshRenderer>();
                        renderer.material.color = Color.red;
                        SoundManager.PlaySound(SoundType.ColorChange);
                    }
                }
            }
        }
        //Explosion
        yield return new WaitForSeconds(0.5f);
        outOfBounds.SetActive(false);
        Explode();
        yield return new WaitForSeconds(3f);

        ShowGameOverUI();
    }

    private void ShowGameOverUI()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        gameOverMenu.SetActive(true);
        finalScoreText.text = ScoreManager.Points.ToString();
    }
    private void Explode()
    {
        foreach(Transform cube in placedCubes)
        {
            Rigidbody rb = cube.gameObject.AddComponent<Rigidbody>();
            rb.AddExplosionForce(explosionForce, explosionPosition, explosionRadius, upwardsModifier, ForceMode.Impulse);
        }
        SoundManager.PlaySound(SoundType.Explode);
    }
    public void Restart()
    {
        SoundManager.PlaySound(SoundType.ClickButton);
        gameOver = false;
        LoadingScreen.sceneToLoad = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("Loading");
    }
}
