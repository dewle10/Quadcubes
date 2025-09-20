using UnityEngine;
using UnityEngine.InputSystem;

public class MoveShape : MonoBehaviour
{
    //INPUT
    private InputAction moveAction;
    private InputAction moveDownAction;
    private InputAction rotateActionX;
    private InputAction rotateActionY;
    private InputAction rotateActionZ;
    private InputAction DropAction;

    private bool moveNow;
    private bool rotateXNow;
    private bool rotateYNow;
    private bool rotateZNow;

    [SerializeField] private float dasTime = 0.2f;  //Delayed Auto Shift DAS
    private float dasCounter;
    [SerializeField] private float arrTime = 0.05f; //repeatRate ARR
    private float arrCounter;

    //CONTROL
    private Vector3 directionVector;
    private float rotateXTurn;
    private float rotateYTurn;
    private float rotateZTurn;
    private bool canMove = true;
    private bool canRotate = true;
    private bool wallKick = false;
    private GameObject currentShape;
    private CameraTracker cameraTracker;
    [SerializeField] private int sector;

    //Detection
    private Transform currentGhost;
    [SerializeField] private GameObject[] ghostcubes;
    private GhostDetection[] ghostDetections;
    [SerializeField] private GameObject indicatorCube;
    private WallKicks[] walls;

