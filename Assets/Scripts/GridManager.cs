using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    //public static GridManager instance;
    [Header("Grid Settings")]
    public int gridLenght;
    public int gridHeight;

    [Header("Cell Size")]
    [SerializeField] private float cellLenght;
    [SerializeField] private float cellHeight;

    [Header("Cell Sprites")]
    //TODO after polishing grid shader

    [Header("Grid Management")]
    [SerializeField] private GameObject gridParent;
    [SerializeField] private PlayerPieceDataSO database;

    private PlayerPieceSO playerPieceSO;
    private List<Vector3Int> squarePositions;
    private Vector3 gridOrigin;

    Dictionary<Vector3Int, CellData> placedSquares = new();
    private List<GameObject> currentlyPlacedPiece = new();


    private void Start()
    {
        gridOrigin = new Vector3(
            -(gridLenght * cellLenght) / 2f,
            0,
            -(gridHeight * cellHeight) / 2f
        );
    }
    public void AddObjectAt(Vector3Int gridPosition,
                            Vector2Int objectSize,
                            int ID,
                            int placedObjectIndex)
    {
        //List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        //CellData data = new CellData(positionToOccupy, ID, placedObjectIndex);

        //foreach (var position in positionToOccupy)
        //{
        //    if (placedSquares.ContainsKey(position))
        //        throw new Exception($"Dictionary already contains this cell position {position}");
        //    placedSquares[position] = data;
        //}
    }

    //private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
    //{
    //List<Vector3Int> returnVal = new();
    //for (int x = 0; x < objectSize.x; x++)
    //{
    //    for (int y = 0; y < objectSize.y; y++)
    //    {
    //        returnVal.Add(gridPosition + new Vector3Int(x, 0, y));
    //    }
    //}
    //return returnVal;
    //    return null;
    //}

    private List<Vector3Int> CalculateGridPositions(Vector3Int gridPosition, List<Vector2Int> objectCells)
    {
        List<Vector3Int> returnVal = new();
        foreach (Vector2Int cell in objectCells)
        {
            Debug.Log(("[GridManager] CalculateGridPositions : cell x=", cell.x, " cell y=", cell.y));
            returnVal.Add(gridPosition + new Vector3Int(cell.x, 0, cell.y));
        }
        return returnVal;
    }

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

        Debug.Log("All rules respected, piece placed !");
        return true;
    }

    //public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize)
    //{
    //    List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
    //    foreach (var position in positionToOccupy)
    //    {
    //        if (placedObjects.ContainsKey(position))
    //            return false;
    //    }
    //    return true;
    //}

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

    public int GetRepresentationIndex(Vector3Int gridPosition)
    {
        //if (placedSquares.ContainsKey(gridPosition) == false)
        //    return -1;
        //return placedSquares[gridPosition].PlacedObjectIndex;
        return 0;
    }

    internal void RemoveObjectAt(Vector3Int gridPosition)
    {
        //foreach (var position in placedSquares[gridPosition].occupiedPositions)
        //{
        //    placedSquares.Remove(position);
        //}
    }


    public Vector3Int WorldToCell(Vector3 world)
    {
        Vector3 local = world - gridOrigin;

        int x = Mathf.FloorToInt(local.x / cellLenght);
        int z = Mathf.FloorToInt(local.z / cellHeight);

        return new Vector3Int(x, 0, z);
    }

    public Vector3 CellToWorld(Vector3Int cell)
    {
        float x = (cell.x + 0.5f) * cellLenght;
        float z = (cell.z + 0.5f) * cellHeight;

        Vector3 result = new Vector3(x, 0, z) + gridOrigin;
        return result;
    }

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

    public void PlaceStartingCellOnGrid()
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

    public void AddPlayerPiece(int ID, int playerID)
    {
        if (playerPieceSO != database.playerPieces[ID])
            return;

        foreach (Vector3Int square in squarePositions)
        {
            placedSquares.Add(square, new CellData(playerID, ID));
            Vector3 worldSquare = CellToWorld(square);
            GameObject newObject = Instantiate(database.squarePreviewPrefab);
            currentlyPlacedPiece.Add(newObject);
            newObject.transform.position = new Vector3(worldSquare.x, 0.02f, worldSquare.z);
        }
    }

    public void RemovePlayerPiece(int ID)
    {
        if (playerPieceSO != database.playerPieces[ID])
            return;

        foreach (Vector3Int square in squarePositions)
        {
            placedSquares.Remove(square);
        }

        foreach (GameObject placedPiece in currentlyPlacedPiece)
        {
            Destroy(placedPiece);
        }
    }
}

public class CellData
{
    public int PlayerID { get; private set; }
    public int PlacedObjectIndex { get; private set; }

    public CellData(int playerId, int placedObjectIndex)
    {
        PlayerID = playerId;
        PlacedObjectIndex = placedObjectIndex;
    }
}