using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager instance;

    [Header("Grid Settings")]
    [SerializeField] private int gridWidth = 14;
    [SerializeField] private int gridHeight = 14;
    [SerializeField] private float cellSize = 128f;

    [Header("Cell Prefab")]
    [SerializeField] private GameObject cellPrefab;

    [Header("Parent for UI Cells")]
    [SerializeField] private RectTransform gridBoard;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        Debug.Log($"[GridManager] Génération grille {gridWidth}x{gridHeight} (0,0 en bas à gauche)");

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                GameObject cellObj = Instantiate(cellPrefab, gridBoard);
                RectTransform rt = cellObj.GetComponent<RectTransform>();

                Vector2 localPos = new Vector2(
                    x * cellSize,
                    y * cellSize
                );

                rt.anchoredPosition = localPos;
            }
        }
    }
}
