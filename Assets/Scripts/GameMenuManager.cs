using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// This class manages the escape menu during the game.
/// At start, it take the optionsSetting of the PlayerSetup and apply them to the correct zone.
/// -OUT- InputManager | PreviewManager 
/// </summary>
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
    private SoundManager soundManager;

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
        soundManager = ServiceManager.Get<SoundManager>();
        inputManager.OnEscapeClicked += ShowMenu;

        menuCloseButton.onClick.AddListener(() =>
        {
            CloseOptionsAction();
        });

        menuOptionsButton.onClick.AddListener(() =>
        {
            OpenOptionsAction();
        });

        menuMainMenuButton.onClick.AddListener(() =>
        {
            GoToMenuScene();
        });
    }

    private void OpenOptionsAction()
    {
        soundManager.PlaySound(SoundType.ButtonPressed);
        Toggle(optionsZone);
    }

    private void CloseOptionsAction()
    {
        soundManager.PlaySound(SoundType.ButtonPressed);
        HideMenu();
    }

    private void GoToMenuScene()
    {
        soundManager.PlaySound(SoundType.CassetteRecord);
        SceneManager.LoadScene(MENU_SCENE);
    }

    private void ShowMenu()
    {
        soundManager.PlaySound(SoundType.Undo);
        Toggle(canvasMain);
        Toggle(menuZone);
        previewManager.ModifyCursorOpacity(0f);
    }

    private void HideMenu()
    {
        soundManager.PlaySound(SoundType.Undo);
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
