using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public enum OptionsValues
{
    SensitivityVer,
    SensitivityHor,
    SensitivityPad,
    VolumeMusic,
    VolumeSounds,
    Quality,
    ScreenMode,
    Resolution,
    InvertX,
    InvertY,
    VSync,
    MaxFPS,
    Gamma,
    SensitivityZoom
}

public class SettingsManager : MonoBehaviour
{
    [Header("Settings UI")]
    [SerializeField] private Toggle invertXToggle;
    [SerializeField] private Toggle invertYToggle;
    [SerializeField] private Toggle vSyncToggle;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown screenModeDropdown;
    [SerializeField] private Slider soundsSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider senHorSlider;
    [SerializeField] private Slider senVerSlider;
    [SerializeField] private Slider senPadSlider;
    [SerializeField] private Slider senZoomSlider;
    [SerializeField] private Slider gammaSlider;
    [SerializeField] private Slider maxFpsSlider;
    [Header("Settings Objects")]
    [SerializeField] private AudioMixer soundsMixer;
    [SerializeField] private AudioMixer musicMixer;
    [SerializeField] private Volume volume;
    private ColorAdjustments colorAdjustments;
    [Header("SettingsTexts")]
    [SerializeField] private TMP_Text soundsText;
    [SerializeField] private TMP_Text musicText;
    [SerializeField] private TMP_Text senHorText;
    [SerializeField] private TMP_Text senVerText;
    [SerializeField] private TMP_Text senPadText;
    [SerializeField] private TMP_Text senZoomText;
    [SerializeField] private TMP_Text gammaText;
    [SerializeField] private TMP_Text maxFpsText;
    //Resolution
    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions;
    private int currentResolutionIndex;
    private float currentRefreshRate;

    private void Awake()
    {
        volume.profile.TryGet(out colorAdjustments);
    }

    private void Start()
    {
        ResolutionStart(); //default: hightest Resolution

        soundsSlider.value = PlayerPrefs.GetFloat(nameof(OptionsValues.VolumeSounds), 1);
        musicSlider.value = PlayerPrefs.GetFloat(nameof(OptionsValues.VolumeMusic), 1);

        senHorSlider.value = PlayerPrefs.GetFloat(nameof(OptionsValues.SensitivityHor), 15);
        senVerSlider.value = PlayerPrefs.GetFloat(nameof(OptionsValues.SensitivityVer), 10);
        senPadSlider.value = PlayerPrefs.GetFloat(nameof(OptionsValues.SensitivityPad), 10);
        senZoomSlider.value = PlayerPrefs.GetFloat(nameof(OptionsValues.SensitivityZoom), 100);

        gammaSlider.value = PlayerPrefs.GetFloat(nameof(OptionsValues.Gamma), 0);
        maxFpsSlider.value = PlayerPrefs.GetFloat(nameof(OptionsValues.MaxFPS), 144);

        screenModeDropdown.value = PlayerPrefs.GetInt(nameof(OptionsValues.ScreenMode), 1); //default: fullscreen Window

        int VSyncint = PlayerPrefs.GetInt(nameof(OptionsValues.VSync), 0); //default: off
        int invertXint = PlayerPrefs.GetInt(nameof(OptionsValues.InvertX), 0); //default: off
        int invertYint = PlayerPrefs.GetInt(nameof(OptionsValues.InvertY), 0); //default: off
        vSyncToggle.isOn = VSyncint == 1;
        invertXToggle.isOn = invertXint == 1;
        invertYToggle.isOn = invertYint == 1;

        soundsText.text = (soundsSlider.value * 100).ToString("0.");
        musicText.text = (musicSlider.value * 100).ToString("0.");

        senHorText.text = senHorSlider.value.ToString("0.");
        senVerText.text = senVerSlider.value.ToString("0.");
        senPadText.text = senPadSlider.value.ToString("0.");
        senZoomText.text = (senZoomSlider.value / 2).ToString("0.");

        gammaText.text = gammaSlider.value.ToString("0.#");
        maxFpsText.text = maxFpsSlider.value.ToString("0.");
    }

