using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// This class manages the keyboard and mouse input.
/// When the player interact with the game, it calls corresponding Actions.
/// -IN- PlayerPieceManager | PreviewManager
/// </summary>
public class MenuManager : MonoBehaviour
{
    [Header("Buttons Settings")]
    [SerializeField] private Button gameButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;

    [Header("Game Settings")]
    [SerializeField] private GameObject gameZone;
    [SerializeField] private Button gameStartButton;
    [SerializeField] private Material playerColorSwap;
    [SerializeField] private Toggle textureToggle;

    [Header("Player Settings")]
    [SerializeField] private List<Button> playerColorButtons;
    [SerializeField] private List<Button> playerTextureButtons;
    [SerializeField] private List<TMP_InputField> playerInputFields;
    [SerializeField] private List<GameObject> playerColorSelectZones;
    [SerializeField] private List<GameObject> playerTextureSelectZones;

    [Header("Player Three and Four Settings")]
    [SerializeField] private Toggle playerThreeToggle;
    [SerializeField] private GameObject playerThreeZone;
    [SerializeField] private Toggle playerFourToggle;
    [SerializeField] private GameObject playerFourZone;

    [Header("Selectable Textures")]
    [SerializeField] private List<Texture2D> cellTextures;

    [Header("Options Settings")]
    [SerializeField] private GameObject optionsZone;
    [SerializeField] private Button optionsCloseButton;
    [SerializeField] private Slider soundSlider;
    [SerializeField] private TextMeshProUGUI soundValueText;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private TextMeshProUGUI musicValueText;
    [SerializeField] private Button languageButton;

    [Header("Credits Settings")]
    [SerializeField] private GameObject creditsZone;
    [SerializeField] private Button creditsCloseButton;

    private int maxPlayerNb = 4;

    private List<Color> playerColors = new();
    private List<int> playerTextureIds = new();
    private int currentLanguageIndex;

    const string GAME_SCENE = "GameScene";

    // Needed services
    private PlayerSetup playerSetup;

    private void Awake()
    {
        ServiceManager.Register(this);
    }

    private void OnDestroy()
    {
        ServiceManager.Unregister<GameManager>();
    }

    private void Start()
    {
        playerSetup = ServiceManager.Get<PlayerSetup>();

        playerColors.Add(new Color(0.24f, 0.24f, 0.73f));
        playerColors.Add(new Color(0.73f, 0.06f, 0.06f));
        playerColors.Add(new Color(1f, 0.75f, 0));
        playerColors.Add(new Color(0.5f, 0, 0.5f));
        playerColors.Add(new Color(0.2f, 0.8f, 0.2f));
        playerColors.Add(new Color(0.24f, 0.79f, 0.88f));
        playerColors.Add(new Color(1f, 0.42f, 0.71f));
        playerColors.Add(new Color(0.6f, 0.6f, 0.6f));

        for (int i = 0; i < maxPlayerNb; i++)
        {
            SetPlayerMenu(i);
        }

        gameButton.onClick.AddListener(() =>
        {
            GameAction();
        });

        gameStartButton.onClick.AddListener(() =>
        {
            StartGameAction();
        });

        optionsButton.onClick.AddListener(() =>
        {
            OptionAction();
        });

        optionsCloseButton.onClick.AddListener(() =>
        {
            Hide(optionsZone);
        });

        creditsButton.onClick.AddListener(() =>
        {
            CreditsAction();
        });

        creditsCloseButton.onClick.AddListener(() =>
        {
            Hide(creditsZone);
        });

        quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        languageButton.onClick.AddListener(() =>
        {
            ToggleLanguage();
        });
    }

    private void SetPlayerMenu(int i)
    {
        int index = i;
        SelectPlayerColor(index, index);
        SelectPlayerTexture(index, 0);

        playerColorButtons[index].onClick.AddListener(() =>
        {
            Toggle(playerColorSelectZones[index]);
        });

        playerTextureButtons[index].onClick.AddListener(() =>
        {
            Toggle(playerTextureSelectZones[index]);
        });

        for (int j = 0; j < playerColors.Count; j++)
        {
            playerColorSelectZones[index].transform.GetChild(j).GetComponent<Button>().onClick.AddListener(() =>
            {
                SelectPlayerColor(index, j);
            });
        }

        for (int k = 0; k < cellTextures.Count; k++)
        {
            playerTextureSelectZones[index].transform.GetChild(k).GetComponent<Button>().onClick.AddListener(() =>
            {
                SelectPlayerTexture(index, k);
            });
        }
    }

