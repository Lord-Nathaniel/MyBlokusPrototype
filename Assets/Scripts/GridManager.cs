using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class manages the grid data and display.
/// It contains a dictionnary of all Cells with its state.
/// -IN- PlayerPieceManager | UIManager
/// </summary>
public class GridManager : MonoBehaviour
{
    [Header("Cell Size")]
    [SerializeField] private float cellLenght;
    [SerializeField] private float cellHeight;

    [Header("Grid Management")]
    [SerializeField] private GameObject gridParent;
    [SerializeField] private GameObject gridVisualsSmall;
    [SerializeField] private GameObject gridVisualsLarge;
    [SerializeField] private PlayerPieceDataSO database;

    [Header("Cell rendering")]
    [SerializeField] private List<Texture2D> placedPieceTextures;
    [SerializeField] private Material playerColorSwap;

    private int gridLenght;
    private int gridHeight;

    private PlayerPieceSO playerPieceSO;
    private List<Vector3Int> tempSquarePositions;
    private List<Vector3Int> squarePositions;
    private Vector3 gridOrigin;

    Dictionary<Vector3Int, CellData> placedSquares = new();

    private List<GameObject> startCells = new();
    private List<GameObject> tempPlacedSquares = new();
    private Color currentPlayerColor;
    private bool isFirstPlacedPiece = true;

    private void Awake()
    {
        ServiceManager.Register(this);
    }

