using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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

    private List<Color> playerColors = new();
    private List<Texture2D> playerTextures = new();
    private Color playerOneColor;
    private Color playerTwoColor;
    private Color playerThreeColor;
    private Color playerFourColor;


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
        playerColors.Add(new Color(0.8f, 0.8f, 0.8f));
        //playerColors.Add(new Color(0.033f, 0, 1f));
        playerColors.Add(new Color(1f, 0, 0));
        playerColors.Add(new Color(1f, 1f, 0));
        playerColors.Add(new Color(0.5f, 0, 0.5f));
        playerColors.Add(new Color(0.2f, 0.8f, 0.2f));
        playerColors.Add(new Color(1f, 0.633f, 0));
        playerColors.Add(new Color(0, 1f, 1f));
        playerColors.Add(new Color(1f, 0.42f, 0.71f));

        gameButton.onClick.AddListener(() =>
        {
            GameAction();
        });

        gameStartButton.onClick.AddListener(() =>
        {
            string value = playerOneInputField.text;
        });

        optionsButton.onClick.AddListener(() =>
        {
            OptionAction();
        });

        optionsCloseButton.onClick.AddListener(() =>
        {
            Hide(optionsZone);
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

    private void ToggleLanguage()
    {
        throw new NotImplementedException();
    }

    private void OptionAction()
    {
        Hide(gameZone);
        Toggle(optionsZone);
    }

    private void GameAction()
    {
        Hide(optionsZone);
        Toggle(gameZone);
    }

    public void SelectPlayerOneColor(int colorID)
    {
        Debug.Log("Selected colorID: " + colorID);
        //Color color = playerColors[colorID];
        //playerOneColor = color;
        //playerOneColorButton.image.color = color;


        //playerOneTextureButton.image.material.color = color;

        //Debug.Log(playerOneTextureSelectZone);
        //foreach (Button button in playerOneTextureSelectZone.GetComponentsInChildren<Button>())
        //{
        //    Image img = button.GetComponent<Image>();

        //    Debug.Log(img.material);
        //    Debug.Log(img.material.shader.name);
        //    Debug.Log(img.material.HasProperty("_PlayerColor"));

        //    //Texture2D pieceTexture = img.sprite.texture;
        //    //if (pieceTexture != null)
        //    //{
        //    //    img.sprite = Sprite.Create(
        //    //        pieceTexture,
        //    //        new Rect(0, 0, pieceTexture.width, pieceTexture.height),
        //    //        new Vector2(0.5f, 0.5f)
        //    //    );
        //    //}

        //    //Material mat = new Material(playerColorSwap);
        //    //mat.SetColor("_PlayerColor", color);
        //    //mat.SetTexture("_MainTex", pieceTexture);

        //    //img.material = mat;

        //    //Image img = button.GetComponent<Image>();

        //    //if (img.material != null && img.material.HasProperty("_PlayerColor"))
        //    //{
        //    //    img.material.SetColor("_PlayerColor", color);
        //    //}
        //}
        Hide(playerOneColorSelectZone);
    }

    public void SelectPlayerOneTexture(int textureID)
    {
        Debug.Log("Selected textureID: " + textureID);
        //Texture2D texture = cellTextures[textureID];
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
