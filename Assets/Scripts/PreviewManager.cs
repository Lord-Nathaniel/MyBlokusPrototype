using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class manages the preview attached to the player cursor.
/// Actions only changes the preview attached to the cursor and have no gameplay interaction.
/// -IN- PlayerPieceManager | GameMenuManager
/// -OUT- InputManager | PlayerPieceManager
/// </summary>
public class PreviewManager : MonoBehaviour
{
    [SerializeField] private GameObject cursorIndicatorParent;
    [SerializeField] private PlayerPieceDataSO database;
    [SerializeField] private Grid grid;
    [SerializeField] private float cellIndicatorParentYOffset = 0.015f;

    private PlayerPieceSO playerPieceSO;
    private Color playerColor;
    private Vector3Int lastDetectedGridPosition = Vector3Int.zero;
    private bool isPlayerColor = true;
    private Tween shakeTween;
    private Vector3 positionBeforeTween;

    private const string PLAYER_COLOR = "_PlayerColor";

    // Needed services
    private InputManager inputManager;
    private PlayerPieceManager playerPieceManager;

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
        inputManager = ServiceManager.Get<InputManager>();
        playerPieceManager = ServiceManager.Get<PlayerPieceManager>();

        cursorIndicatorParent.SetActive(false);
    }

    /// <summary>
    /// Create the preview object.
    /// -IN- PlayerPieceManager from StartPlacement()
    /// </summary>
    /// <param name="pieceID"></param>
    /// <param name="playerColor"></param>
    public void StartShowingPlacementPreview(int pieceID, Color playerColor)
    {
        this.playerColor = playerColor;
        cursorIndicatorParent.SetActive(true);
        playerPieceSO = database.playerPieces[pieceID];
        StartCursorPreview(playerPieceSO.squares, playerPieceSO.corners);
    }

    private void StartCursorPreview(List<Vector2Int> squares, List<Vector2Int> corners)
    {
        ResetCursorPreview();

        foreach (Vector2Int square in squares)
        {
            GameObject squareGO = Instantiate(database.squarePreviewPrefab, cursorIndicatorParent.transform.GetChild(0));
            squareGO.transform.localPosition = new Vector3((float)square.x, 0f, (float)square.y);
            squareGO.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            Renderer squareRenderer = squareGO.GetComponentInChildren<Renderer>();
            Color newColor = new Color(playerColor.r, playerColor.g, playerColor.b, 0.5f);
            squareRenderer.material.SetColor(PLAYER_COLOR, newColor);
        }
    }

    private void ResetCursorPreview()
    {
        foreach (Transform child in cursorIndicatorParent.transform.GetChild(0))
        {
            Destroy(child.gameObject);
        }
        cursorIndicatorParent.transform.position = new Vector3(0f, cellIndicatorParentYOffset, 0f);
        cursorIndicatorParent.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        cursorIndicatorParent.transform.GetChild(0).rotation = Quaternion.Euler(0f, 0f, 0f);

        Vector3 scale = cursorIndicatorParent.transform.GetChild(0).localScale;
        if (scale.x == -1)
        {
            scale.x = 1;
            cursorIndicatorParent.transform.GetChild(0).localScale = scale;
        }
    }

    /// <summary>
    /// Rotates 90° clockwise the preview.
    /// -IN- PlayerPieceManager from RotatePlayerPiece()
    /// /// <param name="rotationNb"></param>
    /// </summary>
    public void RotatePlacementPreview(int rotationNb)
    {
        cursorIndicatorParent.transform.GetChild(0).DORotate(new Vector3(0, rotationNb * 90f, 0), 0.2f);
    }

    /// <summary>
    /// Mirrors the preview.
    /// -IN- PlayerPieceManager from MirrorPlayerPiece()
    /// <param name="isMirrored"></param>
    /// </summary>
    public void MirrorPlacementPreview(bool isMirrored)
    {
        Vector3 scale = cursorIndicatorParent.transform.GetChild(0).localScale;
        scale.x *= -1;
        cursorIndicatorParent.transform.GetChild(0).DOScale(new Vector3(scale.x, scale.y, scale.z), 0.2f);
    }

    /// <summary>
    /// Modify the preview opacity.
    /// -IN- PlayerPieceManager from PlaceStructure(), GameMenuManager from ShowMenu() and HideMenu()
    /// /// <param name="opacity"></param>
    /// </summary>
    public void ModifyCursorOpacity(float opacity)
    {
        Transform preview = cursorIndicatorParent.transform.GetChild(0);
        for (int i = 0; i < preview.childCount; i++)
        {
            GameObject square = preview.GetChild(i).gameObject;
            Renderer squareRenderer = square.GetComponentInChildren<Renderer>();
            squareRenderer.material.SetFloat("_Opacity", opacity);
        }
    }

    /// <summary>
    /// Shakes the preview when clicking and having a wrong placement.
    /// -IN- PlayerPieceManager from PlaceStructure()
    /// </summary>
    public void AnimatePreviewWrongPlacement()
    {
        Transform preview = cursorIndicatorParent.transform.GetChild(0);

        if (shakeTween != null)
        {
            shakeTween?.Kill();
        }
        else
        {
            positionBeforeTween = preview.localPosition;
        }

        shakeTween = preview.DOShakePosition(0.6f, new Vector3(1f, 0f, 0f), 10, 90f, false, false, ShakeRandomnessMode.Full)
                            .OnComplete(() => preview.localPosition = positionBeforeTween);
    }

    /// <summary>
    /// Scale down the preview when clicking and having a good placement.
    /// -IN- PlayerPieceManager from PlaceStructure()
    /// </summary>
    public void AnimatePreviewGoodPlacement()
    {
        Transform preview = cursorIndicatorParent.transform.GetChild(0);
        preview.DOScale(0.9f, 0.4f)
               .SetEase(Ease.OutBounce)
               .OnComplete(() => preview.transform.DOScale(1f, 0.05f));
    }

    /// <summary>
    /// Hide the preview when stopping piece placement.
    /// -IN- PlayerPieceManager from StopPlacement()
    /// </summary>
    public void StopShowingPreview()
    {
        ResetCursorPreview();
    }

    private void Update()
    {
        if (playerPieceSO == null)
            return;
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        cursorIndicatorParent.transform.position = new Vector3(grid.CellToWorld(gridPosition).x,
                                                             cellIndicatorParentYOffset,
                                                             grid.CellToWorld(gridPosition).z);

        if (lastDetectedGridPosition != gridPosition)
        {
            lastDetectedGridPosition = gridPosition;
            if (!playerPieceManager.CheckPlacementValidity(mousePosition, false))
            {
                ModifyCursorOpacity(0.5f);
                isPlayerColor = false;
            }
            else if (!isPlayerColor)
            {
                ModifyCursorOpacity(1f);
                isPlayerColor = true;
            }
        }
    }
}
