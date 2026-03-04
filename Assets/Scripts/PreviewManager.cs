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

    private GameObject previewObject;
    private PlayerPieceSO playerPieceSO;
    [SerializeField] private float cellIndicatorParentYOffset = 0.015f;
    private Color playerColor;
    private Vector3Int lastDetectedGridPosition = Vector3Int.zero;

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
            squareRenderer.material.SetColor("_PlayerColor", playerColor);
        }

        foreach (Vector2Int corner in corners)
        {
            GameObject cornerGO = Instantiate(database.cornerPreviewPrefab, cellIndicatorParent.transform.GetChild(0));
            cornerGO.transform.localPosition = new Vector3((float)corner.x, 0f, (float)corner.y);
            cornerGO.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
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
    }

    /// <summary>
    /// Rotates 90° clockwise the preview.
    /// -IN- PlayerPieceManager from RotatePlayerPiece()
    /// </summary>
    public void RotatePlacementPreview()
    {
        cellIndicatorParent.transform.GetChild(0).Rotate(0f, 90f, 0f);
    }

    /// <summary>
    /// Mirrors the preview.
    /// -IN- PlayerPieceManager from MirrorPlayerPiece()
    /// </summary>
    public void MirrorPlacementPreview()
    {
        Vector3 scale = cellIndicatorParent.transform.GetChild(0).localScale;
        scale.x *= -1;
        cellIndicatorParent.transform.GetChild(0).localScale = scale;
    }

    /// <summary>
    /// -IN- PlayerPieceManager from PlaceStructure()
    /// </summary>
    public void ModifyCursorOpacity()
    {
        //foreach (GameObject cursorSquare in cellIndicatorParent.transform.GetChild(0))
        //{
        //    cursorSquare.transform.GetChild(0).gameObject.SetActive(true);
        //}
    }

    /// <summary>
    /// -IN- PlayerPieceManager from PlaceStructure()
    /// </summary>
    public void ModifyCursorColor()
    {
        //foreach (GameObject cursorSquare in cellIndicatorParent.transform.GetChild(0))
        //{
        //    cursorSquare.transform.GetChild(0).gameObject.SetActive(true);
        //}
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
            if (!playerPieceManager.CheckPlacementValidity(mousePosition))
            {
                ModifyCursorColor();
            }
        }
    }
}
