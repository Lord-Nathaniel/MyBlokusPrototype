using UnityEngine;

public class PlayerPieceManager : MonoBehaviour
{
    [SerializeField] private GameObject mouseIndicator, cellIndicator;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;
    [SerializeField] private PlayerPieceDataSO database;
    [SerializeField] private GameObject gridVisualization;
    [SerializeField] private GridManager gridManager;
    //[SerializeField] private PreviewSystem previewSystem;
    //[SerializeField] private SquarePlacer squarePlacer;
    //[SerializeField] private SoundManager soundManager;

    //private GridManager floorData, furnitureData;
    //private Vector3Int lastDetectedPosition = Vector3Int.zero;
    private int selectedObjectIndex = -1;
    //private bool isBuidling = false;

    private void Start()
    {
        StopPlacement();
        //floorData = new();
        //furnitureData = new();
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        selectedObjectIndex = database.playerPieces.FindIndex(data => data.ID == ID);
        if (selectedObjectIndex > -1)
        {
            //    previewSystem.StartShowingPlacementPreview(database.objectsData[selectedObjectIndex].Prefab,
            //                                               database.objectsData[selectedObjectIndex].Size);
        }
        else
        {
            Debug.LogError($"No object with ID {ID}");
            return;
        }
        gridVisualization.SetActive(true);
        cellIndicator.SetActive(true);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
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

        //bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        //if (!placementValidity)
        //{
        //    soundManager.PlaySound(SoundType.WrongPlacement);
        //    return;
        //}


        GameObject newObject = Instantiate(database.playerPieces[selectedObjectIndex].prefab);
        newObject.transform.position = gridManager.CellToWorld(gridPosition);

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

    public void StopPlacement()
    {
        //if (!isBuidling)
        //{
        //    return;
        //}
        selectedObjectIndex = -1;
        gridVisualization.SetActive(false);
        cellIndicator.SetActive(false);
        //previewSystem.StopShowingPreview();
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        //lastDetectedPosition = Vector3Int.zero;
        //isBuidling = false;
    }

    private void Update()
    {
        //if (!isBuidling)
        //{
        //    return;
        //}
        if (selectedObjectIndex < 0)
            return;
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        mouseIndicator.transform.position = mousePosition;
        cellIndicator.transform.position = grid.CellToWorld(gridPosition);

        //if (lastDetectedPosition != gridPosition)
        //{
        //    bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        //    previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
        //    lastDetectedPosition = gridPosition;
        //}
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        //GridManager selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? floorData : furnitureData;
        //return selectedData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
        return false;
    }
}
