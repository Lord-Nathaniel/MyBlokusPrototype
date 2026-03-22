using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class manages the preview attached to the player cursor.
/// Actions only changes the preview attached to the cursor and have no gameplay interaction.
/// -IN- PlayerPieceManager
/// -OUT- InputManager | PlayerPieceManager
/// </summary>
public class PreviewManager : MonoBehaviour
{
    [SerializeField] private GameObject cellIndicatorParent;
    [SerializeField] private PlayerPieceDataSO database;
    [SerializeField] private Grid grid;

    private PlayerPieceSO playerPieceSO;
    [SerializeField] private float cellIndicatorParentYOffset = 0.015f;
    private Color playerColor;
    private Vector3Int lastDetectedGridPosition = Vector3Int.zero;
    private bool isPlayerColor = true;

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

        cellIndicatorParent.SetActive(false);
    }

    /// <summary>
    /// Create the preview object.
    /// -IN- PlayerPieceManager from StartPlacement()
    /// </summary>
    /// <param name="ID"></param>
    public void StartShowingPlacementPreview(int ID, Color playerColor)
    {
        this.playerColor = playerColor;
        cellIndicatorParent.SetActive(true);
        playerPieceSO = database.playerPieces[ID];
        StartCursorPreview(playerPieceSO.squares, playerPieceSO.corners);
    }

    private void StartCursorPreview(List<Vector2Int> squares, List<Vector2Int> corners)
    {
        ResetCursorPreview();

        foreach (Vector2Int square in squares)
        {
            GameObject squareGO = Instantiate(database.squarePreviewPrefab, cellIndicatorParent.transform.GetChild(0));
            squareGO.transform.localPosition = new Vector3((float)square.x, 0f, (float)square.y);
            squareGO.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            Renderer squareRenderer = squareGO.GetComponentInChildren<Renderer>();
            Color newColor = new Color(playerColor.r, playerColor.g, playerColor.b, 0.5f);
            squareRenderer.material.SetColor("_PlayerColor", newColor);
        }
    }

    private void ResetCursorPreview()
    {
        foreach (Transform child in cellIndicatorParent.transform.GetChild(0))
        {
            Destroy(child.gameObject);
        }
        cellIndicatorParent.transform.position = new Vector3(0f, cellIndicatorParentYOffset, 0f);
        cellIndicatorParent.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        cellIndicatorParent.transform.GetChild(0).rotation = Quaternion.Euler(0f, 0f, 0f);

        Vector3 scale = cellIndicatorParent.transform.GetChild(0).localScale;
        if (scale.x == -1)
        {
            scale.x = 1;
            cellIndicatorParent.transform.GetChild(0).localScale = scale;
        }
    }

    /// <summary>
    /// Rotates 90° clockwise the preview.
    /// -IN- PlayerPieceManager from RotatePlayerPiece()
    /// </summary>
    public void RotatePlacementPreview(int rotationNb)
    {
        cellIndicatorParent.transform.GetChild(0).DORotate(new Vector3(0, rotationNb * 90f, 0), 0.2f);
    }

    /// <summary>
    /// Mirrors the preview.
    /// -IN- PlayerPieceManager from MirrorPlayerPiece()
    /// </summary>
    public void MirrorPlacementPreview(bool isMirrored)
    {
        Vector3 scale = cellIndicatorParent.transform.GetChild(0).localScale;
        scale.x *= -1;
        cellIndicatorParent.transform.GetChild(0).DOScale(new Vector3(scale.x, scale.y, scale.z), 0.2f);
    }

    /// <summary>
    /// -IN- PlayerPieceManager from PlaceStructure()
    /// </summary>
    public void ModifyCursorOpacity(float opacity)
    {
        Transform preview = cellIndicatorParent.transform.GetChild(0);
        for (int i = 0; i < preview.childCount; i++)
        {
            GameObject square = preview.GetChild(i).gameObject;
            Renderer squareRenderer = square.GetComponentInChildren<Renderer>();
            squareRenderer.material.SetFloat("_Opacity", opacity);
        }
    }

    /// <summary>
    /// -IN- PlayerPieceManager from PlaceStructure()
    /// </summary>
    public void ModifyCursorColor(Color color)
    {
        Transform preview = cellIndicatorParent.transform.GetChild(0);
        for (int i = 0; i < preview.childCount; i++)
        {
            GameObject square = preview.GetChild(i).gameObject;
            Renderer squareRenderer = square.GetComponentInChildren<Renderer>();
            squareRenderer.material.SetColor("_PlayerColor", color);
        }
    }

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

        cellIndicatorParent.transform.position = new Vector3(grid.CellToWorld(gridPosition).x,
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