    #region Settings
    public void SetVolumeSounds(float volume)
    {
        soundsMixer.SetFloat("Volume", Mathf.Log10(volume) * 20);
        soundsText.text = (soundsSlider.value * 100).ToString("0.");
        PlayerPrefs.SetFloat(nameof(OptionsValues.VolumeSounds), volume);
    }
    public void SetVolumeMusic(float volume)
    {
        musicMixer.SetFloat("Volume", Mathf.Log10(volume) * 20);
        musicText.text = (musicSlider.value * 100).ToString("0.");
        PlayerPrefs.SetFloat(nameof(OptionsValues.VolumeMusic), volume);
    }
    public void SetGamma(float value)
    {
        colorAdjustments.postExposure.Override(value);
        gammaText.text = gammaSlider.value.ToString("0.#");
        PlayerPrefs.SetFloat(nameof(OptionsValues.Gamma), value);
    }
    public void SetMaxFPS(float value)
    {
        Application.targetFrameRate = Mathf.RoundToInt(value);
        maxFpsText.text = maxFpsSlider.value.ToString("0.");
        PlayerPrefs.SetFloat(nameof(OptionsValues.MaxFPS), value);
    }
    public void SetVsync(bool IsOn)
    {
        int value = IsOn ? 1 : 0;
        QualitySettings.vSyncCount = value;
        PlayerPrefs.SetInt(nameof(OptionsValues.VSync), value);
    }
    public void SetInvertX(bool value)
    {
        PlayerPrefs.SetInt(nameof(OptionsValues.InvertX), value ? 1 : 0);
    }
    public void SetInvertY(bool value)
    {
        PlayerPrefs.SetInt(nameof(OptionsValues.InvertY), value ? 1 : 0);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt(nameof(OptionsValues.Quality), qualityIndex);
    }

    public void SetScreenMode(int screenMode)
    {
        Screen.fullScreenMode = (FullScreenMode)screenMode;
        PlayerPrefs.SetInt(nameof(OptionsValues.ScreenMode), screenMode);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, (FullScreenMode)PlayerPrefs.GetInt(nameof(OptionsValues.ScreenMode), 1));
        PlayerPrefs.SetInt(nameof(OptionsValues.Resolution), resolutionIndex);
    }

    public void SetSensitivityHorizontal(float Sensitivity)
    {
        senHorText.text = senHorSlider.value.ToString("0.");
        PlayerPrefs.SetFloat(nameof(OptionsValues.SensitivityHor), Sensitivity);
    }
    public void SetSensitivityVertical(float Sensitivity)
    {
        senVerText.text = senVerSlider.value.ToString("0.");
        PlayerPrefs.SetFloat(nameof(OptionsValues.SensitivityVer), Sensitivity);
    }
    public void SetSensitivityGamepad(float Sensitivity)
    {
        senPadText.text = senPadSlider.value.ToString("0.");
        PlayerPrefs.SetFloat(nameof(OptionsValues.SensitivityPad), Sensitivity);
    }
    public void SetSensitivityZoom(float Sensitivity)
    {
        senZoomText.text = (senZoomSlider.value / 2).ToString("0.");
        PlayerPrefs.SetFloat(nameof(OptionsValues.SensitivityZoom), Sensitivity);
    }
    #endregion

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

            options.Add(resolutionOption);

            if (filteredResolutions[i].width == Screen.width && filteredResolutions[i].height == Screen.height &&
                (float)filteredResolutions[i].refreshRateRatio.value == currentRefreshRate)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex = PlayerPrefs.GetInt(nameof(OptionsValues.Resolution), 0);
        resolutionDropdown.RefreshShownValue();
    }
    public void SetResolution()
    {
        Resolution resolution = filteredResolutions[currentResolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, (FullScreenMode)PlayerPrefs.GetInt(nameof(OptionsValues.ScreenMode), 1));
    }
}
