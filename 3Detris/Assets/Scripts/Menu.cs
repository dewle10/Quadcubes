using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(LeaderboardDisplay))]
[RequireComponent(typeof(SettingsManager))]
public class Menu : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject gameModeMenu;
    [SerializeField] private GameObject leaderboardsMenu;
    [SerializeField] private GameObject controlsMenu;
    [SerializeField] private GameObject logo;
    [Header("Start Menu")]
    [SerializeField] private Button startButton;
    [SerializeField] private TMP_Text startText;
    [SerializeField] private Button[] modeButtons;
    [SerializeField] private Button[] sizeButtons;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color colorNormal;
    private bool isModeSelected = false;
    private bool isSizeSelected = false;
    [Header("First Selected")] //Back Button
    [SerializeField] private GameObject firstSelected;
    [SerializeField] private GameObject firstSelectedSettings;
    private InputAction backAction;
    private bool isOutOfMain;
    private bool isControls;

    private LeaderboardDisplay leaderboardDisplay;
    private SettingsManager settingsManager;

    private enum GameSize
    {
        Game4x4 = 4,
        Game5x5 = 5,
        Game6x6 = 6,
        Game5x5Demo = 7,
        Game4x4And = 8,
        Game5x5And = 9,
        Game6x6And = 10,
    }
    private GameSize gameSize;
    private void Awake()
    {
        leaderboardDisplay = GetComponent<LeaderboardDisplay>();
        settingsManager = GetComponent<SettingsManager>();
        backAction = InputSystem.actions.FindAction("Back");
    }
    private void Start()
    {
#if UNITY_STANDALONE
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        EventSystem.current.SetSelectedGameObject(firstSelected);
        settingsManager.SetResolution();
#endif
    }
    private void OnEnable()
    {
        backAction.Enable();
        backAction.performed += OnBackPerformed;
        EventSystem.current.SetSelectedGameObject(firstSelected);
    }
    private void OnDisable()
    {
        backAction.performed -= OnBackPerformed;
        backAction.Disable();
    }
    private void OnBackPerformed(InputAction.CallbackContext ctx)
    {
        if (isOutOfMain) BackButton();
        else if (isControls) BackButtonControls();
    }

    #region Buttons
    public void PlayButton()
    {
        isOutOfMain = true;
        mainMenu.SetActive(false);
        logo.SetActive(false);
        gameModeMenu.SetActive(true);
        SoundManager.PlaySound(SoundType.ClickButton);
    }
    public void SettingsButton()
    {
        isOutOfMain = true;
        mainMenu.SetActive(false);
        logo.SetActive(false);
        settingsMenu.SetActive(true);
        SoundManager.PlaySound(SoundType.ClickButton);
    }
    public void LeaderboardsButton()
    {
        isOutOfMain = true;
        mainMenu.SetActive(false);
        logo.SetActive(false);
        leaderboardsMenu.SetActive(true);
        leaderboardDisplay.Refresh();
    }
    public void OpenURLButton(string url)
    {
        Application.OpenURL(url);
        SoundManager.PlaySound(SoundType.ClickButton);
    }
    public void ControlsButton()
    {
        isControls = true;
        isOutOfMain = false;
        settingsMenu.SetActive(false);
        controlsMenu.SetActive(true);
        SoundManager.PlaySound(SoundType.ClickButton);
    }
    public void BackButton()
    {
        isOutOfMain = false;
        mainMenu.SetActive(true);
        logo.SetActive(true);
        gameModeMenu.SetActive(false);
        settingsMenu.SetActive(false);
        leaderboardsMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(firstSelected);
        SoundManager.PlaySound(SoundType.ClickButton);
    }
    public void BackButtonControls()
    {
        isControls = false;
        isOutOfMain = true;
        settingsMenu.SetActive(true);
        controlsMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(firstSelectedSettings);
        SoundManager.PlaySound(SoundType.ClickButton);
    }
    public void GameSizeButton(int size)
    {
        gameSize = (GameSize)size;
        GridManager.gameWidth = size;
        SoundManager.PlaySound(SoundType.ClickButton);

        isSizeSelected = true;
        HighlightSelected(sizeButtons);
        CheckIfReady();
    }
    public void LbModeChallButton()
    {
        LeaderboardDisplay.displayMode = GameMode.Challange;
        leaderboardDisplay.Refresh();
    }
    public void LbModeCasButton()
    {
        LeaderboardDisplay.displayMode = GameMode.Casual;
        leaderboardDisplay.Refresh();
    }
    public void StartButton()
    {
        SoundManager.PlaySound(SoundType.ClickButton);
        LoadingScreen.sceneToLoad = gameSize.ToString();
        SceneManager.LoadScene("Loading");
    }
    public void CasualButton()
    {
        ScoreManager.gameMode = GameMode.Casual;
        SoundManager.PlaySound(SoundType.ClickButton);

        isModeSelected = true;
        HighlightSelected(modeButtons);
        CheckIfReady();
    }
    public void ChallangeButton()
    {
        ScoreManager.gameMode = GameMode.Challange;
        SoundManager.PlaySound(SoundType.ClickButton);

        isModeSelected = true;
        HighlightSelected(modeButtons);
        CheckIfReady();
    }
    public void QuitButton()
    {
        SoundManager.PlaySound(SoundType.ClickButton);
        Application.Quit();
    }
    #endregion

    private void HighlightSelected(Button[] buttons)
    {
        foreach (Button btn in buttons)
        {
            ColorBlock colors = btn.colors;
            colors.normalColor = colorNormal;
            btn.colors = colors;
        }

        Button clickedButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        ColorBlock clickedColors = clickedButton.colors;
        clickedColors.normalColor = selectedColor;
        clickedButton.colors = clickedColors;
    }
    private void CheckIfReady()
    {
        bool ready = (isModeSelected && isSizeSelected);
        if(ready)
            ActivateStart();
    }

    private void ActivateStart()
    {
        startButton.interactable = true;
        Color c = startText.color;
        c.a = 1f;
        startText.color = c;
    }
}