    private void OnDestroy()
    {
        ServiceManager.Unregister<GameManager>();
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
    /// <param name="pieceID"></param>
    /// <param name="rotationNb"></param>
    /// <param name="isMirrored"></param>
    /// <param name="playerID"></param>
    /// <param name="shouldPlacePiece"></param>
    /// <returns></returns>
    public bool CanPlaceObjectAt(Vector3 mousePosition, int pieceID, int rotationNb, bool isMirrored, int playerID, bool shouldPlacePiece)
    {
        Vector3Int gridPosition = WorldToCell(mousePosition);
        playerPieceSO = database.playerPieces[pieceID];
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

        tempSquarePositions = CalculateGridPositions(gridPosition, selectedSquares);
        List<Vector3Int> cornerPositions = CalculateGridPositions(gridPosition, selectedCorners);


        // RULE 1 : No out-of-bound piece
        if (IsAnySquareOutOfBound(tempSquarePositions))
        {
            //Debug.Log("Placement Rule 1 broken : no out-of-boud piece !");
            return false;
        }

        // RULE 2 : First piece must be placed on starting cell
        if (isFirstPlacedPiece)
        {
            if (IsFirstPieceNotOnStartCell(tempSquarePositions))
            {
                //Debug.Log("Placement Rule 2 broken : first piece must be placed on starting cell !");
                return false;
            }
        }
        else
        {
            // RULE 3 : No square must cover an already occupied cell
            if (IsAnySquareOnAlreadyPlacedCell(tempSquarePositions, -10))
            {
                //Debug.Log("Placement Rule 3 broken : no square must cover an already occupied cell !");
                return false;
            }

            //RULE 4 : At least one corner must cover an already owned cell
            if (!isFirstPlacedPiece && IsNoCornerOnAlreadyOwnedPlacedCell(cornerPositions, playerID))
            {
                //Debug.Log("Placement Rule 4 broken : at least one corner must cover an already owned cell !");
                return false;
            }

            // RULE 5 : No square must cover nor touch an already owned cell
            if (IsAnySquareTouchingAOwnedPlacedCell(tempSquarePositions, playerID))
            {
                //Debug.Log("Placement Rule 5 broken : no square must cover nor touch an already owned cell !");
                return false;
            }
        }

        if (shouldPlacePiece)
        {
            //Debug.Log("All rules respected, piece placed !");
            squarePositions = new List<Vector3Int>(tempSquarePositions);
        }
        return true;
    }

    private bool IsAnySquareTouchingAOwnedPlacedCell(List<Vector3Int> squares, int playerID)
    {
        foreach (Vector3Int square in squares)
        {
            List<Vector3Int> touchingSquares = new();

            touchingSquares.Add(new Vector3Int(square.x - 1, 0, square.z));
            touchingSquares.Add(new Vector3Int(square.x + 1, 0, square.z));
            touchingSquares.Add(new Vector3Int(square.x, 0, square.z - 1));
            touchingSquares.Add(new Vector3Int(square.x, 0, square.z + 1));

            foreach (Vector3Int touchingSquare in touchingSquares)
            {
                if (placedSquares.TryGetValue(touchingSquare, out CellData cell) && cell.PlayerID == playerID)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool IsNoCornerOnAlreadyOwnedPlacedCell(List<Vector3Int> corners, int playerID)
    {
        foreach (Vector3Int corner in corners)
        {
            if (placedSquares.TryGetValue(corner, out CellData cell) && cell.PlayerID == playerID)
            {
                return false;
            }
        }
        return true;
    }

    private bool IsAnySquareOnAlreadyPlacedCell(List<Vector3Int> squares, int playerID)
    {
        foreach (Vector3Int square in squares)
        {
            if (placedSquares.TryGetValue(square, out CellData cell) && cell.PlayerID != playerID)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsFirstPieceNotOnStartCell(List<Vector3Int> squares)
    {
        foreach (Vector3Int square in squares)
        {
            if (placedSquares.TryGetValue(square, out CellData cell) && cell.PlayerID == -10)
            {
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
    public void InitGridVisuals(int playerNb)
    {
        List<Vector3Int> startPositions = new();
        if (playerNb == 2)
        {
            placedSquares.Add(new Vector3Int(4, 0, 4), new CellData(-10, -1));
            placedSquares.Add(new Vector3Int(9, 0, 9), new CellData(-10, -1));
            gridLenght = 14;
            gridHeight = 14;
            gridVisualsSmall.SetActive(true);
            gridVisualsLarge.SetActive(false);
        }
        else
        {
            placedSquares.Add(new Vector3Int(0, 0, 0), new CellData(-10, -1));
            placedSquares.Add(new Vector3Int(0, 0, 19), new CellData(-10, -1));
            placedSquares.Add(new Vector3Int(19, 0, 0), new CellData(-10, -1));
            placedSquares.Add(new Vector3Int(19, 0, 19), new CellData(-10, -1));
            gridLenght = 20;
            gridHeight = 20;
            gridVisualsSmall.SetActive(false);
            gridVisualsLarge.SetActive(true);
        }
        gridOrigin = new Vector3(
            -(gridLenght * cellLenght) / 2f,
            0,
            -(gridHeight * cellHeight) / 2f
        );

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
            startCells.Add(newObject);
        }
    }

    /// <summary>
    /// If check is ok, place the player piece squares stored on the grid.
    /// -IN- PlayerPieceManager from PlaceStructure()
    /// </summary>
    /// <param name="pieceID"></param>
    /// <param name="playerID"></param>
    /// <param name="playerColor"></param>
    public void AddTempPlayerPiece(int pieceID, int playerID, Color playerColor)
    {
        if (playerPieceSO != database.playerPieces[pieceID])
            return;
        currentPlayerColor = playerColor;
        foreach (Vector3Int square in tempSquarePositions)
        {
            //Debug.Log("placed square : " + square);
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
    /// -IN- PlayerPieceManager from StartPlacement() and PlaceStructure(), UIManager from OnClickPassAction() and OnClickPieceAction(
    /// </summary>
    public void RemoveTempPlayerPiece()
    {
        if (playerPieceSO == null)
            return;

        foreach (GameObject placedPiece in tempPlacedSquares)
        {
            Destroy(placedPiece);
        }
    }

    /// <summary>
    /// Remove the temporary placed piece, place the piece and add it to the grid dictionary
    /// -IN- PlayerPieceManager from IsPlayerPiecePlaced() 
    /// <param name="playerID"></param>
    /// <param name="textureID"></param>
    /// </summary>
    public void SaveCurrentPiece(int playerID, int textureID)
    {
        int playerPieceID = playerPieceSO.ID;
        RemoveTempPlayerPiece();

        foreach (Vector3Int square in squarePositions)
        {
            if (placedSquares.TryGetValue(square, out CellData cell) && cell.PlayerID == -10)
                placedSquares.Remove(square);

            //Debug.Log("placed square : " + square);
            placedSquares.Add(square, new CellData(playerID, playerPieceID));
            Vector3 worldSquare = CellToWorld(square);
            GameObject newObject = Instantiate(database.squarePreviewPrefab);

            newObject.transform.position = new Vector3(worldSquare.x, 0.02f, worldSquare.z);
            Renderer squareRenderer = newObject.GetComponentInChildren<Renderer>();

            Material mat = new Material(playerColorSwap);
            mat.SetColor("_PlayerColor", currentPlayerColor);
            mat.SetTexture("_MainTex", placedPieceTextures[textureID]);

            squareRenderer.material = mat;
        }
    }

    /// <summary>
    /// Remove all starting cells when every players has ended their first turn.
    /// -IN- PlayerPieceManager from ControlFirstTurn()
    /// <param name="playerNb"></param>
    /// </summary>
    public void PurgeStartingCells(int playerNb)
    {
        isFirstPlacedPiece = false;
        if (playerNb == 2)
        {
            Vector3Int startCell1 = new Vector3Int(4, 0, 4);
            if (placedSquares.TryGetValue(startCell1, out CellData cell1) && cell1.PlayerID == -10)
                placedSquares.Remove(startCell1);
            Vector3Int startCell2 = new Vector3Int(9, 0, 9);
            if (placedSquares.TryGetValue(startCell2, out CellData cell2) && cell2.PlayerID == -10)
                placedSquares.Remove(startCell2);
        }
        else
        {
            Vector3Int startCell1 = new Vector3Int(0, 0, 0);
            if (placedSquares.TryGetValue(startCell1, out CellData cell1) && cell1.PlayerID == -10)
                placedSquares.Remove(startCell1);
            Vector3Int startCell2 = new Vector3Int(0, 0, 19);
            if (placedSquares.TryGetValue(startCell2, out CellData cell2) && cell2.PlayerID == -10)
                placedSquares.Remove(startCell2);
            Vector3Int startCell3 = new Vector3Int(19, 0, 0);
            if (placedSquares.TryGetValue(startCell3, out CellData cell3) && cell3.PlayerID == -10)
                placedSquares.Remove(startCell3);
            Vector3Int startCell4 = new Vector3Int(19, 0, 19);
            if (placedSquares.TryGetValue(startCell4, out CellData cell4) && cell4.PlayerID == -10)
                placedSquares.Remove(startCell4);
        }
        foreach (GameObject startCell in startCells)
        {
            Destroy(startCell);
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