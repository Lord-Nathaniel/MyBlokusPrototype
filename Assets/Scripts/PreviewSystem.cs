using UnityEngine;

public class PreviewSystem : MonoBehaviour
{
    [SerializeField] private float previewYOffset = 0.006f;
    [SerializeField] private GameObject cellIndicatorParent;
    [SerializeField] private PlayerPieceDataSO database;

    private GameObject previewObject;
    private PlayerPieceSO playerPieceSO;

    private void Start()
    {
        cellIndicatorParent.SetActive(false);
    }

    public void StartShowingPlacementPreview(int ID)
    {
        this.playerPieceSO = database.playerPieces[ID];
        //previewObject = Instantiate(playerPieceSO.prefab);
        PreparePreview();
        //PrepareCursor(size);
        cellIndicatorParent.SetActive(true);
    }

    private void PrepareCursor(Vector2Int size)
    {
        if (size.x > 0 || size.y > 0)
        {
            cellIndicatorParent.transform.localScale = new Vector3(size.x, 1, size.y);
        }
    }

    private void PreparePreview()
    {
        //foreach (Vector2Int square in playerPieceSO.squares)
        //{

        //    //Transform cellIndicatorPreviewTransform = cellIndicatorParent./*GetComponent*/<CursorIndicator>().transform;
        //    _ = Instantiate(cellIndicatorParent, cellIndicatorParent.transform, false);
        //}
    }
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
        if (previewObject != null)
        {
            MovePreview(position);
            //ApplyFeedbackToPreview(validity);
        }

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
}
