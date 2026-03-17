using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenuManager : MonoBehaviour
{
    [Header("Menu Settings")]
    [SerializeField] private GameObject canvasMain;
    [SerializeField] private GameObject menuZone;
    [SerializeField] private GameObject optionsZone;
    [SerializeField] private Button menuCloseButton;
    [SerializeField] private Button menuOptionsButton;
    [SerializeField] private Button menuMainMenuButton;

    const string MENU_SCENE = "MenuScene";

    // Needed services
    private InputManager inputManager;
    private PreviewManager previewManager;

    private void Awake()
    {
        ServiceManager.Register(this);
    }

    private void OnDestroy()
    {
        ServiceManager.Unregister<GameMenuManager>();
        inputManager.OnEscapeClicked -= ShowMenu;
    }

    private void Start()
    {
        inputManager = ServiceManager.Get<InputManager>();
        previewManager = ServiceManager.Get<PreviewManager>();
        inputManager.OnEscapeClicked += ShowMenu;

        menuCloseButton.onClick.AddListener(() =>
        {
            HideMenu();
        });

        menuOptionsButton.onClick.AddListener(() =>
        {
            Toggle(optionsZone);
        });

        menuMainMenuButton.onClick.AddListener(() =>
        {
            GoToMenuScene();
        });
    }

    private static void GoToMenuScene()
    {
        SceneManager.LoadScene(MENU_SCENE);
    }

    private void ShowMenu()
    {
        Toggle(canvasMain);
        Toggle(menuZone);
        previewManager.ModifyCursorOpacity(0f);
    }

    private void HideMenu()
    {
        Toggle(canvasMain);
        Toggle(menuZone);
        previewManager.ModifyCursorOpacity(1f);
    }

    private void Toggle(GameObject toToggle)
    {
        if (toToggle.activeInHierarchy)
            toToggle.SetActive(false);
        else
            toToggle.SetActive(true);
    }
}
