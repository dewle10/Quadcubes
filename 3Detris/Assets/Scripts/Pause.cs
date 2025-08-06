using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public static bool isPaused;
    private InputAction pauseAction;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingsMenu;

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
        SoundManager.PlaySound(SoundType.ClickButton);
        SceneManager.LoadScene(0);
    }
    public void BackButton()
    {
        pauseMenu.SetActive(true);
        settingsMenu.SetActive(false);
        SoundManager.PlaySound(SoundType.ClickButton);
    }
}
