using UnityEngine;

/// <summary>
/// This class manages the player piece element data during the player turn.
/// -IN- GameManager
/// -OUT- InputManager | GridManager | PreviewManager
/// </summary>
public class PlayerPieceManager : MonoBehaviour
{
    [SerializeField] private GameObject cellIndicatorParent;
    [SerializeField] private Grid grid;
    [SerializeField] private PlayerPieceDataSO database;
    [SerializeField] private GameObject gridVisualization;

    [SerializeField] private Color playerOneColor;
    [SerializeField] private Color playerTwoColor;
    [SerializeField] private Color playerThreeColor;
    [SerializeField] private Color playerFourColor;

    private int selectedObjectIndex = -1;
    private int selectedObjectRotation = 0;
    private bool isSelectedObjectMirrored = false;
    private bool isFirstPlacedPiece = true;
    private int playerNb = 2;
    private int playerID = 1;
    private bool isPiecePlaced = false;
    private Color selectedColor;

    // Needed services
    private InputManager inputManager;
    private GridManager gridManager;
    private PreviewManager previewManager;

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
        gridManager = ServiceManager.Get<GridManager>();
        previewManager = ServiceManager.Get<PreviewManager>();

        //TODO remove start because should be called for each player.
        StopPlacement();
        gridManager.PlaceStartCell(playerNb);

        //floorData = new();
        //furnitureData = new();d
    }

    /// <summary>
    /// State of the game where playerpiece preview is shown and other things disabbled when a player use the select button.
    /// </summary>
    /// <param name="pieceID"></param>
    public void StartPlacement(int pieceID, int playerID)
    {
        StopPlacement();
        int previousObjectIndex = -1;
        if (selectedObjectIndex > -1)
        {
            previousObjectIndex = selectedObjectIndex;
        }

        selectedObjectIndex = database.playerPieces.FindIndex(data => data.ID == pieceID);
        if (selectedObjectIndex > -1)
        {
            if (isPiecePlaced)
            {
                Debug.Log(("[PlayerPieceManager] previous objectIndex = ", previousObjectIndex, " | selected object index = ", selectedObjectIndex));
                gridManager.RemovePlayerPiece(previousObjectIndex);
            }


            selectedObjectRotation = 0;
            isSelectedObjectMirrored = false;
            isPiecePlaced = false;

            switch (playerID)
            {
                case 0:
                    selectedColor = playerOneColor; break;
                case 1:
                    selectedColor = playerTwoColor; break;
                case 2:
                    selectedColor = playerThreeColor; break;
                case 3:
                    selectedColor = playerFourColor; break;
            }
            previewManager.StartShowingPlacementPreview(database.playerPieces[selectedObjectIndex].ID, selectedColor);
        }
        else
        {
            return;
        }
        gridVisualization.SetActive(true);
        inputManager.OnLeftClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
        inputManager.OnRightClicked += RotatePlayerPiece;
        inputManager.OnMiddleClicked += MirrorPlayerPiece;
    }

    public void StartRemoving()
    {
        //StopPlacement();
        //gridVisualization.SetActive(true);
        //previewManager.StartShowingRemovePreview();
        //inputManager.OnClicked += PlaceStructure;
        //inputManager.OnExit += StopPlacement;
    }

    /// <summary>
    /// State of the game where the gridManager is called to know if writing is legal.
    /// </summary>
    public void PlaceStructure()
    {
        if (inputManager.IsPointerOverUI())
            return;
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();

        bool placementValidity = CheckPlacementValidity(mousePosition, selectedObjectIndex, selectedObjectRotation, isSelectedObjectMirrored, isFirstPlacedPiece);
        if (!placementValidity)
        {
            //soundManager.PlaySound(SoundType.WrongPlacement);
            return;
        }

        if (isPiecePlaced)
        {
            gridManager.RemovePlayerPiece(selectedObjectIndex);
        }

        previewManager.ModifyCursorColorAndOpacity(Color.white, 0.5f);

        gridManager.AddPlayerPiece(selectedObjectIndex, playerID, selectedColor);
        if (!isPiecePlaced)
            isPiecePlaced = true;
        if (isFirstPlacedPiece)
            isFirstPlacedPiece = false;

        //int index = squarePlacer.PlaceObject(database.objectsData[selectedObjectIndex].Prefab, grid.CellToWorld(gridPosition));

        //GridManager selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? floorData : furnitureData;
        //if (selectedData == floorData)
        //{
        //    soundManager.PlaySound(SoundType.PlaceFloor);
        //}
        //else
        //{
        //    soundManager.PlaySound(SoundType.PlaceFurniture);
        //}
        //selectedData.AddObjectAt(gridPosition,
        //                         database.objectsData[selectedObjectIndex].Size,
        //                         database.objectsData[selectedObjectIndex].ID,
        //                         index);

        //previewManager.UpdatePosition(grid.CellToWorld(gridPosition), false);
    }

    private bool CheckPlacementValidity(Vector3 mousePosition, int selectedObjectIndex, int selectedObjectRotation, bool isSelectedObjectMirrored, bool isFirstPlacedPiece)
    {
        return gridManager.CanPlaceObjectAt(mousePosition,
                                            database.playerPieces[selectedObjectIndex].ID,
                                            selectedObjectRotation,
                                            isSelectedObjectMirrored,
                                            isFirstPlacedPiece);
    }


    /// <summary>
    /// This helps to rememeber the current state of piece rotation.
    /// </summary>
    public void RotatePlayerPiece()
    {
        if (selectedObjectIndex > -1 && database.playerPieces[selectedObjectIndex].rotable)
        {
            selectedObjectRotation = (selectedObjectRotation + 1) % 4;
            previewManager.RotatePlacementPreview();
        }
    }

    /// <summary>
    /// This helps to rememeber the current state of piece mirroring.
    /// </summary>
    public void MirrorPlayerPiece()
    {
        if (selectedObjectIndex > -1 && database.playerPieces[selectedObjectIndex].mirrorable)
        {
            isSelectedObjectMirrored = !isSelectedObjectMirrored;
            previewManager.MirrorPlacementPreview();
        }
    }

    /// <summary>
    /// State of the game where player turn should either end or go back to placement state.
    /// </summary>
    public void StopPlacement()
    {
        //if (!isBuidling)
        //{
        //    return;
        //}
        //selectedObjectIndex = -1;
        gridVisualization.SetActive(false);
        cellIndicatorParent.SetActive(false);
        //previewManager.StopShowingPreview();
        inputManager.OnLeftClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        inputManager.OnRightClicked -= RotatePlayerPiece;
        inputManager.OnMiddleClicked -= MirrorPlayerPiece;
    }
    //lastDetectedPosition = Vector3Int.zero;
    //isBuidling = false;

    /// <summary>
    /// -IN- GameManager from NextPlayer()
    /// </summary>
    public bool IsPlayerPiecePlaced()
    {
        if (selectedObjectIndex >= 0)
        {
            gridManager.SaveCurrentPiece();
            return true;
        }
        return false;
    }
}
