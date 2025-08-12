using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum OptionsValues{
    SensitivityVer,
    SensitivityHor,
    SensitivityPad,
    VolumeMusic,
    VolumeSounds,
    Quality,
    ScreenMode,
    Resolution
}

public class Menu : MonoBehaviour
{
    private LeaderboardDisplay leaderboardDisplay;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject gameModeMenu;
    [SerializeField] private GameObject leaderbordsMenu;
    [SerializeField] private GameObject logo;
    [SerializeField] private AudioMixer soundsMixer;
    [SerializeField] private AudioMixer musicMixer;

    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private TMP_Dropdown screenModeDropdown;
    [SerializeField] private Slider soundsSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider senHorSlider;
    [SerializeField] private Slider senVerSlider;
    [SerializeField] private Slider senPadSlider;

    [SerializeField] private TMP_Text soundsText;
    [SerializeField] private TMP_Text musicText;
    [SerializeField] private TMP_Text senHorText;
    [SerializeField] private TMP_Text senVerText;
    [SerializeField] private TMP_Text senPadText;

    [SerializeField] private Button startButton;
    [SerializeField] private TMP_Text startText;
    [SerializeField] private Button[] modeButtons;
    [SerializeField] private Button[] sizeButtons;

    [SerializeField] private Color selectedColor;
    [SerializeField] private Color normalColor;

    [SerializeField] private GameObject firstSelected;

    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions;

    private float currentRefreshRate;
    private int currentResolutionIndex;
    private bool isModeSelected = false;
    private bool isSizeSelected = false;

    private enum GameSize
    {
        Game4x4 = 4,
        Game5x5 = 5,
        Game6x6 = 6
    }
    private GameSize gameSize;
    private void Awake()
    {
        leaderboardDisplay = GetComponent<LeaderboardDisplay>();
    }

