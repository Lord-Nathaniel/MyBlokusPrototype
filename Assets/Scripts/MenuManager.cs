using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [Header("Player One Settings")]
    [SerializeField] private Button playerOneColorButton;
    [SerializeField] private Button playerOneTextureButton;
    [SerializeField] private TMP_InputField playerOneInputField;
    [SerializeField] private GameObject playerOneColorSelectZone;
    [SerializeField] private GameObject playerOneTextureSelectZone;

    [Header("Player Two Settings")]
    [SerializeField] private Button playerTwoColorButton;
    [SerializeField] private Button playerTwoTextureButton;
    [SerializeField] private TMP_InputField playerTwoInputField;
    [SerializeField] private GameObject playerTwoColorSelectZone;
    [SerializeField] private GameObject playerTwoTextureSelectZone;

    [Header("Player Three Settings")]
    [SerializeField] private Toggle playerThreeToggle;
    [SerializeField] private GameObject playerThreeZone;
    [SerializeField] private Button playerThreeColorButton;
    [SerializeField] private Button playerThreeTextureButton;
    [SerializeField] private TMP_InputField playerThreeInputField;
    [SerializeField] private GameObject playerThreeColorSelectZone;
    [SerializeField] private GameObject playerThreeTextureSelectZone;

    [Header("Player Four Settings")]
    [SerializeField] private Toggle playerFourToggle;
    [SerializeField] private GameObject playerFourZone;
    [SerializeField] private Button playerFourColorButton;
    [SerializeField] private Button playerFourTextureButton;
    [SerializeField] private TMP_InputField playerFourInputField;
    [SerializeField] private GameObject playerFourColorSelectZone;
    [SerializeField] private GameObject playerFourTextureSelectZone;

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

    private List<Color> playerColors = new();
    private List<Texture2D> playerTextures = new();
    private Color playerOneColor;
    private Color playerTwoColor;
    private Color playerThreeColor;
    private Color playerFourColor;
    private int playerOneTextureId;
    private int playerTwoTextureId;
    private int playerThreeTextureId;
    private int playerFourTextureId;
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
        playerColors.Add(new Color(.6f, 0.6f, 0.6f));

        SelectPlayerOneColor(1);
        SelectPlayerOneTexture(0);

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

        playerOneColorButton.onClick.AddListener(() =>
        {
            Toggle(playerOneColorSelectZone);
        });

        playerOneTextureButton.onClick.AddListener(() =>
        {
            Toggle(playerOneTextureSelectZone);
        });

        languageButton.onClick.AddListener(() =>
        {
            ToggleLanguage();
        });
    }

    private void StartGameAction()
    {
        //TODO complete with true values
        string playerOneName = playerOneInputField.text;
        string playerTwoName = playerTwoInputField.text;
        string playerThreeName = playerThreeInputField.text;
        string playerFourName = playerFourInputField.text;

        playerSetup.AddPlayerSetting(0, playerOneName, playerOneColor, playerOneTextureId);
        playerSetup.AddPlayerSetting(1, playerTwoName, playerTwoColor, playerTwoTextureId);
        if (playerThreeToggle.isOn)
            playerSetup.AddPlayerSetting(2, playerThreeName, playerThreeColor, playerThreeTextureId);
        if (playerFourToggle.isOn)
            playerSetup.AddPlayerSetting(3, playerFourName, playerFourColor, playerFourTextureId);

        playerSetup.AddOptionsSetting(soundSlider.GetComponent<Slider>().value,
                                      musicSlider.GetComponent<Slider>().value,
                                      currentLanguageIndex);

        SceneManager.LoadScene(GAME_SCENE);
    }

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

    public void SelectPlayerOneColor(int colorID)
    {
        Debug.Log("Selected colorID: " + colorID);
        Color color = playerColors[colorID];
        playerOneColor = color;

        Graphic playerOneColorButtonGraphic = playerOneColorButton.GetComponent<Graphic>();
        Material baseMat = playerOneColorButtonGraphic.materialForRendering;
        Material playerOneColorButtonMaterial = new Material(baseMat);
        playerOneColorButtonMaterial.color = playerOneColor;
        playerOneColorButtonGraphic.material = playerOneColorButtonMaterial;

        Graphic playerOneTextureButtonGraphic = playerOneTextureButton.GetComponent<Graphic>();

        Material runtimeMaterialInstance = new Material(playerColorSwap);
        runtimeMaterialInstance.SetColor("_PlayerColor", color);
        playerOneTextureButtonGraphic.material = runtimeMaterialInstance;

        for (int i = 0; i < playerOneTextureSelectZone.transform.childCount; i++)
        {
            Graphic graphic = playerOneTextureSelectZone.transform.GetChild(i).gameObject.GetComponent<Graphic>();

            Material runtimeMaterialInstanceTex = new Material(playerColorSwap);
            runtimeMaterialInstanceTex.SetColor("_PlayerColor", color);
            graphic.material = runtimeMaterialInstanceTex;
        }
        Hide(playerOneColorSelectZone);
    }

    public void SelectPlayerOneTexture(int textureID)
    {
        playerOneTextureId = textureID;
        Texture2D selectedTexture = cellTextures[textureID];

        Graphic playerOneTextureButtonGraphic = playerOneTextureButton.GetComponent<Graphic>();

        Material runtimeMaterialInstance = new Material(playerColorSwap);
        runtimeMaterialInstance.SetColor("_PlayerColor", playerOneColor);
        runtimeMaterialInstance.SetTexture("_MainTex", selectedTexture);
        playerOneTextureButtonGraphic.material = runtimeMaterialInstance;
        Hide(playerOneTextureSelectZone);
    }

    public void TogglePlayerThree()
    {
        Toggle(playerThreeZone);
    }

    public void TogglePlayerFour()
    {
        Toggle(playerFourZone);
    }

    public void ToggleTextureButtons()
    {
        Toggle(playerOneTextureButton.transform.parent.gameObject);
        Toggle(playerTwoTextureButton.transform.parent.gameObject);
        Toggle(playerThreeTextureButton.transform.parent.gameObject);
        Toggle(playerFourTextureButton.transform.parent.gameObject);
    }

    public void UpdateSoundTextValue()
    {
        soundValueText.text = soundSlider.GetComponent<Slider>().value.ToString();
    }

    public void UpdateMusicTextValue()
    {
        musicValueText.text = musicSlider.GetComponent<Slider>().value.ToString();
    }

    private void Show(GameObject toShow)
    {
        toShow.SetActive(true);
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
