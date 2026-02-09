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

    private int selectedObjectID = -1;
    private int selectedObjectRotation = 0;
    private bool isSelectedObjectMirrored = false;
    private bool isFirstPlacedPiece = true;
    private int playerNb = 4;
    private int playerID = 1;
    private bool isPiecePlaced = false;
    private Color currentPlayerColor;

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
    }

    /// <summary>
    /// State of the game where playerpiece preview is shown and other things disabbled when a player use the select button.
    /// </summary>
    /// <param name="pieceID"></param>
    public void StartPlacement(int pieceID, Color playerColor)
    {
        currentPlayerColor = playerColor;
        int previousObjectIndex = -1;
        if (selectedObjectID > -1)
        {
            previousObjectIndex = selectedObjectID;
        }

        selectedObjectID = database.playerPieces.FindIndex(data => data.ID == pieceID);
        if (selectedObjectID > -1)
        {
            if (isPiecePlaced)
            {
                gridManager.RemoveTempPlayerPiece(previousObjectIndex);
            }

            selectedObjectRotation = 0;
            isSelectedObjectMirrored = false;
            isPiecePlaced = false;

            previewManager.StartShowingPlacementPreview(database.playerPieces[selectedObjectID].ID, currentPlayerColor);
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

    /// <summary>
    /// State of the game where the gridManager is called to know if writing is legal.
    /// </summary>
    public void PlaceStructure()
    {
        if (inputManager.IsPointerOverUI())
            return;
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();

        bool placementValidity = CheckPlacementValidity(mousePosition, selectedObjectID, selectedObjectRotation, isSelectedObjectMirrored, isFirstPlacedPiece);
        if (!placementValidity)
        {
            //soundManager.PlaySound(SoundType.WrongPlacement);
            return;
        }

        if (isPiecePlaced)
        {
            gridManager.RemoveTempPlayerPiece(selectedObjectID);
        }

        previewManager.ModifyCursorOpacity();

        gridManager.AddTempPlayerPiece(selectedObjectID, playerID, currentPlayerColor);
        if (!isPiecePlaced)
            isPiecePlaced = true;
        if (isFirstPlacedPiece)
            isFirstPlacedPiece = false;
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
        if (selectedObjectID > -1 && database.playerPieces[selectedObjectID].rotable)
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
        if (selectedObjectID > -1 && database.playerPieces[selectedObjectID].mirrorable)
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
        isPiecePlaced = false;
        selectedObjectID = -1;
        cellIndicatorParent.SetActive(false);
        previewManager.StopShowingPreview();
        inputManager.OnLeftClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        inputManager.OnRightClicked -= RotatePlayerPiece;
        inputManager.OnMiddleClicked -= MirrorPlayerPiece;
    }

    /// <summary>
    /// -IN- GameManager from NextPlayerTurn()
    /// </summary>
    public int PlacedPlayerPieceID()
    {
        if (isPiecePlaced)
            return selectedObjectID;

        return -1;
    }
}