    private void Start()
    {
        ResolutionStart(); //defoult: hightest Resolution

        soundsSlider.value = PlayerPrefs.GetFloat(OptionsValues.VolumeSounds.ToString(), 0); 
        musicSlider.value = PlayerPrefs.GetFloat(OptionsValues.VolumeMusic.ToString(), 0);

        senHorSlider.value = PlayerPrefs.GetFloat(OptionsValues.SensitivityHor.ToString(), 15);
        senVerSlider.value = PlayerPrefs.GetFloat(OptionsValues.SensitivityVer.ToString(), 10);
        senPadSlider.value = PlayerPrefs.GetFloat(OptionsValues.SensitivityPad.ToString(), 10);

        qualityDropdown.value = PlayerPrefs.GetInt(OptionsValues.Quality.ToString(), 2); //defoult: High
        screenModeDropdown.value = PlayerPrefs.GetInt(OptionsValues.ScreenMode.ToString(), 1); //defoult: fullscreen Window

        soundsText.text = (soundsSlider.value * 100).ToString("0.");
        musicText.text = (musicSlider.value * 100).ToString("0.");

        senHorText.text = senHorSlider.value.ToString("0.");
        senVerText.text = senVerSlider.value.ToString("0.");
        senPadText.text = senPadSlider.value.ToString("0.");

    }
    void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(firstSelected);
    }

    public void PlayButton()
    {
        mainMenu.SetActive(false);
        logo.SetActive(false);
        gameModeMenu.SetActive(true);
        SoundManager.PlaySound(SoundType.ClickButton);
    }
    public void SettingsButton()
    {
        mainMenu.SetActive(false);
        logo.SetActive(false);
        settingsMenu.SetActive(true);
        SoundManager.PlaySound(SoundType.ClickButton);
    }
    public void LeaderboardsButton()
    {
        mainMenu.SetActive(false);
        logo.SetActive(false);
        leaderbordsMenu.SetActive(true);
        SoundManager.PlaySound(SoundType.ClickButton);
    }
    public void BackButton()
    {
        mainMenu.SetActive(true);
        logo.SetActive(true);
        gameModeMenu.SetActive(false);
        settingsMenu.SetActive(false);
        leaderbordsMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(firstSelected);
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
        SoundManager.PlaySound(SoundType.ClickButton);
        leaderboardDisplay.Refresh();
    }
    public void LbModeCasButton()
    {
        LeaderboardDisplay.displayMode = GameMode.Casual;
        SoundManager.PlaySound(SoundType.ClickButton);
        leaderboardDisplay.Refresh();
    }
    public void StartButton()
    {
        SoundManager.PlaySound(SoundType.ClickButton);
        LoadingScreen.sceneToLoad = Enum.GetName(typeof(GameSize), (int)gameSize);
        SceneManager.LoadScene("Loading");
    }
    public void CasualButton()
    {
        LinePoints.gameMode = GameMode.Casual;
        SoundManager.PlaySound(SoundType.ClickButton);

        isModeSelected = true;
        HighlightSelected(modeButtons);
        CheckIfReady();
    }
    public void ChallangeButton()
    {
        LinePoints.gameMode = GameMode.Challange;
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

    public void SetVolumeSounds(float volume)
    {
        soundsMixer.SetFloat("Volume", Mathf.Log10(volume) * 20);
        soundsText.text = (soundsSlider.value * 100).ToString("0.");
        PlayerPrefs.SetFloat(OptionsValues.VolumeSounds.ToString(), volume);
    }
    public void SetVolumeMusic(float volume)
    {
        musicMixer.SetFloat("Volume", Mathf.Log10(volume) * 20);
        musicText.text = (musicSlider.value * 100).ToString("0.");
        PlayerPrefs.SetFloat(OptionsValues.VolumeMusic.ToString(), volume);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt(OptionsValues.Quality.ToString(), qualityIndex);
    }

    public void SetScreenMode(int screenMode)
    {
        Screen.fullScreenMode = (FullScreenMode)screenMode;
        PlayerPrefs.SetInt(OptionsValues.ScreenMode.ToString(), screenMode);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, (FullScreenMode)PlayerPrefs.GetInt(OptionsValues.ScreenMode.ToString(), 1));
        PlayerPrefs.SetInt(OptionsValues.Resolution.ToString(), resolutionIndex);
    }

    public void SetSensitivityHorizontal(float Sensitivity)
    {
        senHorText.text = senHorSlider.value.ToString("0.");
        PlayerPrefs.SetFloat(OptionsValues.SensitivityHor.ToString(), Sensitivity);
    }
    public void SetSensitivityVertical(float Sensitivity)
    {
        senVerText.text = senVerSlider.value.ToString("0.");
        PlayerPrefs.SetFloat(OptionsValues.SensitivityVer.ToString(), Sensitivity);
    }
    public void SetSensitivityGamepad(float Sensitivity)
    {
        senPadText.text = senPadSlider.value.ToString("0.");
        PlayerPrefs.SetFloat(OptionsValues.SensitivityPad.ToString(), Sensitivity);
    }

    private void HighlightSelected(Button[] buttons)
    {
        foreach (Button btn in buttons)
        {
            ColorBlock colors = btn.colors;
            colors.normalColor = normalColor;
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
    public void SelectBack(GameObject backButton)
    {
        EventSystem.current.SetSelectedGameObject(backButton);
    }

    private void ResolutionStart()
    {
        resolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();
        resolutionDropdown.ClearOptions();
        currentRefreshRate = (float)Screen.currentResolution.refreshRateRatio.value;

        for (int i = 0; i < resolutions.Length; i++)
        {
            if ((float)resolutions[i].refreshRateRatio.value == currentRefreshRate)
            {
                filteredResolutions.Add(resolutions[i]);
            }
        }

        filteredResolutions.Sort((a, b) => {
            if (a.width != b.width)
                return b.width.CompareTo(a.width);
            else
                return b.height.CompareTo(a.height);
        });

        List<string> options = new List<string>();

        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            string resolutionOption = filteredResolutions[i].width + "x" + filteredResolutions[i].height;
            // + " " + filteredResolutions[i].refreshRateRatio.value.ToString("0.##") + " Hz";

            options.Add(resolutionOption);

            if (filteredResolutions[i].width == Screen.width && filteredResolutions[i].height == Screen.height &&
                (float)filteredResolutions[i].refreshRateRatio.value == currentRefreshRate)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex = PlayerPrefs.GetInt(OptionsValues.Resolution.ToString(), 0);
        resolutionDropdown.RefreshShownValue();

        Resolution resolution = filteredResolutions[currentResolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, (FullScreenMode)PlayerPrefs.GetInt(OptionsValues.ScreenMode.ToString(), 1));
    }
}
