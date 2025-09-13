using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    [SerializeField] private MoveCamera moveCamera;
    public static bool isPaused;
    public static bool isSettings;
    public static bool isControls;
    private InputAction pauseAction;
    private InputAction backAction;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject controlsMenu;
    [SerializeField] private GameObject firstSelectedPause;
    [SerializeField] private GameObject firstSelectedSettings;
    private ColorAdjustments colorAdjustments;

    private void Start()
    {
        pauseAction = InputSystem.actions.FindAction("Pause");
        backAction = InputSystem.actions.FindAction("Back");
        Volume volume = FindFirstObjectByType<Volume>();
        volume.profile.TryGet(out colorAdjustments);
        colorAdjustments.postExposure.Override(PlayerPrefs.GetFloat(OptionsValues.Gamma.ToString(), 0));
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (pauseAction.WasPressedThisFrame())
        {
            if (isPaused)
            {
                if (!isSettings) GameResume();
            }
            else
            {
                GamePause();
                return;
            }
        }
        if (backAction.WasPressedThisFrame())
        {
            if (!isSettings) GameResume();
            else if (!isControls) BackButton();
            else BackButtonControls();
        }
    }

    public void GameResume()
    {
        StartCoroutine(ResumeWhenReleased());
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private IEnumerator ResumeWhenReleased()
    {
        while (Input.GetButton("Submit"))
            yield return null;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }
    private void GamePause()
    {
        if (!GridManager.gameOver)
        {
            pauseMenu.SetActive(true);
            EventSystem.current.SetSelectedGameObject(firstSelectedPause);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0f;
            isPaused = true;
        }
    }
    public void Settings()
    {
        isSettings = true;
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstSelectedSettings);
        SoundManager.PlaySound(SoundType.ClickButton);
    }
    public void Controls()
    {
        isControls = true;
        settingsMenu.SetActive(false);
        controlsMenu.SetActive(true);
        SoundManager.PlaySound(SoundType.ClickButton);
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SoundManager.PlaySound(SoundType.ClickButton); 
        LoadingScreen.sceneToLoad = "Menu";
        SceneManager.LoadScene("Loading");
    }
    public void LoadMenuDemo()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SoundManager.PlaySound(SoundType.ClickButton);
        LoadingScreen.sceneToLoad = "MenuDemo";
        SceneManager.LoadScene("Loading");
    }
    public void BackButton()
    {
        isSettings = false;
        pauseMenu.SetActive(true);
        settingsMenu.SetActive(false);
        moveCamera.SetSettingsValues();
        EventSystem.current.SetSelectedGameObject(firstSelectedPause);
        SoundManager.PlaySound(SoundType.ClickButton);
    }
    public void BackButtonControls()
    {
        isControls = false;
        settingsMenu.SetActive(true);
        controlsMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(firstSelectedSettings);
        SoundManager.PlaySound(SoundType.ClickButton);
    }
    public void QuitButton()
    {
        SoundManager.PlaySound(SoundType.ClickButton);
        Application.Quit();
    }
}
