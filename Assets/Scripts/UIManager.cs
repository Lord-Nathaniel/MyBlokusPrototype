using System.Collections.Generic;
using TMPro;
using UnityEngine;
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

    [Header("Start & End Settings")]
    [SerializeField] private GameObject startingMessage;
    [SerializeField] private Button startMessageButton;
    [SerializeField] private GameObject endingMessage;
    [SerializeField] private Button endMessageButton;

    private List<Button> pieceButtons = new();
    private Button selectedButton;
    private Color currentPlayerColor;
    private List<GameObject> remainingPlayerPieceSubzones = new();
    private List<Dictionary<int, Image>> remainingPieceImagesPerPlayer = new();

    // Needed services
    private GameManager gameManager;
    private PlayerPieceManager playerPieceManager;
    private GridManager gridManager;

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
        gameManager = ServiceManager.Get<GameManager>();
        playerPieceManager = ServiceManager.Get<PlayerPieceManager>();
        gridManager = ServiceManager.Get<GridManager>();

        startMessageButton.onClick.AddListener(() =>
        {
            gameManager.GameStart();
        });

        endMessageButton.onClick.AddListener(() =>
        {
            gameManager.GameEnd();
        });

        nextPlayerButton.onClick.AddListener(() =>
        {
            gameManager.NextPlayerTurn();
        });
    }

    /// <summary>
    /// Spawn all player piece images in the remaining pieces zone.
    /// -IN- GameManager from InitPlayers()
    /// </summary>
    public void GenerateRemainingPlayerPieceImages(List<Color> playerColors)
    {
        for (int i = 1; i < playerColors.Count + 1; i++)
        {
            Dictionary<int, Image> playerPieceImages = new();
            GameObject currentSubzone = Instantiate(playerPiecesSubzonePrefab, playerPieceImageZone.transform);
            currentSubzone.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "player " + i + " remaining pieces :";
            foreach (PlayerPieceSO playerPiece in database.playerPieces)
            {
                Image img = Instantiate(playerPieceImagePrefab, currentSubzone.transform.GetChild(1));

                Texture2D pieceTexture = playerPiece.miniature;
                if (pieceTexture != null)
                {
                    img.sprite = Sprite.Create(
                        playerPiece.miniature,
                        new Rect(0, 0, playerPiece.miniature.width, playerPiece.miniature.height),
                        new Vector2(0.5f, 0.5f)
                    );
                }

                Material mat = new Material(playerColorSwap);
                mat.SetColor("_PlayerColor", playerColors[i - 1]);
                mat.SetTexture("_MainTex", pieceTexture);

                img.material = mat;

                playerPieceImages.Add(playerPiece.ID, img);
            }
            remainingPieceImagesPerPlayer.Add(playerPieceImages);
            remainingPlayerPieceSubzones.Add(currentSubzone);
        }
    }

    /// <summary>
    /// Spawn all player piece button and a pass buttons in the player zone.
    /// -IN- GameManager from SwitchState()
    /// </summary>
    public void GeneratePlayerPieceButtons(Color playerColor, List<int> remainingPlayerPieces)
    {
        currentPlayerColor = playerColor;
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
            Hide(selectedButton.transform.GetChild(0).gameObject);
            selectedButton = null;
        }

        else if (selectedButton != null)
        {
            Hide(selectedButton.transform.GetChild(0).gameObject);
            playerPieceManager.StopPlacement();
            gridManager.RemoveTempPlayerPiece();

            selectedButton = passButton;
            Show(passButton.transform.GetChild(0).gameObject);
        }

        else
        {
            selectedButton = passButton;
            Show(passButton.transform.GetChild(0).gameObject);
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

            playerPieceManager.StartPlacement(playerPieceID, currentPlayerColor);
            selectedButton = button;
            Show(button.transform.GetChild(0).gameObject);
        }

        else
        {
            playerPieceManager.StartPlacement(playerPieceID, currentPlayerColor);
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
        if (pieceID < 0)
        {
            remainingPlayerPieceSubzones[playerID].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "player " + playerID + " passed and have " + score + "pts.";
            remainingPlayerPieceSubzones[playerID].transform.GetChild(2).gameObject.SetActive(true);
        }
        else
        {
            remainingPlayerPieceSubzones[playerID].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "player " + playerID + " score : " + score + " pts; remaining pieces :";
            if (remainingPieceImagesPerPlayer[playerID].TryGetValue(pieceID, out Image img))
            {
                Destroy(img.gameObject);
                remainingPieceImagesPerPlayer[playerID].Remove(pieceID);
            }
        }
    }

    /// <summary>
    /// Show all UI elements needed for the start of the game.
    /// -IN- GameManager from SwitchState()
    /// </summary>
    public void ShowStartScreen()
    {
        Show(startingMessage);
    }

    /// <summary>
    /// End the start of the game by hidding all UI elements.
    /// -IN- GameManager from SwitchState()
    /// </summary>
    public void HideStartScreen()
    {
        Hide(startingMessage);
    }

    /// <summary>
    /// Show all UI elements needed for the end of the game.
    /// -IN- GameManager from SwitchState()
    /// </summary>
    public void ShowEndScreen()
    {
        Show(endingMessage);
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
