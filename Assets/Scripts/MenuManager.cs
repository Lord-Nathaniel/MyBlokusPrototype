using System.Collections.Generic;
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

    [Header("Player One Settings")]
    [SerializeField] private Button playerOneColorButton;
    [SerializeField] private Button playerOneTextureButton;
    [SerializeField] private InputField playerOneInputField;
    [SerializeField] private GameObject playerOneColorSelectZone;
    [SerializeField] private GameObject playerOneTextureSelectZone;

    [Header("Options Settings")]
    [SerializeField] private GameObject optionsZone;
    [SerializeField] private Button optionsCloseButton;

    private Color playerOneColor;
    private List<Color> playerColors = new();
    private List<Texture2D> playerTextures = new();


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
        playerColors.Add(new Color(0.033f, 0, 1f));
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
            //TODO
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
        Color color = playerColors[colorID];
        playerOneColor = color;
        playerOneColorButton.image.color = color;
        Hide(playerOneColorSelectZone);
    }

    public void SelectPlayerOneTexture(int textureID)
    {
        Texture2D texture = playerTextures[textureID];
        Hide(playerOneTextureSelectZone);
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
