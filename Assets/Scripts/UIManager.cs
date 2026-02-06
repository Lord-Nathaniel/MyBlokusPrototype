using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class manage all UI settings and actions.
/// It mainly exchange with the GameManager to display UI from the current state of the game.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private Button playerPieceButtonPrefab;
    [SerializeField] private PlayerPieceDataSO database;
    [SerializeField] private GameObject zone;
    [SerializeField] private PlayerPieceManager playerPieceManager;
    [SerializeField] private Texture2D passButtonTexture;
    [SerializeField] private Color playerColor;
    [SerializeField] private Material playerColorSwap;
    [SerializeField] private Material highlight;
    [SerializeField] private Button nextPlayerButton;

    [Header("Player Settings")]
    [SerializeField] private Image playerPieceImagePrefab;
    [SerializeField] private GameObject playerPiecesSubzonePrefab;
    [SerializeField] private GameObject playerPieceImageZone;

    [Header("Game Settings")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject startingMessage;
    [SerializeField] private Button startMessageButton;
    [SerializeField] private GameObject endingMessage;
    [SerializeField] private Button endMessageButton;

    private int currentPlayer;
    private Button selectedButton;


    private void Start()
    {
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
            gameManager.NextPlayer();
        });
    }

    /// <summary>
    /// Spawn all player piece images in the remaining pieces zone.
    /// </summary>
    public void GenerateRemainingPlayerPieceImages(List<Color> playerColors)
    {
        for (int i = 1; i < playerColors.Count + 1; i++)
        {
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
                mat.SetColor("_PlayerColor", playerColor);
                mat.SetTexture("_MainTex", pieceTexture);

                img.material = mat;
            }
        }
    }

    /// <summary>
    /// Spawn all player piece button and a pass buttons in the player zone.
    /// </summary>
    public void GeneratePlayerPieceButtons(int playerID, Color playerColor, List<int> remainingPlayerPieces)
    {
        if (zone.transform.childCount > 0)
        {
            foreach (Transform child in zone.transform)
            {
                Destroy(child.gameObject);
            }
        }

        foreach (PlayerPieceSO playerPiece in database.playerPieces)
        {
            if (remainingPlayerPieces.Contains(playerPiece.ID))
            {
                Button newButton = Instantiate(playerPieceButtonPrefab, zone.transform, false);
                Image img = newButton.GetComponent<Image>();
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
                mat.SetColor("_PlayerColor", playerColor);
                mat.SetTexture("_MainTex", pieceTexture);

                img.material = mat;

                newButton.onClick.AddListener(() =>
                {
                    SelectButton(newButton);
                    playerPieceManager.StartPlacement(playerPiece.ID, playerID);
                });
            }
        }

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
        passButtonMat.SetTexture("_MainTex", passButtonTexture);
        passButtonImg.material = passButtonMat;
        passButton.onClick.AddListener(() =>
        {
            //TODO to change to pass
            SelectButton(passButton);
        });
    }

    private void SelectButton(Button button)
    {
        if (selectedButton != null)
            Hide(selectedButton.transform.GetChild(0).gameObject);

        selectedButton = button;
        Show(selectedButton.transform.GetChild(0).gameObject);
    }


    /// <summary>
    /// Show all UI elements needed for the start of the game.
    /// </summary>
    public void ShowStartScreen()
    {
        Show(startingMessage);
    }

    /// <summary>
    /// End the start of the game by hidding all UI elements.
    /// </summary>
    public void HideStartScreen()
    {
        Hide(startingMessage);
    }

    /// <summary>
    /// Show all UI elements needed for the end of the game.
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
