using UnityEngine;
using UnityEngine.InputSystem;

public class ShapeDispenser : MonoBehaviour
{
    public GameObject[] shapes;
    public GameObject[] displayShapes;
    private MoveShape playerController;
    private GameObject currentShape;
    private GameObject holdShape;
    private GameObject nextShape;
    private int currentShapenum;
    private int nextShapenum = -1;
    private int previousShapenum = -1;
    private int holdShapenum = -1;
    [SerializeField] private GameObject cam;
    [SerializeField] private Transform shapeSpawnPos;
    [SerializeField] private Transform holdShapePos;
    [SerializeField] private Transform nextShapePos;

    private InputAction holdAction;
    private bool holding;

    private void Awake()
    {
        playerController = FindFirstObjectByType<MoveShape>();
        holdAction = InputSystem.actions.FindAction("Hold");
    }
    private void Start()
    {
        nextShapenum = RandomShapeNum();
        SpawnShape(false);
    }
    private void Update()
    {
        HoldShape();
    }
    public int RandomShapeNum()
    { 
        int num = 0;
        num = Random.Range(0, shapes.Length); 
        while (num == previousShapenum && previousShapenum == currentShapenum)
        {
            num = Random.Range(0, shapes.Length);
        }
        return num;
    }
    public void SpawnShape(bool isHoldShape)
    {
        if (!isHoldShape)
        {
            previousShapenum = currentShapenum;
            currentShapenum = nextShapenum;
            nextShapenum = RandomShapeNum();

            if(nextShape != null)
                Destroy(nextShape);
            nextShape = Instantiate(displayShapes[nextShapenum], cam.transform);
            nextShape.transform.position = nextShapePos.position;
        }
        else
        {
            Destroy(holdShape);
            int holdNum = holdShapenum;
            holdShapenum = currentShapenum;
            currentShapenum = holdNum;
            holdShape = Instantiate(displayShapes[holdShapenum], cam.transform);
            holdShape.transform.position = holdShapePos.position;

        }
            currentShape = Instantiate(shapes[currentShapenum], shapeSpawnPos.position, Quaternion.identity);
            playerController.ChangeCurrentShape(currentShape);
    }
    private void HoldShape()
    {
        if (holdAction.WasPressedThisFrame() && !holding)
        {
            holding = true;
            currentShape.GetComponent<Falling>().DestroyShape();
            if (holdShapenum >= 0) 
            {
                SpawnShape(true);
            }
            else //first time
            {
                holdShapenum = currentShapenum;
                holdShape = Instantiate(displayShapes[holdShapenum], cam.transform);
                holdShape.transform.position = holdShapePos.position;
                SpawnShape(false);
            }
        }
    }
    public void ResetHold()
    {
        holding = false;
    }
}
