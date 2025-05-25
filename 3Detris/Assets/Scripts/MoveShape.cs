using UnityEngine;
using UnityEngine.InputSystem;

public class MoveShape : MonoBehaviour
{
    public InputActionAsset inputAction;

    private InputAction moveAction;
    private InputAction rotateAction;
    private Vector3 vector;
    public float rotateTurn;

    public GameObject currentShape;

    private void Awake()
    {
        moveAction = InputSystem.actions.FindAction("move");
        rotateAction = InputSystem.actions.FindAction("Rotate");
    }
    void Update()
    {
        vector = moveAction.ReadValue<Vector3>();
        rotateTurn = rotateAction.ReadValue<float>();

        if (moveAction.WasPressedThisFrame())
        {
            currentShape.transform.position += vector;
        }
        Rotate();
    }

    private void Rotate()
    {
        if (rotateAction.WasPressedThisFrame())
        {
            if(rotateTurn >= 0)
                currentShape.transform.rotation = Quaternion.Euler(0,0,90) * currentShape.transform.rotation;
            else if(rotateTurn <= 0)
                currentShape.transform.rotation = Quaternion.Euler(0, 0, -90) * currentShape.transform.rotation;
        }
        
    }
}
