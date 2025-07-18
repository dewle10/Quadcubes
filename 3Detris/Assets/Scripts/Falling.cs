using System.Collections;
using UnityEngine;

public class Falling : MonoBehaviour
{
    private bool falling = true;
    private bool aboveSolid;
    private float rayDistance = 1.1f;
    public float fallingSpeed = 1f;
    private float fallingTimerTime = 5.0f;
    private float fallingTimercounter;
    [SerializeField] private int fallenBlocksPoints; 

    public GameObject[] cubes;
    LayerMask layerMask;

    [SerializeField] private ShapeDispenser shapeDispenser;
    [SerializeField] private LinePoints linePoints;

    private Vector3[] indicatorPos;
    [SerializeField] private GameObject ghostIndicator;
    private GameObject[] ghostIndicators;

    public bool _Debug_IsLine;

    private void Awake()
    {
        layerMask = LayerMask.GetMask("Solid");
    }
    private void Start()
    {
        shapeDispenser = FindFirstObjectByType<ShapeDispenser>();
        linePoints = FindFirstObjectByType<LinePoints>();
        if (!_Debug_IsLine)
            GhostIndicatorArrays();
        fallingSpeed = linePoints.GetFallingSpeed();
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
    private void Fall()
    {
        fallingTimercounter += Time.deltaTime;

        if (fallingTimercounter >= fallingTimerTime / fallingSpeed)
        {
            if (aboveSolid)
            {
                MakeSolid();
            }
            else
            {
                fallingTimercounter = 0;
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
    }
    private void GroundCheck()
    {
        foreach (GameObject obj in cubes)
        {
            aboveSolid = false;
            RaycastHit hit;
            if (Physics.Raycast(obj.transform.position, Vector3.down, out hit, rayDistance, layerMask))
            {
                Debug.DrawRay(obj.transform.position, Vector3.down * hit.distance, Color.yellow);
                //Debug.Log("Did Hit");
                aboveSolid = true;
                break;
            }
            else
            {
                Debug.DrawRay(obj.transform.position, Vector3.down * rayDistance, Color.white);
                //Debug.Log("Did not Hit");
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
        }
        fallingTimercounter = 0;
        DestroyGhostCubes();
        linePoints.GetFallingSpeed();
        shapeDispenser.ResetHold();

        if (!_Debug_IsLine)
        {
            linePoints.PointsCheck(fallingSpeed);
            linePoints.AddDropPoints(fallenBlocksPoints);
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
                Debug.DrawRay(obj.transform.position, Vector3.down * hit.distance, Color.blue);
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
                //Debug.Log("ghost");
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
    public void DestroyShape()
    {
        DestroyGhostCubes();
        Destroy(gameObject);
    }
    public void DestroyIfEmpty()
    {
        if (transform.childCount <= 1)
            Destroy(gameObject);
    }
    public void ResetFallingTimer()
    {
        fallingTimercounter = 0;
    }
}