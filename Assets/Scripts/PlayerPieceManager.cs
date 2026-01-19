using UnityEngine;

public class PlayerPieceManager : MonoBehaviour
{
    [SerializeField] private GameObject cellIndicatorParent;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;
    [SerializeField] private PlayerPieceDataSO database;
    [SerializeField] private GameObject gridVisualization;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private PreviewSystem previewSystem;
    //[SerializeField] private SquarePlacer squarePlacer;
    //[SerializeField] private SoundManager soundManager;

    //private GridManager floorData, furnitureData;
    //private Vector3Int lastDetectedPosition = Vector3Int.zero;
    private int selectedObjectIndex = -1;
    private int selectedObjectRotation = 0;
    private bool isSelectedObjectMirrored = false;
    private bool isFirstPlacedPiece = true;
    private int playerNb = 2;
    //private bool isBuidling = false;

    private void Start()
    {
        StopPlacement();
        gridManager.PlaceStartCell(playerNb);

        //floorData = new();
        //furnitureData = new();d
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        selectedObjectIndex = database.playerPieces.FindIndex(data => data.ID == ID);
        if (selectedObjectIndex > -1)
        {
            previewSystem.StartShowingPlacementPreview(database.playerPieces[selectedObjectIndex].ID);
            selectedObjectRotation = 0;
            isSelectedObjectMirrored = false;
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
        //previewSystem.StartShowingRemovePreview();
        //inputManager.OnClicked += PlaceStructure;
        //inputManager.OnExit += StopPlacement;
    }

    public void PlaceStructure()
    {
        if (inputManager.IsPointerOverUI())
            return;
        //isBuidling = true;
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = gridManager.WorldToCell(mousePosition);

        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex, selectedObjectRotation, isSelectedObjectMirrored);
        if (!placementValidity)
        {
            //soundManager.PlaySound(SoundType.WrongPlacement);
            return;
        }

        GameObject newObject = Instantiate(database.squarePreviewPrefab);
        newObject.transform.position = gridManager.CellToWorld(gridPosition);
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

        //previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex, int selectedObjectRotation, bool isSelectedObjectMirrored)
    {
        return gridManager.CanPlaceObjectAt(gridPosition,
                                            database.playerPieces[selectedObjectIndex].ID,
                                            selectedObjectRotation,
                                            isSelectedObjectMirrored,
                                            isFirstPlacedPiece);
    }

    public void RotatePlayerPiece()
    {
        if (selectedObjectIndex > -1 && database.playerPieces[selectedObjectIndex].rotable)
        {
            selectedObjectRotation = (selectedObjectRotation + 1) % 4; ;
            previewSystem.RotatePlacementPreview();
        }
    }

    public void MirrorPlayerPiece()
    {
        if (selectedObjectIndex > -1 && database.playerPieces[selectedObjectIndex].mirrorable)
        {
            isSelectedObjectMirrored = !isSelectedObjectMirrored;
            previewSystem.MirrorPlacementPreview();
        }
    }

    public void StopPlacement()
    {
        //if (!isBuidling)
        //{
        //    return;
        //}
        selectedObjectIndex = -1;
        gridVisualization.SetActive(false);
        cellIndicatorParent.SetActive(false);
        //previewSystem.StopShowingPreview();
        inputManager.OnLeftClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        inputManager.OnRightClicked -= RotatePlayerPiece;
        inputManager.OnMiddleClicked -= MirrorPlayerPiece;
    }
    //lastDetectedPosition = Vector3Int.zero;
    //isBuidling = false;
}
