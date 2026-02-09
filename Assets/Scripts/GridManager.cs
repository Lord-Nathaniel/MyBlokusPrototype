using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class manages the grid data and display.
/// It contains a dictionnary of all Cells with its state.
/// -IN- PlayerPieceManager
/// </summary>
public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridLenght;
    public int gridHeight;

    [Header("Cell Size")]
    [SerializeField] private float cellLenght;
    [SerializeField] private float cellHeight;

    [Header("Grid Management")]
    [SerializeField] private GameObject gridParent;
    [SerializeField] private PlayerPieceDataSO database;

    [Header("Cell rendering")]
    [SerializeField] private Texture2D placedPieceTexture;
    [SerializeField] private Material playerColorSwap;

    private PlayerPieceSO playerPieceSO;
    private List<Vector3Int> squarePositions;
    private Vector3 gridOrigin;

    Dictionary<Vector3Int, CellData> placedSquares = new();

    private List<GameObject> tempPlacedSquares = new();
    private Color currentPlayerColor;

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
        gridOrigin = new Vector3(
            -(gridLenght * cellLenght) / 2f,
            0,
            -(gridHeight * cellHeight) / 2f
        );
    }

    private List<Vector3Int> CalculateGridPositions(Vector3Int gridPosition, List<Vector2Int> objectCells)
    {
        List<Vector3Int> returnVal = new();
        foreach (Vector2Int cell in objectCells)
        {
            returnVal.Add(gridPosition + new Vector3Int(cell.x, 0, cell.y));
        }
        return returnVal;
    }

    /// <summary>
    /// Check with game rule if squares from a piece are allowed to go on cells.
    /// -IN- PlayerPieceManager from CheckPlacementValidity()
    /// </summary>
    /// <param name="mousePosition"></param>
    /// <param name="ID"></param>
    /// <param name="rotationNb"></param>
    /// <param name="isMirrored"></param>
    /// <param name="isFirstPlacedPiece"></param>
    /// <returns></returns>
    public bool CanPlaceObjectAt(Vector3 mousePosition, int ID, int rotationNb, bool isMirrored, bool isFirstPlacedPiece)
    {
        Vector3Int gridPosition = WorldToCell(mousePosition);
        playerPieceSO = database.playerPieces[ID];
        List<Vector2Int> selectedSquares = playerPieceSO.squares;
        List<Vector2Int> selectedCorners = playerPieceSO.corners;

        if (isMirrored)
        {
            selectedSquares = MirrorVector2List(selectedSquares);
            selectedCorners = MirrorVector2List(selectedCorners);
        }

        if (rotationNb > 0)
        {
            selectedSquares = RotateVector2List(selectedSquares, rotationNb);
            selectedCorners = RotateVector2List(selectedCorners, rotationNb);
        }

        squarePositions = CalculateGridPositions(gridPosition, selectedSquares);
        List<Vector3Int> cornerPositions = CalculateGridPositions(gridPosition, selectedCorners);


        // RULE 1 : No out-of-bound piece
        //if (IsAnySquareOutOfBound(squarePositions))
        //{
        //    Debug.Log("Placement Rule 1 broken : no out-of-boud piece !");
        //    return false;
        //}
        //
        //// RULE 2 : First piece must be placed on starting cell
        //if (isFirstPlacedPiece && IsFirstPieceNotOnStartCell(squarePositions))
        //{
        //    Debug.Log("Placement Rule 2 broken : first piece must be placed on starting cell !");
        //    return false;
        //}
        //
        //// RULE 3 : No square must cover an already occupied cell
        //if (IsAnySquareOnAlreadyPlacedCell(squarePositions))
        //{
        //    Debug.Log("Placement Rule 3 broken : no square must cover an already occupied cell !");
        //    return false;
        //}
        //
        ////RULE 4 : At least one corner must cover an already owned cell
        //if (!isFirstPlacedPiece && IsNoCornerOnAlreadyPlacedCell(cornerPositions))
        //{
        //    Debug.Log("Placement Rule 4 broken : at least one corner must cover an already owned cell !");
        //    return false;
        //}
        //
        //// RULE 5 : No square must cover nor touch an already owned cell
        //if (IsAnySquareTouchingAnyPlacedCell(squarePositions))
        //{
        //    Debug.Log("Placement Rule 5 broken : no square must cover nor touch an already owned cell !");
        //    return false;
        //}

        //Debug.Log("All rules respected, piece placed !");
        return true;
    }

    private bool IsAnySquareTouchingAnyPlacedCell(List<Vector3Int> squares)
    {
        foreach (Vector3Int square in squares)
        {
            List<Vector3Int> touchingSquares = new();

            touchingSquares.Add(new Vector3Int(square.x - 1, 0, square.z));
            touchingSquares.Add(new Vector3Int(square.x + 1, 0, square.z));
            touchingSquares.Add(new Vector3Int(square.x, 0, square.z - 1));
            touchingSquares.Add(new Vector3Int(square.x, 0, square.z + 1));

            if (IsAnySquareOnAlreadyPlacedCell(touchingSquares))
                return true;
        }
        return false;
    }

    private bool IsNoCornerOnAlreadyPlacedCell(List<Vector3Int> corners)
    {
        foreach (Vector3Int corner in corners)
        {
            if (placedSquares.ContainsKey(corner))
            {
                return false;
            }
        }
        return true;
    }

    private bool IsAnySquareOnAlreadyPlacedCell(List<Vector3Int> squares)
    {
        foreach (Vector3Int square in squares)
        {
            if (placedSquares.ContainsKey(square))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsFirstPieceNotOnStartCell(List<Vector3Int> squares)
    {
        Debug.Log("Thrown in IsFirstPieceNotOnStartCell");
        foreach (Vector3Int square in squares)
        {
            if (placedSquares.TryGetValue(square, out CellData cell) && cell.PlayerID == -10)
            {
                placedSquares.Remove(square);
                return false;
            }
        }
        return true;
    }

    private bool IsAnySquareOutOfBound(List<Vector3Int> squares)
    {
        foreach (Vector3Int square in squares)
        {
            if (square.x < 0 || square.x >= gridLenght
                || square.z < 0 || square.z >= gridHeight)
                return true;
        }
        return false;
    }

    private List<Vector2Int> MirrorVector2List(List<Vector2Int> vectors)
    {
        List<Vector2Int> results = new();
        foreach (Vector2Int vector in vectors)
        {
            results.Add(new Vector2Int(-vector.x, vector.y));
        }
        return results;
    }

    private List<Vector2Int> RotateVector2List(List<Vector2Int> vectors, int rotationNb)
    {
        List<Vector2Int> results = new();
        switch (rotationNb)
        {
            case 1:
                foreach (Vector2Int vector in vectors)
                {
                    results.Add(new Vector2Int(vector.y, -vector.x));
                }
                break;
            case 2:
                foreach (Vector2Int vector in vectors)
                {
                    results.Add(new Vector2Int(-vector.x, -vector.y));
                }
                break;
            case 3:
                foreach (Vector2Int vector in vectors)
                {
                    results.Add(new Vector2Int(-vector.y, vector.x));
                }
                break;
        }
        return results;
    }

    /// <summary>
    /// Internal conversion from Vector3 to Vector3Int with the grid offset.
    /// </summary>
    /// <param name="world"></param>
    /// <returns></returns>
    public Vector3Int WorldToCell(Vector3 world)
    {
        Vector3 local = world - gridOrigin;

        int x = Mathf.FloorToInt(local.x / cellLenght);
        int z = Mathf.FloorToInt(local.z / cellHeight);

        return new Vector3Int(x, 0, z);
    }

    /// <summary>
    /// Internal conversion from Vector3Int to Vector3 with the grid offset.
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    public Vector3 CellToWorld(Vector3Int cell)
    {
        float x = (cell.x + 0.5f) * cellLenght;
        float z = (cell.z + 0.5f) * cellHeight;

        Vector3 result = new Vector3(x, 0, z) + gridOrigin;
        return result;
    }

    /// <summary>
    /// Place the correct number of cells depending of the number of players.
    /// -IN- PlayerPieceManager from Start()
    /// </summary>
    /// <param name="playerNb"></param>
    public void PlaceStartCell(int playerNb)
    {
        List<Vector3Int> startPositions = new();
        if (playerNb == 2)
        {
            placedSquares.Add(new Vector3Int(4, 0, 4), new CellData(-10, -1));
            placedSquares.Add(new Vector3Int(9, 0, 9), new CellData(-10, -1));
        }
        else
        {
            placedSquares.Add(new Vector3Int(0, 0, 0), new CellData(-10, -1));
            placedSquares.Add(new Vector3Int(0, 0, 19), new CellData(-10, -1));
            placedSquares.Add(new Vector3Int(19, 0, 0), new CellData(-10, -1));
            placedSquares.Add(new Vector3Int(19, 0, 19), new CellData(-10, -1));
        }
        PlaceStartingCellOnGrid();
    }

    private void PlaceStartingCellOnGrid()
    {
        var enumerator = placedSquares.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var element = enumerator.Current;

            Vector3 worldSquare = CellToWorld(element.Key);
            GameObject newObject = Instantiate(database.cornerPreviewPrefab);
            newObject.transform.position = new Vector3(worldSquare.x, 0.01f, worldSquare.z);
        }
    }

    /// <summary>
    /// If check is ok, place the player piece squares stored on the grid.
    /// -IN- PlayerPieceManager from PlaceStructure()
    /// </summary>
    /// <param name="pieceID"></param>
    /// <param name="playerID"></param>
    public void AddTempPlayerPiece(int pieceID, int playerID, Color playerColor)
    {
        if (playerPieceSO != database.playerPieces[pieceID])
            return;
        currentPlayerColor = playerColor;
        foreach (Vector3Int square in squarePositions)
        {
            //placedSquares.Add(square, new CellData(playerID, pieceID));
            Vector3 worldSquare = CellToWorld(square);
            GameObject newObject = Instantiate(database.squarePreviewPrefab);
            tempPlacedSquares.Add(newObject);
            newObject.transform.position = new Vector3(worldSquare.x, 0.02f, worldSquare.z);
            Renderer squareRenderer = newObject.GetComponentInChildren<Renderer>();
            squareRenderer.material.SetColor("_PlayerColor", currentPlayerColor);
        }
    }

    /// <summary>
    /// Remove the player piece squares stored on the grid.
    /// -IN- PlayerPieceManager from StartPlacement() and PlaceStructure()
    /// </summary>
    /// <param name="ID"></param>
    public void RemoveTempPlayerPiece(int pieceID)
    {
        if (playerPieceSO == null && playerPieceSO != database.playerPieces[pieceID])
            return;

        foreach (GameObject placedPiece in tempPlacedSquares)
        {
            Destroy(placedPiece);
        }
    }

    /// <summary>
    /// -IN- PlayerPieceManager from IsPlayerPiecePlaced() 
    /// </summary>

    public void SaveCurrentPiece(int playerID)
    {
        int playerPieceID = playerPieceSO.ID;
        RemoveTempPlayerPiece(playerPieceID);

        foreach (Vector3Int square in squarePositions)
        {
            placedSquares.Add(square, new CellData(playerID, playerPieceID));
            Vector3 worldSquare = CellToWorld(square);
            GameObject newObject = Instantiate(database.squarePreviewPrefab);

            newObject.transform.position = new Vector3(worldSquare.x, 0.02f, worldSquare.z);
            Renderer squareRenderer = newObject.GetComponentInChildren<Renderer>();

            Material mat = new Material(playerColorSwap);
            mat.SetColor("_PlayerColor", currentPlayerColor);
            mat.SetTexture("_MainTex", placedPieceTexture);

            squareRenderer.material = mat;
        }
    }
}

/// <summary>
/// Store the cell data to be put in the GridManager dictionnary.
/// </summary>
public class CellData
{
    public int PlayerID { get; private set; }
    public int PieceID { get; private set; }

    public CellData(int playerId, int pieceId)
    {
        PlayerID = playerId;
        PieceID = pieceId;
    }
}