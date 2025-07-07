using UnityEngine;
using UnityEngine.InputSystem;

public class MoveCamera : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform pivotPoint;
    [SerializeField] private Camera cam;
    [Header("Speeds")]
    [SerializeField] private float horizontalSpeed;
    [SerializeField] private float verticalSpeed;
    [SerializeField] private float zoomSpeed;
    [Header("Limits")]
    [SerializeField] private float minAngleX;
    [SerializeField] private float maxAngleX;
    [SerializeField] private float minDistance;
    [SerializeField] private float maxDistance;
    [Header("Smoothing")]
    [SerializeField] private float rotationSmoothTime;
    [SerializeField] private float zoomSmoothTime;
    [Header("Start")]
    [SerializeField] private float startAngleX;
    [SerializeField] private float startAngleY;
    //Inputs
    private InputAction lookAction, mmbAction, zoomAction;
    //Targets
    [SerializeField] private float targetAngleX, targetAngleY, targetDistance;
    //Smoothed
    [SerializeField] private float smoothAngleX, smoothAngleY, smoothDistance;
    //Velocities for SmoothDamp
    private float angleXVelocity, angleYVelocity, distanceVelocity;

    private void Awake()
    {
        lookAction = InputSystem.actions.FindAction("Look");
        mmbAction = InputSystem.actions.FindAction("MMB");
        zoomAction = InputSystem.actions.FindAction("Zoom");

        targetDistance = Vector3.Distance(pivotPoint.position, cam.transform.position);
        smoothDistance = targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);

        float sAngleX = -startAngleX;
        float sAngleY = startAngleY-180f;
        startAngleX = Mathf.Clamp(sAngleX, minAngleX, maxAngleX);
        targetAngleX = smoothAngleX = sAngleX;
        targetAngleY = smoothAngleY = sAngleY;

        Quaternion rot = Quaternion.Euler(smoothAngleX, smoothAngleY, 0f);
        Vector3 startOffset = rot * Vector3.forward * smoothDistance;
        cam.transform.position = pivotPoint.position + startOffset;
        cam.transform.LookAt(pivotPoint.position);
    }
    private void OnEnable()
    {
        lookAction.Enable();
        mmbAction.Enable();
        zoomAction.Enable();
    }
    private void OnDisable()
    {
        lookAction.Disable();
        mmbAction.Disable();
        zoomAction.Disable();
    }
    private void Update()
    {
        //Zoom target
        float scroll = zoomAction.ReadValue<Vector2>().y;
        if (Mathf.Abs(scroll) > 0.01f)
        {
            targetDistance -= scroll * zoomSpeed * Time.deltaTime;
            targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);
        }

        //Rotation target
        if (mmbAction.IsPressed())
        {
            Vector2 delta = lookAction.ReadValue<Vector2>();
            targetAngleY += delta.x * horizontalSpeed * Time.deltaTime;
            targetAngleX = Mathf.Clamp(targetAngleX + delta.y * verticalSpeed * Time.deltaTime, minAngleX, maxAngleX);
        }

        //SmoothDamp to targets
        smoothAngleY = Mathf.SmoothDampAngle(smoothAngleY, targetAngleY, ref angleYVelocity, rotationSmoothTime);
        smoothAngleX = Mathf.SmoothDampAngle(smoothAngleX, targetAngleX, ref angleXVelocity, rotationSmoothTime);
        smoothDistance = Mathf.SmoothDamp(smoothDistance, targetDistance, ref distanceVelocity, zoomSmoothTime);
    }

    private void LateUpdate()
    {
        Quaternion rot = Quaternion.Euler(smoothAngleX, smoothAngleY, 0f);
        Vector3 offset = rot * Vector3.forward * smoothDistance;
        cam.transform.position = pivotPoint.position + offset;

        cam.transform.LookAt(pivotPoint.position);
    }
}
