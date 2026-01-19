using System.Collections.Generic;
using UnityEngine;

public class PreviewSystem : MonoBehaviour
{
    [SerializeField] private float previewYOffset = 0.006f;
    [SerializeField] private GameObject cellIndicatorParent;
    [SerializeField] private PlayerPieceDataSO database;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;

    private GameObject previewObject;
    private PlayerPieceSO playerPieceSO;
    [SerializeField] private float cellIndicatorParentYOffset = 0.015f;

    private void Start()
    {
        cellIndicatorParent.SetActive(false);
    }

    public void StartShowingPlacementPreview(int ID)
    {
        cellIndicatorParent.SetActive(true);
        playerPieceSO = database.playerPieces[ID];
        cursorPreview(playerPieceSO.squares, playerPieceSO.corners);
    }

    private void cursorPreview(List<Vector2Int> squares, List<Vector2Int> corners)
    {
        foreach (Transform child in cellIndicatorParent.transform.GetChild(0))
        {
            Destroy(child.gameObject);
        }
        cellIndicatorParent.transform.position = new Vector3(0f, cellIndicatorParentYOffset, 0f);
        cellIndicatorParent.transform.rotation = Quaternion.Euler(0f, 0f, 0f);


        //square preview
        foreach (Vector2Int square in squares)
        {
            GameObject squareGO = Instantiate(database.squarePreviewPrefab, cellIndicatorParent.transform.GetChild(0));
            squareGO.transform.localPosition = new Vector3((float)square.x, 0f, (float)square.y);
            squareGO.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        }

        //corner preview
        foreach (Vector2Int corner in corners)
        {
            GameObject cornerGO = Instantiate(database.cornerPreviewPrefab, cellIndicatorParent.transform.GetChild(0));
            cornerGO.transform.localPosition = new Vector3((float)corner.x, 0f, (float)corner.y);
            cornerGO.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        }
    }

    public void RotatePlacementPreview()
    {
        cellIndicatorParent.transform.GetChild(0).Rotate(0f, 90f, 0f);
    }

    public void MirrorPlacementPreview()
    {
        Vector3 scale = cellIndicatorParent.transform.GetChild(0).localScale;
        scale.x *= -1;
        cellIndicatorParent.transform.GetChild(0).localScale = scale;
    }


    //private void PreparePreview()
    //{
    //foreach (Vector2Int square in playerPieceSO.squares)
    //{

    //    //Transform cellIndicatorPreviewTransform = cellIndicatorParent./*GetComponent*/<CursorIndicator>().transform;
    //    _ = Instantiate(cellIndicatorParent, cellIndicatorParent.transform, false);
    //}
    //}
    //Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
    //foreach (Renderer renderer in renderers)
    //{
    //    Material[] materials = renderer.materials;
    //    for (int i = 0; i < materials.Length; i++)
    //    {
    //        materials[i] = previewMaterialInstance;
    //    }
    //    renderer.materials = materials;
    //}

    public void StopShowingPreview()
    {
        cellIndicatorParent.SetActive(false);
        if (previewObject != null)
            Destroy(previewObject);
    }

    public void UpdatePosition(Vector3 position, bool validity)
    {
        //if (previewObject != null)
        //{
        //    MovePreview(position);
        //    //ApplyFeedbackToPreview(validity);
        //}

        MoveCursor(position);
        //ApplyFeedbackToCursor(validity);
    }

    private void MovePreview(Vector3 position)
    {
        previewObject.transform.position = new Vector3(position.x, position.y + previewYOffset, position.z);
    }

    private void MoveCursor(Vector3 position)
    {
        cellIndicatorParent.transform.position = position;
    }

    //private void ApplyFeedbackToPreview(bool validity)
    //{
    //    Color color = validity ? Color.white : Color.red;
    //    previewMaterialInstance.color = color;
    //    color.a = 0.5f;
    //}

    //private void ApplyFeedbackToCursor(bool validity)
    //{
    //    Color color = validity ? Color.white : Color.red;
    //    cellIndicatorRenderer.material.color = color;
    //    color.a = 0.5f;
    //}

    //internal void StartShowingRemovePreview()
    //{
    //    cellIndicator.SetActive(true);
    //    PrepareCursor(Vector2Int.one);
    //    ApplyFeedbackToCursor(false);
    //}

    private void Update()
    {
        if (playerPieceSO == null)
            return;
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        //Debug.Log(("gridPosition :", gridPosition.x, gridPosition.y, gridPosition.z));


        cellIndicatorParent.transform.position = new Vector3(grid.CellToWorld(gridPosition).x,
                                                             cellIndicatorParentYOffset,
                                                             grid.CellToWorld(gridPosition).z);

        //if (lastDetectedPosition != gridPosition)
        //{
        //    bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        //    previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
        //    lastDetectedPosition = gridPosition;
        //}
    }
}