    private void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        moveDownAction = InputSystem.actions.FindAction("Move Down");
        rotateActionX = InputSystem.actions.FindAction("Rotate X");
        rotateActionY = InputSystem.actions.FindAction("Rotate Y");
        rotateActionZ = InputSystem.actions.FindAction("Rotate Z");
        DropAction = InputSystem.actions.FindAction("Drop");
        walls = FindObjectsByType<WallKicks>(FindObjectsSortMode.None);
        cameraTracker = FindFirstObjectByType<CameraTracker>();
    }
    private void OnEnable()
    {
        moveAction.Enable();
        moveDownAction.Enable();
        rotateActionX.Enable();
        rotateActionY.Enable();
        rotateActionZ.Enable();
        DropAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        moveDownAction.Disable();
        rotateActionX.Disable();
        rotateActionY.Disable();
        rotateActionZ.Disable();
        DropAction.Disable();
    }
    void Update()
    {
        if (!GridManager.gameOver && !Pause.IsPaused)
            CheckInput();
    }
    private void FixedUpdate()
    {
        if (!GridManager.gameOver && !Pause.IsPaused)
        {
            Move();
            Rotate();
        }
    }
    private void CheckInput()
    {
        if (moveAction.WasPressedThisFrame() || moveDownAction.WasPressedThisFrame())
        {
            moveNow = true;
            TryMove();
        }
        if (!moveAction.IsPressed() && !moveDownAction.IsPressed())
        {
            dasCounter = 0f;
            arrCounter = 0f;
            moveNow = false;
        }

        if (rotateActionX.WasPressedThisFrame())
            rotateXNow = true;
        if (rotateActionY.WasPressedThisFrame())
            rotateYNow = true;
        if (rotateActionZ.WasPressedThisFrame())
            rotateZNow = true;

        if (DropAction.WasPressedThisFrame())
        {
            Falling falling = currentShape.GetComponent<Falling>();
            falling.DropShape();
            SoundManager.PlaySound(SoundType.Place);
        }
    }
    private void Move()
    {
        if (moveNow)
        {
            if (dasCounter < dasTime)
                dasCounter += Time.deltaTime;
            else
            {
                arrCounter += Time.deltaTime;

                if (arrCounter > arrTime)
                {
                    TryMove();
                    arrCounter = 0f;
                }
            }
        }
    }
    private void Rotate()
    {
        if (!(rotateXNow || rotateYNow || rotateZNow)) return;

        int sector = cameraTracker.GetCameraSector();
        Vector3 inputRot = Vector3.zero;

        rotateXTurn = rotateActionX.ReadValue<float>();
        rotateYTurn = rotateActionY.ReadValue<float>();
        rotateZTurn = rotateActionZ.ReadValue<float>();

        if (rotateXNow)
            inputRot = new Vector3(rotateXTurn, 0, 0);
        else if (rotateYNow)
            inputRot = new Vector3(0, rotateYTurn, 0);
        else if (rotateZNow)
            inputRot = new Vector3(0, 0, rotateZTurn);

        Vector3 Rotation = RemapBySector(inputRot, sector);

        // ghost
        currentGhost.transform.rotation = Quaternion.Euler(Rotation * 90f) * currentGhost.transform.rotation;
        Physics.SyncTransforms();

        //check
        canRotate = true;
        Vector3 wallKickDirection = Vector3.zero;
        for (int i = 0; i < currentGhost.childCount; i++)
        {
            bool hit = ghostDetections[i].GetghostHitRotate();
            if (ghostDetections[i].WallHit)
            {
                wallKick = true;
                wallKickDirection += ghostDetections[i].KickDirection;
            }
            else if (hit)
            {
                FailedRotationIndicator();
                canRotate = false;
                wallKick = false;
                break;
            }
        }
        if (wallKick)
        {
            directionVector = wallKickDirection;
            TryMove();
        }
        if (!canMove && wallKick)
        {
            FailedRotationIndicator();
            canRotate = false;
        }

        wallKick = false;
        ResetWalls();
        currentGhost.transform.rotation = currentShape.transform.rotation;

        //rotation
        if (canRotate)
        {
            currentShape.transform.rotation = Quaternion.Euler(Rotation * 90f) * currentShape.transform.rotation;
            SoundManager.PlaySound(SoundType.Rotate);
        }

        rotateXNow = rotateYNow = rotateZNow = false;
    }
    private void TryMove()
    {
        canMove = true;
        bool canMoveX = false;
        bool canMoveZ = false;

        if (!wallKick)
        {
            Vector2 inputVector = VectorSign(moveAction.ReadValue<Vector2>());
            float inputDown = -moveDownAction.ReadValue<float>();
            Vector3 inputVector3 = Vector2To3(inputVector, inputDown);

            sector = cameraTracker.GetCameraSector();
            directionVector = RemapBySector(inputVector3, sector);
        }
        for (int i = 0; i < currentGhost.childCount; i++)
        {
            if (ghostDetections[i].GetghostHitMove(directionVector))
            {
                canMove = false;
                if (directionVector.x != 0 && directionVector.z != 0)
                {
                    if (!ghostDetections[i].GetghostHitMove(new Vector3(directionVector.x, 0, 0)))
                    {
                        canMoveX = true;
                        continue;
                    }
                    if (!ghostDetections[i].GetghostHitMove(new Vector3(0, 0, directionVector.z)))
                    {
                        canMoveZ = true;
                        continue;
                    }
                }
                canMoveX = false;
                canMoveZ = false;
                break;
            }
        }
        if (canMoveX ^ canMoveZ)
        {
            if (canMoveX)
                NowMove(new Vector3(directionVector.x, 0, 0));
            else if (canMoveZ)
                NowMove(new Vector3(0, 0, directionVector.z));
        }
        else if (canMove)
        {
            NowMove(directionVector);

            if (directionVector.y < 0) // if down
            {
                ScoreManager.AddDropPoints(1);
            }
        }
    }
    private void NowMove(Vector3 dir)
    {
        currentShape.transform.position += dir;
        SoundManager.PlaySound(SoundType.Move, 0.8f);
    }
    private Vector3 RemapBySector(Vector3 inputVec, int sector)
    {
        return sector switch
        {
            //front
            0 => new Vector3(-inputVec.x, inputVec.y, -inputVec.z),
            //right
            1 => new Vector3(-inputVec.z, inputVec.y, inputVec.x),
            //back
            2 => inputVec,
            //left
            3 => new Vector3(inputVec.z, inputVec.y, -inputVec.x),
            _ => Vector3.zero,
        };
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
    private void FailedRotationIndicator()
    {
        SoundManager.PlaySound(SoundType.CantRotate);
        for (int i = 0; i < currentGhost.childCount; i++)
        {
            GameObject failedRotationIndicator = Instantiate(indicatorCube, ghostcubes[i].transform.position, ghostcubes[i].transform.rotation);
            AutoDestroy IndicatorTime = failedRotationIndicator.GetComponent<AutoDestroy>();
            IndicatorTime.autoDestroytimerTime = 0.2f;
        }
    }
    private void ResetWalls()
    {
        foreach (WallKicks wall in walls)
        {
            wall.ResetHit();
        }

    }
    public void ChangeCurrentShape(GameObject newShape)
    {
        currentShape = newShape;
        GetGhost();
    }
    private Vector3 VectorSign(Vector3 vec)
    {
        float threshold = 0.35f;
        return new Vector3(
        Mathf.Abs(vec.x) > threshold ? Mathf.Sign(vec.x) : 0f,
        Mathf.Abs(vec.y) > threshold ? Mathf.Sign(vec.y) : 0f,
        Mathf.Abs(vec.z) > threshold ? Mathf.Sign(vec.z) : 0f
    );
    }
    private Vector3 Vector2To3(Vector3 vec, float down)
    {
        return new Vector3(vec.x, down, vec.y);
    }
}
