using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

/// <summary>
/// This class manages the options menu of the game.
/// At start, it take the optionsSetting of the PlayerSetup and apply them.
/// -IN- GameManager | SoundSlider | MusicSlider
/// -OUT- PlayerSetup | SoundManager | MusicManager
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
    private MusicManager musicManager;

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
        musicManager = ServiceManager.Get<MusicManager>();

        optionsCloseButton.onClick.AddListener(() =>
        {
            optionsCloseButton.transform.DOScale(1.1f, 0.1f)
                                        .SetEase(Ease.OutBounce)
                                        .OnComplete(() => CloseOptionsAction());
        });

        languageButton.onClick.AddListener(() =>
        {
            AnimateButton(languageButton);
            ToggleLanguage();
        });
        InitOptions();
    }

    private void AnimateButton(Button button)
    {
        button.transform.DOScale(1.2f, 0.2f)
                        .SetEase(Ease.OutBounce)
                        .OnComplete(() => button.transform.DOScale(1f, 0.2f));
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
        }
        else
        {
            soundVolume = 8f;
            musicVolume = 3f;
        }
        soundSlider.GetComponent<Slider>().SetValueWithoutNotify(soundVolume);
        soundValueText.text = soundVolume.ToString();
        soundManager.GetComponent<AudioSource>().volume = soundVolume / 10;

        musicSlider.GetComponent<Slider>().SetValueWithoutNotify(musicVolume);
        musicValueText.text = musicVolume.ToString();
        musicManager.GetComponent<AudioSource>().volume = musicVolume / 10;
    }

    private void ToggleLanguage()
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
    /// Update the sound value text when the slider value changes.    /// 
    /// -IN- SoundSlider from OnValueChanged()
    /// </summary>
    public void UpdateSoundValue()
    {
        float newSoundVolume = soundSlider.GetComponent<Slider>().value;
        soundManager.PlaySound(SoundType.Click);
        soundValueText.text = newSoundVolume.ToString();
        soundManager.GetComponent<AudioSource>().volume = newSoundVolume / 10;
    }

    /// <summary>
    /// Update the music value text when the slider value changes.
    /// -IN- MusicSlider from OnValueChanged()
    /// </summary>
    public void UpdateMusicValue()
    {
        float newMusicVolume = musicSlider.GetComponent<Slider>().value;
        soundManager.PlaySound(SoundType.Click);
        musicValueText.text = newMusicVolume.ToString();
        musicManager.GetComponent<AudioSource>().volume = newMusicVolume / 10;
    }

    /// <summary>
    /// Set the sound and music volumes and language to the Player Setup.
    /// -IN- GameManager from StartGameAction()
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
