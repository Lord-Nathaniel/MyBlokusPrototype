using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// This class manages the main menu of the game.
/// It manages the game start conditions, the credits and the quit actions.
/// -OUT- PlayerPieceManager | PreviewManager | SoundManager
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

    private const string GAME_SCENE = "GameScene";
    private const string TIME_OFFSET = "_TimeOffset";
    private const string PLAYER_COLOR = "_PlayerColor";

    // Needed services
    private PlayerSetup playerSetup;
    private OptionsMenuManager optionsMenuManager;
    private SoundManager soundManager;

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
        optionsMenuManager = ServiceManager.Get<OptionsMenuManager>();
        soundManager = ServiceManager.Get<SoundManager>();

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

        creditsButton.onClick.AddListener(() =>
        {
            CreditsAction();
        });

        creditsCloseButton.onClick.AddListener(() =>
        {
            CloseCreditsAction();
        });

        quitButton.onClick.AddListener(() =>
        {
            QuitAction();
        });
    }

    private void QuitAction()
    {
        soundManager.PlaySound(SoundType.CassetteRecordClick);
        Application.Quit();
    }

    private void CloseCreditsAction()
    {
        soundManager.PlaySound(SoundType.ButtonPress);
        Hide(creditsZone);
    }

    private void SetPlayerMenu(int i)
    {
        int index = i;
        playerColors.Add(index);
        playerTextureIds.Add(0);

        SelectPlayerColor(index, index, false);
        SelectPlayerTexture(index, 0, false);

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
                SelectPlayerColor(index, colorIndex, true);
            });
        }

        for (int k = 0; k < cellTextures.Count; k++)
        {
            int textureIndex = k;
            playerTextureSelectZones[index].transform.GetChild(k).GetComponent<Button>().onClick.AddListener(() =>
            {
                SelectPlayerTexture(index, textureIndex, true);
            });
        }
    }

    private void StartGameAction()
    {
        soundManager.PlaySound(SoundType.ButtonPress);

        playerSetup.AddPlayerSetting(0, playerInputFields[0].text, cellColors[playerColors[0]], playerTextureIds[0]);
        playerSetup.AddPlayerSetting(1, playerInputFields[1].text, cellColors[playerColors[1]], playerTextureIds[1]);
        if (playerThreeToggle.isOn)
            playerSetup.AddPlayerSetting(2, playerInputFields[2].text, cellColors[playerColors[2]], playerTextureIds[2]);
        if (playerFourToggle.isOn)
            playerSetup.AddPlayerSetting(1, playerInputFields[3].text, cellColors[playerColors[3]], playerTextureIds[3]);

        optionsMenuManager.SetOptionsSettingsToPlayerSetup();

        SceneManager.LoadScene(GAME_SCENE);
    }

    private void CreditsAction()
    {
        soundManager.PlaySound(SoundType.ButtonPress);
        Hide(gameZone);
        Hide(optionsZone);
        Toggle(creditsZone);
    }

    private void OptionAction()
    {
        soundManager.PlaySound(SoundType.ButtonPress);
        Hide(gameZone);
        Hide(creditsZone);
        Toggle(optionsZone);
    }

    private void GameAction()
    {
        soundManager.PlaySound(SoundType.ButtonPress);
        Hide(optionsZone);
        Hide(creditsZone);
        Toggle(gameZone);
    }

    private void SelectPlayerColor(int playerId, int colorID, bool shouldSoundPlay)
    {
        Color color = cellColors[colorID];
        playerColors[playerId] = colorID;

        Graphic playerOneColorButtonGraphic = playerColorButtons[playerId].GetComponent<Graphic>();
        Material baseMat = playerOneColorButtonGraphic.materialForRendering;
        Material playerOneColorButtonMaterial = new Material(baseMat);
        playerOneColorButtonMaterial.color = color;
        playerOneColorButtonGraphic.material = playerOneColorButtonMaterial;

        Graphic playerTextureButtonGraphic = playerTextureButtons[playerId].GetComponent<Graphic>();

        Material runtimeMaterialInstance = new Material(playerColorSwap);
        runtimeMaterialInstance.SetColor("_PlayerColor", color);
        runtimeMaterialInstance.SetTexture("_MainTex", cellTextures[playerTextureIds[playerId]]);
        playerTextureButtonGraphic.material = runtimeMaterialInstance;

        for (int i = 0; i < playerTextureSelectZones[playerId].transform.childCount; i++)
        {
            Graphic graphic = playerTextureSelectZones[playerId].transform.GetChild(i).gameObject.GetComponent<Graphic>();

            Material runtimeMaterialInstanceTex = new Material(playerColorSwap);
            runtimeMaterialInstanceTex.SetColor("_PlayerColor", color);
            runtimeMaterialInstanceTex.SetTexture("_MainTex", cellTextures[playerTextureIds[playerId]]);
            graphic.material = runtimeMaterialInstanceTex;
        }
        if (shouldSoundPlay)
            soundManager.PlaySound(SoundType.SprayCanPaint);
        Hide(playerColorSelectZones[playerId]);
    }

    private void SelectPlayerTexture(int playerId, int textureID, bool shouldSoundPlay)
    {
        Texture2D selectedTexture = cellTextures[textureID];
        playerTextureIds[playerId] = textureID;

        Graphic graphic = playerTextureButtons[playerId].GetComponent<Graphic>();

        Material runtimeMaterialInstance = new Material(playerColorSwap);
        runtimeMaterialInstance.SetColor("_PlayerColor", cellColors[playerColors[playerId]]);
        runtimeMaterialInstance.SetTexture("_MainTex", selectedTexture);
        graphic.material = runtimeMaterialInstance;

        if (shouldSoundPlay)
            soundManager.PlaySound(SoundType.SprayCanPaint);
        Hide(playerTextureSelectZones[playerId]);
    }

    /// <summary>
    /// Allow the player three to be selected.
    /// </summary>
    public void TogglePlayerThree()
    {
        soundManager.PlaySound(SoundType.CheckButtonClick);
        Toggle(playerThreeZone);
    }

    /// <summary>
    /// Allow the player four to be selected.
    /// </summary>
    public void TogglePlayerFour()
    {
        soundManager.PlaySound(SoundType.CheckButtonClick);
        Toggle(playerFourZone);
    }

    /// <summary>
    /// Allow the textures to be selected.
    /// </summary>
    public void ToggleTextureButtons()
    {
        soundManager.PlaySound(SoundType.CheckButtonClick);
        for (int i = 0; i < playerTextureButtons.Count; i++)
        {
            Toggle(playerTextureButtons[i].transform.parent.gameObject);
        }
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
