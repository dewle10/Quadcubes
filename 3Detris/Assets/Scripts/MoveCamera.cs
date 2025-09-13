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
    //Inputs
    private PlayerInput playerInput;
    private InputAction lookAction, zoomAction;
    private InputAction lookLRAction, lookDUAction, zoomInOutAction;
    private bool usingPad;
    //Targets
    private float targetAngleX, targetAngleY, targetDistance;
    //Smoothed
    private float smoothAngleX, smoothAngleY, smoothDistance;
    //Velocities for SmoothDamp
    private float angleXVelocity, angleYVelocity, distanceVelocity;

    private void Awake()
    {
        playerInput = gameObject.GetComponent<PlayerInput>();
        lookAction = playerInput.actions["Look"];
        zoomAction = playerInput.actions["ZoomScroll"];
        lookLRAction = playerInput.actions["Look Horizontal"];
        lookDUAction = playerInput.actions["Look Vertical"];
        zoomInOutAction = playerInput.actions["Zoom"];
    }
    private void Start()
    {
        targetDistance = Vector3.Distance(pivotPoint.position, cam.transform.position);
        smoothDistance = targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);

        float sAngleX = -startAngleX;
        float sAngleY = startAngleY - 180f;
        startAngleX = Mathf.Clamp(sAngleX, minAngleX, maxAngleX);
        targetAngleX = smoothAngleX = sAngleX;
        targetAngleY = smoothAngleY = sAngleY;

        Quaternion rot = Quaternion.Euler(smoothAngleX, smoothAngleY, 0f);
        Vector3 startOffset = rot * Vector3.forward * smoothDistance;
        cam.transform.position = pivotPoint.position + startOffset;
        cam.transform.LookAt(pivotPoint.position);

        SetSettingsValues();

        StartCoroutine(StartPasue());
    }
    private void OnEnable()
    {
        lookAction.Enable();
        zoomAction.Enable();
        lookLRAction.Enable();
        lookDUAction.Enable();
        zoomInOutAction.Enable();
        playerInput.onControlsChanged += OnControlsChanged;
    }
    private void OnDisable()
    {
        lookAction.Disable();
        zoomAction.Disable();
        lookLRAction.Disable();
        lookDUAction.Disable();
        zoomInOutAction.Disable();
        playerInput.onControlsChanged -= OnControlsChanged;
    }
    private void Update()
    {
        OnControlsChanged(playerInput);

        //Zoom target
        float scroll = zoomAction.ReadValue<Vector2>().y;
        if (Mathf.Abs(scroll) > 0.01f)
        {
            targetDistance -= scroll * zoomSpeed * Time.deltaTime;
            targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);
        }

        float zoomDir = zoomInOutAction.ReadValue<float>();
        if (zoomInOutAction.IsPressed())
            targetDistance = Mathf.Clamp(targetDistance + zoomSpeed * zoomDir * Time.deltaTime * 0.2f, minDistance, maxDistance);

        //Rotation target
        if (!isStartPause)
        {
            Vector2 delta = lookAction.ReadValue<Vector2>();
            if (usingPad)
            {
                delta *= padLookSpeed;
            }

            float invertXValue = invertX ? -1f : 1f;
            float invertYValue = invertY ? -1f : 1f;

            targetAngleY += delta.x * horizontalSpeed * Time.deltaTime * invertXValue;
            targetAngleX = Mathf.Clamp(targetAngleX + delta.y * verticalSpeed * Time.deltaTime * invertYValue, minAngleX, maxAngleX);
        }
        //Rotation On Buttons
        float lookLRDir = lookLRAction.ReadValue<float>();
        if (lookLRAction.triggered)
        {
            targetAngleY += 90f * lookLRDir;
        }
        float lookDUDir = lookDUAction.ReadValue<float>();
        if (lookDUAction.triggered)
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
        horizontalSpeed = PlayerPrefs.GetFloat(OptionsValues.SensitivityHor.ToString(), 15);
        verticalSpeed = PlayerPrefs.GetFloat(OptionsValues.SensitivityVer.ToString(), 10);
        padLookSpeed = PlayerPrefs.GetFloat(OptionsValues.SensitivityPad.ToString(), 10);
        zoomSpeed = PlayerPrefs.GetFloat(OptionsValues.SensitivityZoom.ToString(), 90);

        invertX = PlayerPrefs.GetInt(OptionsValues.InvertX.ToString(), 0) == 1;
        invertY = PlayerPrefs.GetInt(OptionsValues.InvertY.ToString(), 0) == 1;
    }
    private IEnumerator StartPasue()
    {
        isStartPause = true;
        yield return new WaitForSeconds(1f);
        isStartPause = false;
    }
}

