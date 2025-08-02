using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions;

    private float currentRefreshRate;
    private int currentResolutionIndex;

    private enum GameSize
    {
        Game4x4 = 4,
        Game5x5 = 5,
        Game6x6 = 6
    }
    private GameSize gameSize;

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

        soundsText.text = (80 + soundsSlider.value).ToString("0.");
        musicText.text = (80 + musicSlider.value).ToString("0.");
        senHorText.text = senHorSlider.value.ToString("0.");
        senVerText.text = senVerSlider.value.ToString("0.");
        senPadText.text = senPadSlider.value.ToString("0.");

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
    public void BackButton()
    {
        mainMenu.SetActive(true);
        logo.SetActive(true);
        gameModeMenu.SetActive(false);
        settingsMenu.SetActive(false);
        leaderbordsMenu.SetActive(false);
        SoundManager.PlaySound(SoundType.ClickButton);
    }
    public void GameSizeButton(int size)
    {
        gameSize = (GameSize)size;
        GridManager.gameWidth = size;
        SoundManager.PlaySound(SoundType.ClickButton);
    }
    public void StartButton()
    {
        SceneManager.LoadScene(Enum.GetName(typeof(GameSize),(int)gameSize));
        SoundManager.PlaySound(SoundType.ClickButton);
    }
    public void CasualButton()
    {
        LinePoints.gameMode = GameMode.Casual;
        SoundManager.PlaySound(SoundType.ClickButton);
    }
    public void ChallangeButton()
    {
        LinePoints.gameMode = GameMode.Challange;
        SoundManager.PlaySound(SoundType.ClickButton);
    }
    public void QuitButton()
    {
        SoundManager.PlaySound(SoundType.ClickButton);
        Application.Quit();
    }

    public void SetVolumeSounds(float volume)
    {
        soundsMixer.SetFloat("Volume", volume);
        soundsText.text = (80 + soundsSlider.value).ToString("0.");
        PlayerPrefs.SetFloat(OptionsValues.VolumeSounds.ToString(), volume);
    }
    public void SetVolumeMusic(float volume)
    {
        musicMixer.SetFloat("Volume", volume);
        musicText.text = (80 + musicSlider.value).ToString("0.");
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
