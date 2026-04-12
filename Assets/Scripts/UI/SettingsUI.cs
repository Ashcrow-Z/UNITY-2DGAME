using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    public Slider bgmSlider;
    public Slider sfxSlider;
    public Button backButton;

    void OnEnable()
    {
        if (AudioManager.Instance != null)
        {
            if (bgmSlider != null)
            {
                bgmSlider.value = AudioManager.Instance.GetBGMVolume();
                bgmSlider.onValueChanged.AddListener(OnBGMChanged);
            }

            if (sfxSlider != null)
            {
                sfxSlider.value = AudioManager.Instance.GetSFXVolume();
                sfxSlider.onValueChanged.AddListener(OnSFXChanged);
            }
        }

        if (backButton != null)
            backButton.onClick.AddListener(OnBackClicked);
    }

    void OnDisable()
    {
        if (bgmSlider != null) bgmSlider.onValueChanged.RemoveListener(OnBGMChanged);
        if (sfxSlider != null) sfxSlider.onValueChanged.RemoveListener(OnSFXChanged);
        if (backButton != null) backButton.onClick.RemoveListener(OnBackClicked);
    }

    void OnBGMChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetBGMVolume(value);
    }

    void OnSFXChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetSFXVolume(value);
    }

    void OnBackClicked()
    {
        MainMenuUI mainMenu = GetComponentInParent<MainMenuUI>();
        if (mainMenu != null) mainMenu.OnBackFromSettings();
    }
}
