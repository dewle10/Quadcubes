using System.Collections;
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
    [SerializeField] private float padLookSpeed;
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
    private bool isStartPause;
    [Header("Invert")]
    [SerializeField] private bool invertX;
    [SerializeField] private bool invertY;
    private float invertXValue;
    private float invertYValue;

    private float targetAngleX, targetAngleY, targetDistance;
    private float smoothAngleX, smoothAngleY, smoothDistance;
    private float angleXVelocity, angleYVelocity, distanceVelocity;
    //Inputs
    private PlayerInput playerInput;
    private InputAction lookAction, zoomAction;
    private InputAction lookHorAction, lookVerAction, zoomInOutAction;
    private bool usingPad;

    private void Awake()
    {
        playerInput = gameObject.GetComponent<PlayerInput>();
        lookAction = playerInput.actions["Look"];
        zoomAction = playerInput.actions["ZoomScroll"];
        lookHorAction = playerInput.actions["Look Horizontal"];
        lookVerAction = playerInput.actions["Look Vertical"];
        zoomInOutAction = playerInput.actions["Zoom"];
    }
    private void Start()
    {
        targetDistance = Vector3.Distance(pivotPoint.position, cam.transform.position);
        smoothDistance = targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);

        targetAngleX = smoothAngleX = Mathf.Clamp(-startAngleX, minAngleX, maxAngleX);
        targetAngleY = smoothAngleY = startAngleY - 180f;

        Quaternion rot = Quaternion.Euler(smoothAngleX, smoothAngleY, 0f);
        Vector3 startOffset = rot * Vector3.forward * smoothDistance;
        cam.transform.position = pivotPoint.position + startOffset;
        cam.transform.LookAt(pivotPoint.position);

        SetSettingsValues();

        StartCoroutine(StartPause());
    }
    
    private void OnEnable()
    {
        lookAction.Enable();
        zoomAction.Enable();
        lookHorAction.Enable();
        lookVerAction.Enable();
        zoomInOutAction.Enable();
        playerInput.onControlsChanged += OnControlsChanged;
    }
    private void OnDisable()
    {
        lookAction.Disable();
        zoomAction.Disable();
        lookHorAction.Disable();
        lookVerAction.Disable();
        zoomInOutAction.Disable();
        playerInput.onControlsChanged -= OnControlsChanged;
    }
    private void Update()
    {
        //Zoom target
        float zoomInput = zoomAction.ReadValue<Vector2>().y - zoomInOutAction.ReadValue<float>() * 0.2f;
        if (Mathf.Abs(zoomInput) > 0.01f)
        {
            targetDistance = Mathf.Clamp(
                targetDistance - zoomInput * zoomSpeed * Time.deltaTime,
                minDistance, maxDistance
            );
        }
        //Rotation target
        if (!isStartPause)
        {
            Vector2 delta = lookAction.ReadValue<Vector2>();
            if (usingPad)
            {
                delta *= padLookSpeed;
            }
            targetAngleY += delta.x * horizontalSpeed * Time.deltaTime * invertXValue;
            targetAngleX = Mathf.Clamp(targetAngleX + delta.y * verticalSpeed * Time.deltaTime * invertYValue, minAngleX, maxAngleX);
        }
        //Rotation On Buttons
        float lookLRDir = lookHorAction.ReadValue<float>();
        if (lookHorAction.triggered)
        {
            targetAngleY += 90f * lookLRDir;
        }
        float lookDUDir = lookVerAction.ReadValue<float>();
        if (lookVerAction.triggered)
        {
            targetAngleX = Mathf.Clamp(targetAngleX + 30f * lookDUDir, minAngleX, maxAngleX);
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

    private void OnControlsChanged(PlayerInput input)
    {
        string scheme = input.currentControlScheme;
        usingPad = scheme == "Gamepad";
    }
    public void SetSettingsValues()
    {
        horizontalSpeed = PlayerPrefs.GetFloat(nameof(OptionsValues.SensitivityHor), 15);
        verticalSpeed = PlayerPrefs.GetFloat(nameof(OptionsValues.SensitivityVer), 10);
        padLookSpeed = PlayerPrefs.GetFloat(nameof(OptionsValues.SensitivityPad), 10);
        zoomSpeed = PlayerPrefs.GetFloat(nameof(OptionsValues.SensitivityZoom), 90);

        invertX = PlayerPrefs.GetInt(nameof(OptionsValues.InvertX), 0) == 1;
        invertY = PlayerPrefs.GetInt(nameof(OptionsValues.InvertY), 0) == 1;
        invertXValue = invertX ? -1f : 1f;
        invertYValue = invertY ? -1f : 1f;
    }
    private IEnumerator StartPause()
    {
        isStartPause = true;
        yield return new WaitForSeconds(1f);
        isStartPause = false;
    }
}

