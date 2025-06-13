using UnityEngine;

public class ShapeDispenser : MonoBehaviour
{
    public GameObject[] Shapes;
    private MoveShape playerController;
    private GameObject currentShape;
    [SerializeField] private Transform shapeSpawnPos;

    private void Awake()
    {
        playerController = FindFirstObjectByType<MoveShape>();
        SpawnShape();
    }

    private void Update()
    {
        
    }

    private int RandomShapeNum()
    {
        int randomShapeNum;
        randomShapeNum = Random.Range(0, Shapes.Length);
        return randomShapeNum;
    }

    public void SpawnShape()
    {
        currentShape = Instantiate(Shapes[RandomShapeNum()], shapeSpawnPos.position, Quaternion.identity);
        playerController.ChangeCurrentShape(currentShape);
    }
}