    private void StartGameAction()
    {
        playerSetup.AddPlayerSetting(0, playerInputFields[0].text, playerColors[0], playerTextureIds[0]);
        playerSetup.AddPlayerSetting(1, playerInputFields[1].text, playerColors[1], playerTextureIds[1]);
        if (playerThreeToggle.isOn)
            playerSetup.AddPlayerSetting(2, playerInputFields[2].text, playerColors[2], playerTextureIds[2]);
        if (playerFourToggle.isOn)
            playerSetup.AddPlayerSetting(1, playerInputFields[3].text, playerColors[3], playerTextureIds[3]);

        playerSetup.AddOptionsSetting(soundSlider.GetComponent<Slider>().value,
                                      musicSlider.GetComponent<Slider>().value,
                                      currentLanguageIndex);

        SceneManager.LoadScene(GAME_SCENE);
    }

    /// <summary>
    /// Toggle to the next available language.
    /// </summary>
    public void ToggleLanguage()
    {
        List<Locale> locales = LocalizationSettings.AvailableLocales.Locales;

        if (locales.Count == 0)
            return;

        Locale currentLocale = LocalizationSettings.SelectedLocale;
        currentLanguageIndex = locales.IndexOf(currentLocale);

        int nextIndex = (currentLanguageIndex + 1) % locales.Count;

        LocalizationSettings.SelectedLocale = locales[nextIndex];
    }

    private void CreditsAction()
    {
        Hide(gameZone);
        Hide(optionsZone);
        Toggle(creditsZone);
    }

    private void OptionAction()
    {
        Hide(gameZone);
        Hide(creditsZone);
        Toggle(optionsZone);
    }

    private void GameAction()
    {
        Hide(optionsZone);
        Hide(creditsZone);
        Toggle(gameZone);
    }

    private void SelectPlayerColor(int playerId, int colorID)
    {
        Color color = playerColors[colorID];
        playerColors[playerId] = color;

        Graphic playerOneColorButtonGraphic = playerColorButtons[playerId].GetComponent<Graphic>();
        Material baseMat = playerOneColorButtonGraphic.materialForRendering;
        Material playerOneColorButtonMaterial = new Material(baseMat);
        playerOneColorButtonMaterial.color = playerColors[playerId];
        playerOneColorButtonGraphic.material = playerOneColorButtonMaterial;

        Graphic playerOneTextureButtonGraphic = playerTextureButtons[playerId].GetComponent<Graphic>();

        Material runtimeMaterialInstance = new Material(playerColorSwap);
        runtimeMaterialInstance.SetColor("_PlayerColor", color);
        playerOneTextureButtonGraphic.material = runtimeMaterialInstance;

        for (int i = 0; i < playerTextureSelectZones[playerId].transform.childCount; i++)
        {
            Graphic graphic = playerTextureSelectZones[playerId].transform.GetChild(i).gameObject.GetComponent<Graphic>();

            Material runtimeMaterialInstanceTex = new Material(playerColorSwap);
            runtimeMaterialInstanceTex.SetColor("_PlayerColor", color);
            graphic.material = runtimeMaterialInstanceTex;
        }
        Hide(playerColorSelectZones[playerId]);
    }

    private void SelectPlayerTexture(int playerId, int textureID)
    {
        playerTextureIds.Add(textureID);
        Texture2D selectedTexture = cellTextures[textureID];

        Graphic playerOneTextureButtonGraphic = playerTextureButtons[playerId].GetComponent<Graphic>();

        Material runtimeMaterialInstance = new Material(playerColorSwap);
        runtimeMaterialInstance.SetColor("_PlayerColor", playerColors[playerId]);
        runtimeMaterialInstance.SetTexture("_MainTex", selectedTexture);
        playerOneTextureButtonGraphic.material = runtimeMaterialInstance;
        Hide(playerTextureSelectZones[playerId]);
    }

    /// <summary>
    /// Allow the player three to be selected.
    /// </summary>
    public void TogglePlayerThree()
    {
        Toggle(playerThreeZone);
    }

    /// <summary>
    /// Allow the player four to be selected.
    /// </summary>
    public void TogglePlayerFour()
    {
        Toggle(playerFourZone);
    }

    /// <summary>
    /// Allow the textures to be selected.
    /// </summary>
    public void ToggleTextureButtons()
    {
        for (int i = 0; i < playerTextureButtons.Count; i++)
        {
            Toggle(playerTextureButtons[i].transform.parent.gameObject);
        }
    }

    /// <summary>
    /// Update the sound value text when the slider value changes.
    /// </summary>
    public void UpdateSoundTextValue()
    {
        soundValueText.text = soundSlider.GetComponent<Slider>().value.ToString();
    }

    /// <summary>
    /// Update the music value text when the slider value changes.
    /// </summary>
    public void UpdateMusicTextValue()
    {
        musicValueText.text = musicSlider.GetComponent<Slider>().value.ToString();
    }

    private void Hide(GameObject toHide)
    {
        toHide.SetActive(false);
    }

    private void Toggle(GameObject toToggle)
    {
        if (toToggle.activeInHierarchy)
            toToggle.SetActive(false);
        else
            toToggle.SetActive(true);
    }
}
