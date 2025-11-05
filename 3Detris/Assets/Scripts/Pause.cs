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
    public static bool IsPaused { private set; get; }
    public static bool IsSettings { private set; get; }
    public static bool IsControls { private set; get; }
    private InputAction pauseAction;
    private InputAction backAction;
    private InputAction dropAction;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject controlsMenu;
    [SerializeField] private GameObject firstSelectedPause;
    [SerializeField] private GameObject firstSelectedSettings;

    private void Start()
    {
        pauseAction = InputSystem.actions.FindAction("Pause");
        backAction = InputSystem.actions.FindAction("Back");
        dropAction = InputSystem.actions.FindAction("Drop");
#if !UNITY_ANDROID
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
#endif
    }

    private void Update()
    {
        if (pauseAction.WasPressedThisFrame())
        {
            if (IsPaused)
            {
                if (!IsSettings) GameResume();
            }
            else
            {
                GamePause();
                return;
            }
        }
        if (backAction.WasPressedThisFrame())
        {
            if (!IsSettings) GameResume();
            else if (!IsControls) BackButton();
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
        while (dropAction.IsPressed())
            yield return null;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        IsPaused = false;
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
            IsPaused = true;
        }
    }
    public void Settings()
    {
        IsSettings = true;
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstSelectedSettings);
        SoundManager.PlaySound(SoundType.ClickButton);
    }
    public void Controls()
    {
        IsControls = true;
        settingsMenu.SetActive(false);
        controlsMenu.SetActive(true);
        SoundManager.PlaySound(SoundType.ClickButton);
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        SoundManager.PlaySound(SoundType.ClickButton); 
        LoadingScreen.sceneToLoad = "Menu";
        SceneManager.LoadScene("Loading");
    }
    public void LoadMenuDemo()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        SoundManager.PlaySound(SoundType.ClickButton);
        LoadingScreen.sceneToLoad = "MenuDemo";
        SceneManager.LoadScene("Loading");
    }
    public void BackButton()
    {
        IsSettings = false;
        pauseMenu.SetActive(true);
        settingsMenu.SetActive(false);
        moveCamera.SetSettingsValues();
        EventSystem.current.SetSelectedGameObject(firstSelectedPause);
        SoundManager.PlaySound(SoundType.ClickButton);
    }
    public void BackButtonControls()
    {
        IsControls = false;
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
