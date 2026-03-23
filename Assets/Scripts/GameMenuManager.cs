using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// This class manages the escape menu during the game.
/// At start, it take the optionsSetting of the PlayerSetup and apply them to the correct zone.
/// -OUT- InputManager | PreviewManager | SoundManager | GameManager 
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
    private GameManager gameManager;

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
        gameManager = ServiceManager.Get<GameManager>();
        inputManager.OnEscapeClicked += ShowMenu;

        menuCloseButton.onClick.AddListener(() =>
        {
            menuCloseButton.transform.DOScale(1.1f, 0.1f)
                                     .SetEase(Ease.OutBounce)
                                     .OnComplete(() => CloseOptionsAction());
        });

        menuOptionsButton.onClick.AddListener(() =>
        {
            AnimateButton(menuOptionsButton);
            OpenOptionsAction();
        });

        menuMainMenuButton.onClick.AddListener(() =>
        {
            menuMainMenuButton.transform.DOScale(1.1f, 0.1f)
                                        .SetEase(Ease.OutBounce)
                                        .OnComplete(() => GoToMenuScene());
        });
    }

    private void AnimateButton(Button button)
    {
        button.transform.DOScale(1.2f, 0.2f)
                        .SetEase(Ease.OutBounce)
                        .OnComplete(() => button.transform.DOScale(1f, 0.2f));
    }

    private void OpenOptionsAction()
    {
        soundManager.PlaySound(SoundType.ButtonPress);
        Toggle(optionsZone);
    }

    private void CloseOptionsAction()
    {
        soundManager.PlaySound(SoundType.ButtonPress);
        HideMenu();
    }

    private void GoToMenuScene()
    {
        soundManager.PlaySound(SoundType.CassetteRecordClick);
        gameManager.GameEnd();
        SceneManager.LoadScene(MENU_SCENE);
    }

    private void ShowMenu()
    {
        if (!menuZone.activeInHierarchy)
        {
            soundManager.PlaySound(SoundType.Undo);
            Toggle(canvasMain);
            Toggle(menuZone);
            previewManager.ModifyCursorOpacity(0f);
        }
        else
        {
            HideMenu();
        }
    }

    private void HideMenu()
    {
        soundManager.PlaySound(SoundType.Undo);
        Toggle(canvasMain);
        Toggle(menuZone);
        if (optionsZone.activeInHierarchy)
            Toggle(optionsZone);
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
