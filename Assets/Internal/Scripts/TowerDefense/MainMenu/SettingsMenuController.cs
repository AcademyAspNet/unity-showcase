using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenuController : MonoBehaviour
{
    private const string GeneralVolumeParameterName = "GeneralVolume";

    [Header("External References")]

    [SerializeField]
    private AudioMixer _audioMixer;

    [SerializeField]
    private Animator _canvasAnimator;

    [Header("User Interface Elements")]

    [SerializeField]
    private TMP_Dropdown _screenResolutionDropdown;

    [SerializeField]
    private Toggle _fullscreenToggle;

    [SerializeField]
    private TMP_Dropdown _graphicsQualityDropdown;

    [SerializeField]
    private Slider _volumeSlider;

    [SerializeField]
    private Button _applyButton;

    [Header("Trigger Names")]

    [SerializeField]
    private string _returnToMainMenuTriggerName = "ToMainMenu";

    private Resolution[] _availableScreenResolutions;

    public void ReturnToMainMenu()
    {
        _canvasAnimator.SetTrigger(_returnToMainMenuTriggerName);
    }

    private void Start()
    {
        _availableScreenResolutions = Screen.resolutions;

        if (_screenResolutionDropdown != null)
            SetUpScreenResolutionDropdown();

        if (_fullscreenToggle != null)
            SetUpFullscreenToggle();

        if (_graphicsQualityDropdown != null)
            SetUpGraphicsQualityDropdown();

        if (_volumeSlider != null)
            SetUpVolumeSlider();

        if (_applyButton != null)
            SetUpApplyButton();
    }

    private void SetUpScreenResolutionDropdown()
    {
        List<string> dropdownOptions = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < _availableScreenResolutions.Length; i++)
        {
            Resolution resolution = _availableScreenResolutions[i];
            string dropdownOption = $"{resolution.width}x{resolution.height}";

            dropdownOptions.Add(dropdownOption);

            if (resolution.Equals(Screen.currentResolution))
                currentResolutionIndex = i;
        }

        _screenResolutionDropdown.ClearOptions();
        _screenResolutionDropdown.AddOptions(dropdownOptions);

        _screenResolutionDropdown.value = currentResolutionIndex;
        _screenResolutionDropdown.RefreshShownValue();

        _screenResolutionDropdown.onValueChanged.AddListener(delegate
        {
            UnlockApplyButton();
        });
    }

    private void SetUpFullscreenToggle()
    {
        _fullscreenToggle.isOn = Screen.fullScreen;
        _fullscreenToggle.onValueChanged.AddListener(delegate
        {
            UnlockApplyButton();
        });
    }

    private void SetUpGraphicsQualityDropdown()
    {
        string[] qualityPresetNames = QualitySettings.names;
        List<string> dropdownOptions = new List<string>(qualityPresetNames);

        _graphicsQualityDropdown.ClearOptions();
        _graphicsQualityDropdown.AddOptions(dropdownOptions);

        _graphicsQualityDropdown.value = QualitySettings.GetQualityLevel();
        _graphicsQualityDropdown.RefreshShownValue();

        _graphicsQualityDropdown.onValueChanged.AddListener(delegate
        {
            UnlockApplyButton();
        });
    }

    private void SetUpVolumeSlider()
    {
        _volumeSlider.minValue = -80f;
        _volumeSlider.maxValue = 0f;

        _volumeSlider.onValueChanged.AddListener(delegate
        {
            UnlockApplyButton();
        });

        if (_audioMixer.GetFloat(GeneralVolumeParameterName, out float generalVolume))
        {
            _volumeSlider.value = Mathf.Clamp01(generalVolume);
            return;
        }

        _volumeSlider.value = 1f;
    }

    private void SetUpApplyButton()
    {
        _applyButton.interactable = false;
        _applyButton.onClick.AddListener(ApplySettings);
    } 

    private void UnlockApplyButton()
    {
        if (_applyButton == null)
            return;

        _applyButton.interactable = true;
    }

    private void ApplySettings()
    {
        if (_screenResolutionDropdown != null)
            ApplyScreenResolutionChanges();

        if (_fullscreenToggle != null)
            ApplyScreenModeChanges();

        if (_graphicsQualityDropdown != null)
            AppluQualityPresetChanges();

        if (_volumeSlider != null)
            ApplySoundVolumeChanges();

        if (_applyButton != null)
            _applyButton.interactable = false;
    }

    private void ApplyScreenResolutionChanges()
    {
        int screenResolutionIndex = _screenResolutionDropdown.value;

        if (screenResolutionIndex < 0)
            return;

        if (screenResolutionIndex > _availableScreenResolutions.Length)
            return;

        Resolution? resolution = _availableScreenResolutions[screenResolutionIndex];

        if (resolution == null)
            return;

        Resolution nextResolution = resolution.Value;
        bool isFullscreen = true;

        if (_fullscreenToggle != null)
            isFullscreen = _fullscreenToggle.isOn;

        Screen.SetResolution(nextResolution.width, nextResolution.height, isFullscreen);
    }

    private void ApplyScreenModeChanges()
    {
        Screen.fullScreen = _fullscreenToggle.isOn;
    }

    private void AppluQualityPresetChanges()
    {
        int qualityPresetIndex = _graphicsQualityDropdown.value;
        QualitySettings.SetQualityLevel(qualityPresetIndex);
    }

    private void ApplySoundVolumeChanges()
    {
        float soundVolume = _volumeSlider.value;
        _audioMixer.SetFloat(GeneralVolumeParameterName, soundVolume);
    }
}
