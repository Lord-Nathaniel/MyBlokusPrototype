using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.Extensions;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.UI;

/// <summary>
/// This class manage all UI settings and actions.
/// It mainly exchange with the GameManager to display UI from the current state of the game.
/// -IN- GameManager
/// -OUT- GameManager | PlayerPieceManager
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("Buttons Settings")]
    [SerializeField] private Button playerPieceButtonPrefab;
    [SerializeField] private PlayerPieceDataSO database;
    [SerializeField] private GameObject zone;
    [SerializeField] private Texture2D passButtonTexture;
    [SerializeField] private Material playerColorSwap;
    [SerializeField] private Material highlight;
    [SerializeField] private Button nextPlayerButton;

    [Header("Images Settings")]
    [SerializeField] private Image playerPieceImagePrefab;
    [SerializeField] private GameObject playerPiecesSubzonePrefab;
    [SerializeField] private GameObject playerPieceImageZone;
    [SerializeField] private GameObject remainingPlayerPiecePrefab;
    [SerializeField] private List<Sprite> remainingPlayerPieces;

    [Header("Start & End Settings")]
    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private GameObject startingMessage;
    [SerializeField] private Button startMessageButton;
    [SerializeField] private GameObject endingMessage;
    [SerializeField] private Button endMessageButton;

    [Header("Material Settings")]
    [SerializeField] private Material fullscreenMaterial;
    [SerializeField] private Material cellHighlightMaterial;

    private List<Button> pieceButtons = new();
    private Button selectedButton;
    private bool isPassButtonSelected = false;
    private Color currentPlayerColor;
    private int currentPlayerID;
    private List<GameObject> remainingPlayerPieceSubzones = new();
    private List<Dictionary<int, GameObject>> remainingPieceImagesPerPlayer = new();

    private PersistentVariablesSource source;
    private List<String> playerScoreSmartStringRefences = new();
    private const string FIRST_PLAYER_SCORE_REF = "firstPlayerScore";
    private const string SECOND_PLAYER_SCORE_REF = "secondPlayerScore";
    private const string THIRD_PLAYER_SCORE_REF = "thirdPlayerScore";
    private const string FOURTH_PLAYER_SCORE_REF = "fourthPlayerScore";

    private const string FIRST_PLAYER_INGAME_SCORE = "FirstPlayerIngameScore";
    private const string SECOND_PLAYER_INGAME_SCORE = "SecondPlayerIngameScore";
    private const string THIRD_PLAYER_INGAME_SCORE = "ThirdPlayerIngameScore";
    private const string FOURTH_PLAYER_INGAME_SCORE = "FourthPlayerIngameScore";
    private const string GAME_SCENE_TABLE = "GameSceneTable";
    private const string PLAYER_COLOR = "_PlayerColor";
    private const string INLINE_COLOR = "_InlineColor";

    // Needed services
    private GameManager gameManager;
    private PlayerPieceManager playerPieceManager;
    private GridManager gridManager;
    private SoundManager soundManager;

    private void Awake()
    {
        ServiceManager.Register(this);
        source = LocalizationSettings.StringDatabase.SmartFormatter.GetSourceExtension<PersistentVariablesSource>();
    }

    private void OnDestroy()
    {
        ServiceManager.Unregister<GameManager>();
    }

    private void Start()
    {
        gameManager = ServiceManager.Get<GameManager>();
        playerPieceManager = ServiceManager.Get<PlayerPieceManager>();
        gridManager = ServiceManager.Get<GridManager>();
        soundManager = ServiceManager.Get<SoundManager>();

        startMessageButton.onClick.AddListener(() =>
        {
            startMessageButton.transform.DOScale(1.1f, 0.1f)
                                        .SetEase(Ease.OutBounce)
                                        .OnComplete(() => StartAction());
        });

        endMessageButton.onClick.AddListener(() =>
        {
            endMessageButton.transform.DOScale(1.1f, 0.1f)
                                      .SetEase(Ease.OutBounce)
                                      .OnComplete(() => EndGameAction());
        });

        nextPlayerButton.onClick.AddListener(() =>
        {
            AnimateButton(nextPlayerButton);
            NextPlayerAction();
        });

        playerScoreSmartStringRefences.Add(FIRST_PLAYER_SCORE_REF);
        playerScoreSmartStringRefences.Add(SECOND_PLAYER_SCORE_REF);
        playerScoreSmartStringRefences.Add(THIRD_PLAYER_SCORE_REF);
        playerScoreSmartStringRefences.Add(FOURTH_PLAYER_SCORE_REF);
    }

    private void AnimateButton(Button button)
    {
        button.transform.DOScale(1.2f, 0.2f)
                        .SetEase(Ease.OutBounce)
                        .OnComplete(() => button.transform.DOScale(1f, 0.2f));
    }

    private void NextPlayerAction()
    {
        if (selectedButton == null)
            return;

        if (isPassButtonSelected)
        {
            gameManager.NextPlayerTurn(true);
        }
        else
        {
            gameManager.NextPlayerTurn(false);
        }
    }

    private void EndGameAction()
    {
        soundManager.PlaySound(SoundType.CassetteRecordClick);
        gameManager.GameEnd();
    }

    private void StartAction()
    {
        soundManager.PlaySound(SoundType.CassetteRecordClick);
        gameManager.GameStart();
    }

    /// <summary>
    /// Spawn all player piece images in the remaining pieces zone.
    /// -IN- GameManager from ProtoInitPlayers()
    /// </summary>
    public void GenerateRemainingPlayerPieceImages(List<Color> playerColors)
    {
        List<String> stringReference = new();
        stringReference.Add(FIRST_PLAYER_INGAME_SCORE);
        stringReference.Add(SECOND_PLAYER_INGAME_SCORE);
        stringReference.Add(THIRD_PLAYER_INGAME_SCORE);
        stringReference.Add(FOURTH_PLAYER_INGAME_SCORE);
        for (int i = 1; i < playerColors.Count + 1; i++)
        {
            Dictionary<int, GameObject> playerPieceImages = new();
            GameObject currentSubzone = Instantiate(playerPiecesSubzonePrefab, playerPieceImageZone.transform);
            //text setup
            currentSubzone.transform.GetChild(0).GetComponent<LocalizeStringEvent>().StringReference.SetReference(GAME_SCENE_TABLE, stringReference[i - 1]);
            //backgroud setup
            Material mat = new Material(playerColorSwap);
            mat.SetColor("_PlayerColor", playerColors[i - 1]);
            currentSubzone.transform.GetChild(1).GetComponent<Image>().material = mat;
            //images setup
            Transform imageZone = currentSubzone.transform.GetChild(2);
            for (int j = 0; j < remainingPlayerPieces.Count; j++)
            {
                GameObject img = Instantiate(remainingPlayerPiecePrefab, imageZone);
                img.GetComponent<Image>().sprite = remainingPlayerPieces[j];
                playerPieceImages.Add(j, img);
            }
            remainingPieceImagesPerPlayer.Add(playerPieceImages);
            remainingPlayerPieceSubzones.Add(currentSubzone);
        }
    }

    /// <summary>
    /// Spawn all player piece button and a pass buttons in the player zone.
    /// -IN- GameManager from SwitchState()
    /// </summary>
    public void GeneratePlayerPieceButtons(Color playerColor, List<int> remainingPlayerPieces, int playerID)
    {
        currentPlayerColor = playerColor;
        currentPlayerID = playerID;
        if (zone.transform.childCount > 0)
        {
            foreach (Transform child in zone.transform)
            {
                Destroy(child.gameObject);
            }
            pieceButtons = new();
        }

        foreach (PlayerPieceSO playerPiece in database.playerPieces)
        {
            if (remainingPlayerPieces.Contains(playerPiece.ID))
            {
                Button newButton = GenerateNewPieceButton(playerColor, playerPiece);
                newButton.onClick.AddListener(() =>
                {
                    OnClickPieceAction(newButton, playerPiece.ID);
                });
            }
        }

        Button passButton = GeneratePassButton(playerColor);
        passButton.onClick.AddListener(() =>
        {
            OnClickPassAction(passButton);
        });
    }

    private Button GeneratePassButton(Color playerColor)
    {
        isPassButtonSelected = false;
        Button passButton = Instantiate(playerPieceButtonPrefab, zone.transform, false);
        Image passButtonImg = passButton.GetComponent<Image>();
        if (passButtonTexture != null)
        {
            passButtonImg.sprite = Sprite.Create(
                passButtonTexture,
                new Rect(0, 0, passButtonTexture.width, passButtonTexture.height),
                new Vector2(0.5f, 0.5f)
            );
        }
        Material passButtonMat = new Material(playerColorSwap);
        passButtonMat.SetColor("_PlayerColor", playerColor);
        passButtonMat.SetTexture("_MainTexture", passButtonTexture);
        passButtonImg.material = passButtonMat;

        pieceButtons.Add(passButton);
        return passButton;
    }

    private void OnClickPassAction(Button passButton)
    {
        if (selectedButton == passButton)
        {
            soundManager.PlaySound(SoundType.Cancel);
            Hide(selectedButton.transform.GetChild(0).gameObject);
            selectedButton = null;
            isPassButtonSelected = false;
        }

        else if (selectedButton != null)
        {
            soundManager.PlaySound(SoundType.Notification);
            Hide(selectedButton.transform.GetChild(0).gameObject);
            playerPieceManager.StopPlacement();
            gridManager.RemoveTempPlayerPiece();

            selectedButton = passButton;
            Show(passButton.transform.GetChild(0).gameObject);
            isPassButtonSelected = true;
        }

        else
        {
            soundManager.PlaySound(SoundType.Notification);
            selectedButton = passButton;
            Show(passButton.transform.GetChild(0).gameObject);
            isPassButtonSelected = true;
        }
    }

    private Button GenerateNewPieceButton(Color playerColor, PlayerPieceSO playerPiece)
    {
        Button newButton = Instantiate(playerPieceButtonPrefab, zone.transform, false);
        Image img = newButton.GetComponent<Image>();
        Texture2D pieceTexture = playerPiece.miniature;
        if (pieceTexture != null)
        {
            img.sprite = Sprite.Create(
                pieceTexture,
                new Rect(0, 0, pieceTexture.width, pieceTexture.height),
                new Vector2(0.5f, 0.5f)
            );
        }

        Material mat = new Material(playerColorSwap);
        mat.SetColor("_PlayerColor", playerColor);
        mat.SetTexture("_MainTexture", pieceTexture);

        img.material = mat;

        pieceButtons.Add(newButton);
        return newButton;
    }

    private void OnClickPieceAction(Button button, int playerPieceID)
    {
        if (selectedButton == button)
        {
            soundManager.PlaySound(SoundType.Cancel);
            Hide(selectedButton.transform.GetChild(0).gameObject);
            playerPieceManager.StopPlacement();
            gridManager.RemoveTempPlayerPiece();
            selectedButton = null;
        }

        else if (selectedButton != null)
        {
            Hide(selectedButton.transform.GetChild(0).gameObject);
            playerPieceManager.StopPlacement();
            gridManager.RemoveTempPlayerPiece();

            soundManager.PlaySound(SoundType.Cancel);
            playerPieceManager.StartPlacement(playerPieceID, currentPlayerColor, currentPlayerID);
            selectedButton = button;
            Show(button.transform.GetChild(0).gameObject);
        }
        else
        {
            soundManager.PlaySound(SoundType.Click);
            playerPieceManager.StartPlacement(playerPieceID, currentPlayerColor, currentPlayerID);
            selectedButton = button;
            Show(button.transform.GetChild(0).gameObject);
        }
    }

    /// <summary>
    /// Update the player pieces lists
    /// </summary>
    /// <param name="playerID"></param>
    /// <param name="pieceID"></param>
    /// <param name="score"></param>
    public void UpdateRemainingPlayerPieceImages(int playerID, int pieceID, int score)
    {
        if (pieceID >= 0)
        {
            var playerScoreVar = source["global"][playerScoreSmartStringRefences[playerID]] as IntVariable;
            playerScoreVar.Value = score;

            if (remainingPieceImagesPerPlayer[playerID].TryGetValue(pieceID, out GameObject img))
            {
                Destroy(img.gameObject);
                remainingPieceImagesPerPlayer[playerID].Remove(pieceID);
            }
        }
    }

    /// <summary>
    /// Update the screen border with the current player color.
    /// -IN- GameManager from SwitchState()
    /// </summary>
    public void UpdateScreenColor(Color playerColor)
    {
        fullscreenMaterial.SetColor(PLAYER_COLOR, playerColor);
        cellHighlightMaterial.SetColor(INLINE_COLOR, playerColor);
    }

    /// <summary>
    /// Show all UI elements needed for the start of the game.
    /// -IN- GameManager from SwitchState()
    /// </summary>
    public void ShowStartScreen()
    {
        soundManager.PlaySound(SoundType.Notification);
        Show(startingMessage);
        Hide(gameCanvas);
    }

    /// <summary>
    /// End the start of the game by hidding all UI elements.
    /// -IN- GameManager from SwitchState()
    /// </summary>
    public void HideStartScreen()
    {
        Hide(startingMessage);
        Show(gameCanvas);
    }

    /// <summary>
    /// Show all UI elements needed for the end of the game.
    /// -IN- GameManager from SwitchState()
    /// </summary>
    public void ShowEndScreen()
    {
        soundManager.PlaySound(SoundType.SucessFanfare);
        Show(endingMessage);
        Hide(gameCanvas);
    }

    private void Show(GameObject toShow)
    {
        toShow.SetActive(true);
    }

    private void Hide(GameObject toHide)
    {
        toHide.SetActive(false);
    }
}
