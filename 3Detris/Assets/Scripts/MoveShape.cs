using UnityEngine;
using UnityEngine.InputSystem;

public class MoveShape : MonoBehaviour
{
    //INPUT
    public InputActionAsset inputAction;

    private InputAction moveAction;
    private InputAction rotateActionX;
    private InputAction rotateActionY;
    private InputAction rotateActionZ;
    private InputAction DropAction;

    private bool moveNow;
    private bool rotateXNow;
    private bool rotateYNow;
    private bool rotateZNow;
    private bool dropNow;

    //CONTROL
    private Vector3 directionVector;
    private float rotateXTurn;
    private float rotateYTurn;
    private float rotateZTurn;
    private bool canMove = true;
    private bool canRotate = true;
    [SerializeField] private GameObject currentShape;

    //Detection
    private Transform currentGhost;
    [SerializeField] private GameObject[] ghostcubes;
    private GhostDetection[] ghostDetections;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private Vector3 checkSize = new Vector3(0.9f,0.9f,0.9f);
    private Collider[] hits = new Collider[1];
    [SerializeField] private GameObject indicatorCube;

    private void Awake()
    {
        moveAction = InputSystem.actions.FindAction("move");
        rotateActionX = InputSystem.actions.FindAction("RotateX");
        rotateActionY = InputSystem.actions.FindAction("RotateY");
        rotateActionZ = InputSystem.actions.FindAction("RotateZ");
        DropAction = InputSystem.actions.FindAction("Drop");
        obstacleLayer = LayerMask.GetMask("Solid");
        
    }
    void Update()
    {
        directionVector = moveAction.ReadValue<Vector3>();
        rotateXTurn = rotateActionX.ReadValue<float>();
        rotateYTurn = rotateActionY.ReadValue<float>();
        rotateZTurn = rotateActionZ.ReadValue<float>();
        CheckInput();
    }
    private void FixedUpdate()
    {
        Move();
        Rotate();
    }

    private void Move()
    {
        if (moveNow)
        {
            canMove = true;
            GhostCheck();
            if (canMove)
            {
                currentShape.transform.position += directionVector;
            }
            moveNow = false;
        }
    }
    private void Rotate()
    {
        if (rotateXNow || rotateYNow || rotateZNow)
        {
            //obrot ducha
            if (rotateZNow)
            {
                if (rotateZTurn >= 0)
                    currentGhost.transform.rotation = Quaternion.Euler(0, 0, 90) * currentGhost.transform.rotation;
                else if (rotateZTurn <= 0)
                    currentGhost.transform.rotation = Quaternion.Euler(0, 0, -90) * currentGhost.transform.rotation;
                Physics.SyncTransforms();
            }
            if (rotateYNow)
            {
                if (rotateYTurn >= 0)
                    currentGhost.transform.rotation = Quaternion.Euler(0, 90, 0) * currentGhost.transform.rotation;
                else if (rotateYTurn <= 0)
                    currentGhost.transform.rotation = Quaternion.Euler(0, -90, 0) * currentGhost.transform.rotation;
                Physics.SyncTransforms();
            }
            if (rotateXNow)
            {
                if (rotateXTurn >= 0)
                    currentGhost.transform.rotation = Quaternion.Euler(90, 0, 0) * currentGhost.transform.rotation;
                else if (rotateXTurn <= 0)
                    currentGhost.transform.rotation = Quaternion.Euler(-90, 0, 0) * currentGhost.transform.rotation;
                Physics.SyncTransforms();
            }

            //check
            canRotate = true;
            for (int i = 0; i < currentGhost.childCount; i++)
            {
                if (ghostDetections[i].GetghostHit(Vector3.zero))
                {
                    FailedRotationIndicator();

                    canRotate = false;
                }
            }
            currentGhost.transform.rotation = currentShape.transform.rotation;

            //obrot
            if (canRotate)
            {
                if (rotateZNow)
                {
                    if (rotateZTurn >= 0)
                        currentShape.transform.rotation = Quaternion.Euler(0, 0, 90) * currentShape.transform.rotation;
                    else if (rotateZTurn <= 0)
                        currentShape.transform.rotation = Quaternion.Euler(0, 0, -90) * currentShape.transform.rotation;
                }
                if (rotateYNow)
                {
                    if (rotateYTurn >= 0)
                        currentShape.transform.rotation = Quaternion.Euler(0, 90, 0) * currentShape.transform.rotation;
                    else if (rotateYTurn <= 0)
                        currentShape.transform.rotation = Quaternion.Euler(0, -90, 0) * currentShape.transform.rotation;
                }
                if (rotateXNow)
                {
                    if (rotateXTurn >= 0)
                        currentShape.transform.rotation = Quaternion.Euler(90, 0, 0) * currentShape.transform.rotation;
                    else if (rotateXTurn <= 0)
                        currentShape.transform.rotation = Quaternion.Euler(-90, 0, 0) * currentShape.transform.rotation;
                }
            }
            rotateXNow = false;
            rotateYNow = false;
            rotateZNow = false;
        }
    }
    private void CheckInput()
    {
        if (moveAction.WasPressedThisFrame())
        {
            moveNow = true;
        }
        if (rotateActionX.WasPressedThisFrame())
        {
            rotateXNow = true;
        }
        if (rotateActionY.WasPressedThisFrame())
        {
            rotateYNow = true;
        }
        if (rotateActionZ.WasPressedThisFrame())
        {
            rotateZNow = true;
        }
        if (DropAction.WasPressedThisFrame())
        {
            Falling falling = currentShape.GetComponent<Falling>();
            falling.DropShape();
        }
    }
    private void FailedRotationIndicator()
    {
        for (int i = 0; i < currentGhost.childCount; i++)
        {
            GameObject failedRotationIndicator = Instantiate(indicatorCube, ghostcubes[i].transform.position, ghostcubes[i].transform.rotation);
            AutoDestroy IndicatorTime = failedRotationIndicator.GetComponent<AutoDestroy>();
            IndicatorTime.autoDestroytimerTime = 0.2f;
        }
    }
    private void GetGhost()
    {
        currentGhost = currentShape.transform.GetChild(0);

        ghostcubes = new GameObject[currentGhost.childCount];
        ghostDetections = new GhostDetection[currentGhost.childCount];
        for (int i = 0; i < currentGhost.childCount; i++)
        {
            ghostcubes[i] = currentGhost.GetChild(i).gameObject;
            ghostDetections[i] = ghostcubes[i].GetComponent<GhostDetection>();
        }
    }
    private void GhostCheck()
    {
        for (int i = 0; i < currentGhost.childCount; i++)
        {
            if (ghostDetections[i].GetghostHit(directionVector))
            {
                canMove = false;
                break;
            }
        }
    }
    public void ChangeCurrentShape(GameObject newShape)
    {
        currentShape = newShape;
        GetGhost();
    }
}
