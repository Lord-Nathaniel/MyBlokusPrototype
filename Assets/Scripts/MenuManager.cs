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
    [Header("Background Settings")]
    [SerializeField] private GameObject background;
    [SerializeField] private Material fullscreenMaterial;

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

    private List<Color> cellColors = new();

    private List<int> playerColors = new();
    private List<int> playerTextureIds = new();
    private int currentLanguageIndex;

    private const string GAME_SCENE = "GameScene";
    private const string TIME_OFFSET = "_TimeOffset";
    private const string PLAYER_COLOR = "_PlayerColor";

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
        fullscreenMaterial.SetColor(PLAYER_COLOR, new Color(0f, 0f, 0f, 0f));
        playerSetup = ServiceManager.Get<PlayerSetup>();

        cellColors.Add(new Color(0.24f, 0.24f, 0.73f));
        cellColors.Add(new Color(0.73f, 0.06f, 0.06f));
        cellColors.Add(new Color(1f, 0.75f, 0));
        cellColors.Add(new Color(0.5f, 0, 0.5f));
        cellColors.Add(new Color(0.2f, 0.8f, 0.2f));
        cellColors.Add(new Color(0.24f, 0.79f, 0.88f));
        cellColors.Add(new Color(1f, 0.42f, 0.71f));
        cellColors.Add(new Color(0.6f, 0.6f, 0.6f));

        for (int i = 0; i < maxPlayerNb; i++)
        {
            SetPlayerMenu(i);
        }

        background.GetComponent<Image>().material.SetFloat(TIME_OFFSET, Random.Range(0f, 255f));

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
        playerColors.Add(index);
        playerTextureIds.Add(0);

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

        for (int j = 0; j < cellColors.Count; j++)
        {
            int colorIndex = j;
            playerColorSelectZones[index].transform.GetChild(j).GetComponent<Button>().onClick.AddListener(() =>
            {
                SelectPlayerColor(index, colorIndex);
            });
        }

        for (int k = 0; k < cellTextures.Count; k++)
        {
            int textureIndex = k;
            playerTextureSelectZones[index].transform.GetChild(k).GetComponent<Button>().onClick.AddListener(() =>
            {
                SelectPlayerTexture(index, textureIndex);
            });
        }
    }

    private void StartGameAction()
    {
        playerSetup.AddPlayerSetting(0, playerInputFields[0].text, cellColors[playerColors[0]], playerTextureIds[0]);
        playerSetup.AddPlayerSetting(1, playerInputFields[1].text, cellColors[playerColors[1]], playerTextureIds[1]);
        if (playerThreeToggle.isOn)
            playerSetup.AddPlayerSetting(2, playerInputFields[2].text, cellColors[playerColors[2]], playerTextureIds[2]);
        if (playerFourToggle.isOn)
            playerSetup.AddPlayerSetting(1, playerInputFields[3].text, cellColors[playerColors[3]], playerTextureIds[3]);

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
        Color color = cellColors[colorID];
        playerColors[playerId] = colorID;

        Graphic playerOneColorButtonGraphic = playerColorButtons[playerId].GetComponent<Graphic>();
        Material baseMat = playerOneColorButtonGraphic.materialForRendering;
        Material playerOneColorButtonMaterial = new Material(baseMat);
        playerOneColorButtonMaterial.color = color;
        playerOneColorButtonGraphic.material = playerOneColorButtonMaterial;

        Graphic playerOneTextureButtonGraphic = playerTextureButtons[playerId].GetComponent<Graphic>();

        Material runtimeMaterialInstance = new Material(playerColorSwap);
        runtimeMaterialInstance.SetColor("_PlayerColor", color);
        runtimeMaterialInstance.SetTexture("_MainTex", cellTextures[playerTextureIds[playerId]]);
        playerOneTextureButtonGraphic.material = runtimeMaterialInstance;

        for (int i = 0; i < playerTextureSelectZones[playerId].transform.childCount; i++)
        {
            Graphic graphic = playerTextureSelectZones[playerId].transform.GetChild(i).gameObject.GetComponent<Graphic>();

            Material runtimeMaterialInstanceTex = new Material(playerColorSwap);
            runtimeMaterialInstanceTex.SetColor("_PlayerColor", color);
            runtimeMaterialInstanceTex.SetTexture("_MainTex", cellTextures[playerTextureIds[playerId]]);
            graphic.material = runtimeMaterialInstanceTex;
        }
        Hide(playerColorSelectZones[playerId]);
    }

    private void SelectPlayerTexture(int playerId, int textureID)
    {
        Texture2D selectedTexture = cellTextures[textureID];
        playerTextureIds[playerId] = textureID;

        Graphic playerOneTextureButtonGraphic = playerTextureButtons[playerId].GetComponent<Graphic>();

        Material runtimeMaterialInstance = new Material(playerColorSwap);
        runtimeMaterialInstance.SetColor("_PlayerColor", cellColors[playerId]);
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
