using UnityEngine;
using UnityEngine.InputSystem;

public class ShapeDispenser : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] shapes;
    public GameObject[] displayShapes;
    public GameObject[] holdShapes;

    [Header("References")]
    [SerializeField] private GameObject cam;
    [SerializeField] private Transform shapeSpawnPos;
    [SerializeField] private Transform holdShapePos;
    [SerializeField] private Transform nextShapePos;

    private MoveShape playerController;

    private GameObject currentShape;
    private GameObject holdShape;
    private GameObject nextShape;

    private int currentShapeIndex;
    private int nextShapeIndex = -1;
    private int previousShapeIndex = -1;
    private int holdShapeIndex = -1;

    private InputAction holdAction;
    private bool holding;

    private void Awake()
    {
        playerController = FindFirstObjectByType<MoveShape>();
        holdAction = InputSystem.actions.FindAction("Hold");
    }
    private void Start()
    {
        nextShapeIndex = RandomShapeIndex();
        SpawnShape(false);
    }
    private void OnEnable()
    {
        holdAction.Enable();
    }
    private void OnDisable()
    {
        holdAction.Disable();
    }
    private void Update()
    {
        HoldShape();
    }
    public int RandomShapeIndex()
    {
        int Index = Random.Range(0, shapes.Length); 
        while (Index == previousShapeIndex && previousShapeIndex == currentShapeIndex)
        {
            Index = Random.Range(0, shapes.Length);
        }
        return Index;
    }
    public void SpawnShape(bool isHoldShape)
    {
        GridManager.LossCheck();
        if (!GridManager.gameOver)
        {
            if (!isHoldShape)
            {
                previousShapeIndex = currentShapeIndex;
                currentShapeIndex = nextShapeIndex;
                nextShapeIndex = RandomShapeIndex();

                if(nextShape != null)
                    Destroy(nextShape);
                nextShape = Instantiate(displayShapes[nextShapeIndex], cam.transform);
                nextShape.transform.position = nextShapePos.position;
            }
            else
            {
                Destroy(holdShape);
                (currentShapeIndex, holdShapeIndex) = (holdShapeIndex, currentShapeIndex);
                holdShape = Instantiate(holdShapes[holdShapeIndex], cam.transform);
                holdShape.transform.position = holdShapePos.position;

            }
            currentShape = Instantiate(shapes[currentShapeIndex], shapeSpawnPos.position, Quaternion.identity);
            playerController.ChangeCurrentShape(currentShape);
        }
    }
    private void HoldShape()
    {
        if (holdAction.WasPressedThisFrame() && !GridManager.gameOver && !Pause.IsPaused)
        {
            if (!holding)
            {
                holding = true;
                currentShape.GetComponent<Falling>().DestroyShape();
                if (holdShapeIndex >= 0) 
                {
                    SpawnShape(true);
                }
                else //first time
                {
                    holdShapeIndex = currentShapeIndex;
                    holdShape = Instantiate(holdShapes[holdShapeIndex], cam.transform);
                    holdShape.transform.position = holdShapePos.position;
                    SpawnShape(false);
                }
                SoundManager.PlaySound(SoundType.Hold, 1.2f);
            }
            else
            {
                SoundManager.PlaySound(SoundType.CantHold);
            }

        }
    }
    public void ResetHold() //on place
    {
        holding = false;
    }
}
