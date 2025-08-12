using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Pause : MonoBehaviour
{
    public static bool isPaused;
    private InputAction pauseAction;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject firstSelected;

    private void Start()
    {
        pauseAction = InputSystem.actions.FindAction("Pause");
    }

    private void Update()
    {
        if (pauseAction.WasPressedThisFrame())
        {
            if (isPaused)
            {
                GameResume();
            }
            else
            {
                GamePause();
            }
        }
    }

    public void GameResume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }
    private void GamePause()
    {
        pauseMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstSelected);
        Time.timeScale = 0f;
        isPaused = true;
    }
    public void Settings()
    {
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(true);
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
    public void BackButton()
    {
        pauseMenu.SetActive(true);
        settingsMenu.SetActive(false);
        SoundManager.PlaySound(SoundType.ClickButton);
    }
}
