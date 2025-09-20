using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class Falling : MonoBehaviour
{
    public float fallingSpeed = 1f;
    [Space(10)]
    [SerializeField] private GameObject ghostIndicator;
    [SerializeField] private GameObject lights;
    public GameObject[] cubes;

    //Falling
    private bool falling = true;
    private readonly float fallingTimerTime = 5.0f;
    private float fallingTimerCounter;
    //GroundCheck
    private bool isOnSolid;
    private readonly float rayDistance = 1.1f;
    //Ghost
    private Vector3[] indicatorPos;
    private GameObject[] ghostIndicators;

    private int fallenBlocksPoints; 

    private LayerMask layerMask;
    private ShapeDispenser shapeDispenser;
    private Transform gameArena;

    [Space(10)]
    public bool _Debug_IsLine; //For pre-placed lines

    private void Awake()
    {
        layerMask = LayerMask.GetMask("Solid");
    }
    private void Start()
    {
        shapeDispenser = FindFirstObjectByType<ShapeDispenser>();
        gameArena = FindFirstObjectByType<SceneShake>().transform;
        if (!_Debug_IsLine)
            GhostIndicatorArrays();
        fallingSpeed = ScoreManager.GetFallingSpeed();
        if (_Debug_IsLine)
        {
            MakeSolid();
        }
    }
    private void FixedUpdate()
    {
        if (falling)
        {
            GroundCheck();
        }
    }
    void Update()
    {
        if (falling)
        {
            Fall();
            GhostIndicator();
        }
        else
            DestroyIfEmpty();
    }


    #region FALLING
    private void Fall()
    {
        fallingTimerCounter += Time.deltaTime;

        if (fallingTimerCounter >= fallingTimerTime / fallingSpeed)
        {
            if (isOnSolid)
            {
                MakeSolid();
            }
            else
            {
                fallingTimerCounter = 0;
                transform.position += Vector3.down;
            }
        }
    }
    public void DropShape()
    {
        float distance = RayDistance() - 0.5f; // 0.5 to center of cube
        transform.position += Vector3.down * distance;
        fallenBlocksPoints += (int)distance*2; //droped blocks x2 points
        MakeSolid();

        SceneShake.Shake();
    }
    #endregion

    #region GROUND & SOLID
    private void GroundCheck() //Checks if shape is on ground or solid shape 
    {
        isOnSolid = false;
        foreach (GameObject obj in cubes)
        {
            if (Physics.Raycast(obj.transform.position, Vector3.down, out RaycastHit hit, rayDistance, layerMask))
            {
                isOnSolid = true;
                break;
            }
        }
    }
    private void MakeSolid()
    {
        falling = false;
        SnapToGrid(0.5f, transform);
        gameObject.layer = 6;
        foreach (GameObject cube in cubes)
        {
            cube.layer = 6; //solid
            GridManager.AddToGrid(cube.transform);
            RowColors.ChangeColor(cube);
            cube.GetComponentInChildren<VisualEffects>().PlayPlaceParticle();
        }
        fallingTimerCounter = 0;
        DestroyGhostCubes();
        shapeDispenser.ResetHold();
        transform.SetParent(gameArena, true);

        if (!_Debug_IsLine)
        {
            lights.SetActive(false);
            ScoreManager.PointsCheck(fallingSpeed);
            ScoreManager.AddDropPoints(fallenBlocksPoints);
            shapeDispenser.SpawnShape(false);
        }
    }
    private float RayDistance()
    {
        float rayDistance = 100f;
        foreach (GameObject obj in cubes)
        {
            RaycastHit hit;
            if (Physics.Raycast(obj.transform.position, Vector3.down, out hit, 50, layerMask))
            {
                float distance = hit.distance - .05f; //cubes are slightly smaller than 1f
                if (rayDistance >= distance)
                {
                    rayDistance = distance;
                }
            }
        }
        return rayDistance;
    }
    private void SnapToGrid(float gridUnit, Transform obj)
    {
        Vector3 newPos = obj.position;
        newPos.x = Mathf.Round(newPos.x / gridUnit) * gridUnit;
        newPos.y = Mathf.Round(newPos.y / gridUnit) * gridUnit;
        newPos.z = Mathf.Round(newPos.z / gridUnit) * gridUnit;
        obj.position = newPos;
    }
    #endregion

    #region GHOST INDICATOR
    private void GhostIndicator()
    {
        for (int i = 0; i < cubes.Length; i++)
        {
            Vector3 newIndicatorPos = cubes[i].transform.position - new Vector3(0, RayDistance() - 0.5f, 0);
            float diferenceDistance = Vector3.Distance(indicatorPos[i], newIndicatorPos);
            if(diferenceDistance > 0.1f)
            {
                indicatorPos[i] = newIndicatorPos;
                ghostIndicators[i].transform.position = indicatorPos[i];
                SnapToGrid(0.5f, ghostIndicators[i].transform);
            }
        }
    }
    private void GhostIndicatorArrays()
    {
        indicatorPos = new Vector3[cubes.Length];
        ghostIndicators = new GameObject[cubes.Length];
        for (int i = 0; i < cubes.Length; i++)
        {
            indicatorPos[i] = new Vector3(1000f, 1000f, 1000f);
            ghostIndicators[i] = Instantiate(ghostIndicator, indicatorPos[i], Quaternion.identity);
            if (cubes[i].CompareTag("RotateIndicator"))
            {
                MeshRenderer renderer = ghostIndicators[i].GetComponent<MeshRenderer>();
                renderer.material.color = new Color(0.490566f, 0.4666855f, 0.1920612f);
            }
        }
    }
    private void DestroyGhostCubes()
    {
        if (!_Debug_IsLine)
        {
            foreach (var obj in ghostIndicators)
            {
                Destroy(obj);
            }
        }
    }
    #endregion

    #region UTILITY
    public void DestroyShape()
    {
        DestroyGhostCubes();
        Destroy(gameObject);
    }
    public void DestroyIfEmpty()
    {
        if (transform.childCount == 0)
            Destroy(gameObject);
    }
    public void ResetFallingTimer()
    {
        fallingTimerCounter = 0;
    }
    #endregion
}