using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

/// <summary>
/// This class manages the options menu of the game.
/// At start, it take the optionsSetting of the PlayerSetup and apply them.
/// -OUT- PlayerSetup | SoundManager 
/// </summary>
public class OptionsMenuManager : MonoBehaviour
{
    [Header("Options Settings")]
    [SerializeField] private GameObject optionsZone;
    [SerializeField] private Button optionsCloseButton;
    [SerializeField] private Slider soundSlider;
    [SerializeField] private TextMeshProUGUI soundValueText;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private TextMeshProUGUI musicValueText;
    [SerializeField] private Button languageButton;

    private float soundVolume;
    private float musicVolume;
    private int currentLanguageIndex;

    // Needed services
    private PlayerSetup playerSetup;
    private SoundManager soundManager;

    private void Awake()
    {
        ServiceManager.Register(this);
    }

    private void OnDestroy()
    {
        ServiceManager.Unregister<OptionsMenuManager>();
    }

    private void Start()
    {
        playerSetup = ServiceManager.Get<PlayerSetup>();
        soundManager = ServiceManager.Get<SoundManager>();

        optionsCloseButton.onClick.AddListener(() =>
        {
            CloseOptionsAction();
        });

        languageButton.onClick.AddListener(() =>
        {
            ToggleLanguage();
        });
        InitOptions();
    }

    private void CloseOptionsAction()
    {
        soundManager.PlaySound(SoundType.ButtonPress);
        Hide(optionsZone);
    }

    private void InitOptions()
    {
        if (playerSetup.optionsSettings != null)
        {
            soundVolume = playerSetup.optionsSettings.soundVolume;
            musicVolume = playerSetup.optionsSettings.musicVolume;
            currentLanguageIndex = playerSetup.optionsSettings.languageIndex;

            soundSlider.GetComponent<Slider>().SetValueWithoutNotify(soundVolume);
            musicSlider.GetComponent<Slider>().SetValueWithoutNotify(musicVolume);
            UpdateSoundValue();
            UpdateMusicTextValue();
        }
    }

    /// <summary>
    /// Toggle to the next available language.
    /// </summary>
    public void ToggleLanguage()
    {
        soundManager.PlaySound(SoundType.ButtonPress);
        List<Locale> locales = LocalizationSettings.AvailableLocales.Locales;

        if (locales.Count == 0)
            return;

        Locale currentLocale = LocalizationSettings.SelectedLocale;
        currentLanguageIndex = locales.IndexOf(currentLocale);

        int nextIndex = (currentLanguageIndex + 1) % locales.Count;

        LocalizationSettings.SelectedLocale = locales[nextIndex];
    }

    /// <summary>
    /// Update the sound value text when the slider value changes.
    /// </summary>
    public void UpdateSoundValue()
    {
        float newSoundVolume = soundSlider.GetComponent<Slider>().value;
        soundManager.PlaySound(SoundType.Click);
        soundValueText.text = soundSlider.GetComponent<Slider>().value.ToString();
        soundManager.GetComponent<AudioSource>().volume = newSoundVolume / 10;
    }

    /// <summary>
    /// Update the music value text when the slider value changes.
    /// </summary>
    public void UpdateMusicTextValue()
    {
        soundManager.PlaySound(SoundType.Click);
        musicValueText.text = musicSlider.GetComponent<Slider>().value.ToString();
    }

    /// <summary>
    /// Set the sound and music volumes and language to the Player Setup.
    /// </summary>
    public void SetOptionsSettingsToPlayerSetup()
    {
        playerSetup.AddOptionsSetting(soundSlider.GetComponent<Slider>().value,
                                  musicSlider.GetComponent<Slider>().value,
                                  currentLanguageIndex);
    }
    private void Hide(GameObject toHide)
    {
        toHide.SetActive(false);
    }
}
