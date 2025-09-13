using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class PauseSettings : MonoBehaviour
{
    [SerializeField] private Volume volume;
    private ColorAdjustments colorAdjustments;

    [SerializeField] private AudioMixer soundsMixer;
    [SerializeField] private AudioMixer musicMixer;

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

    [SerializeField] private TMP_Text soundsText;
    [SerializeField] private TMP_Text musicText;
    [SerializeField] private TMP_Text senHorText;
    [SerializeField] private TMP_Text senVerText;
    [SerializeField] private TMP_Text senPadText;
    [SerializeField] private TMP_Text senZoomText;
    [SerializeField] private TMP_Text gammaText;
    [SerializeField] private TMP_Text maxFpsText;

    //[SerializeField] private GameObject firstSelected;

    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions;

    private float currentRefreshRate;
    private int currentResolutionIndex;

    private void Awake()
    {
        volume.profile.TryGet(out colorAdjustments);
    }

    private void Start()
    {
        ResolutionStart(); //defoult: hightest Resolution

        soundsSlider.value = PlayerPrefs.GetFloat(OptionsValues.VolumeSounds.ToString(), 1);
        musicSlider.value = PlayerPrefs.GetFloat(OptionsValues.VolumeMusic.ToString(), 1);

        senHorSlider.value = PlayerPrefs.GetFloat(OptionsValues.SensitivityHor.ToString(), 15);
        senVerSlider.value = PlayerPrefs.GetFloat(OptionsValues.SensitivityVer.ToString(), 10);
        senPadSlider.value = PlayerPrefs.GetFloat(OptionsValues.SensitivityPad.ToString(), 10);
        senZoomSlider.value = PlayerPrefs.GetFloat(OptionsValues.SensitivityZoom.ToString(), 100);

        gammaSlider.value = PlayerPrefs.GetFloat(OptionsValues.Gamma.ToString(), 0);
        maxFpsSlider.value = PlayerPrefs.GetFloat(OptionsValues.MaxFPS.ToString(), 144);

        screenModeDropdown.value = PlayerPrefs.GetInt(OptionsValues.ScreenMode.ToString(), 1); //defoult: fullscreen Window

        int VSyncint = PlayerPrefs.GetInt(OptionsValues.VSync.ToString(), 0); //defoult: off
        int invertXint = PlayerPrefs.GetInt(OptionsValues.InvertX.ToString(), 0); //defoult: off
        int invertYint = PlayerPrefs.GetInt(OptionsValues.InvertY.ToString(), 0); //defoult: off
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
    public void SetGamma(float value)
    {
        colorAdjustments.postExposure.Override(value);
        gammaText.text = gammaSlider.value.ToString("0.#");
        PlayerPrefs.SetFloat(OptionsValues.Gamma.ToString(), value);
    }
    public void SetMaxFPS(float value)
    {
        Application.targetFrameRate = Mathf.RoundToInt(value);
        maxFpsText.text = maxFpsSlider.value.ToString("0.");
        PlayerPrefs.SetFloat(OptionsValues.MaxFPS.ToString(), value);
    }
    public void SetVsync(bool IsOn)
    {
        int value = IsOn ? 1 : 0;
        QualitySettings.vSyncCount = value;
        PlayerPrefs.SetInt(OptionsValues.VSync.ToString(), value);
    }
    public void SetInvertX(bool value)
    {
        PlayerPrefs.SetInt(OptionsValues.InvertX.ToString(), value ? 1 : 0);
    }
    public void SetInvertY(bool value)
    {
        PlayerPrefs.SetInt(OptionsValues.InvertY.ToString(), value ? 1 : 0);
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
    public void SetSensitivityZoom(float Sensitivity)
    {
        senZoomText.text = (senZoomSlider.value / 2).ToString("0.");
        PlayerPrefs.SetFloat(OptionsValues.SensitivityZoom.ToString(), Sensitivity);
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
    }
}
