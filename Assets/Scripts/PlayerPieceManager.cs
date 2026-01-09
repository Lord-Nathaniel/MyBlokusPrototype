using UnityEngine;

public class PlayerPieceManager : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;
    //[SerializeField] private ObjectDatabaseSO database;
    [SerializeField] private GameObject gridVisualization;
    [SerializeField] private PreviewSystem previewSystem;
    //[SerializeField] private ObjectPlacer objectPlacer;
    //[SerializeField] private SoundFeedback soundFeedback;

    //private GridData floorData, furnitureData;
    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    //_IBuildingState buildingState;

    private void Start()
    {
        StopPlacement();
        //floorData = new();
        //furnitureData = new();
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        //buildingState = new PlacementState(ID,
        //                                   grid,
        //                                   previewSystem,
        //                                   database,
        //                                   floorData,
        //                                   furnitureData,
        //                                   objectPlacer,
        //                                   soundFeedback);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    public void StartRemoving()
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        //buildingState = new RemovingState(grid,
        //                                  previewSystem,
        //                                  floorData,
        //                                  furnitureData,
        //                                  objectPlacer,
        //                                  soundFeedback);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    public void PlaceStructure()
    {
        if (inputManager.IsPointerOverUI())
            return;
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        //buildingState.OnAction(gridPosition);
    }

    public void StopPlacement()
    {
        //if (buildingState == null)
        //    return;
        gridVisualization.SetActive(false);
        //buildingState.EndState();
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        lastDetectedPosition = Vector3Int.zero;
        //buildingState = null;
    }

    private void Update()
    {
        //if (buildingState == null)
        //    return;
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        if (lastDetectedPosition != gridPosition)
        {
            //buildingState.UpdateState(gridPosition);
            lastDetectedPosition = gridPosition;
        }
    }
}
