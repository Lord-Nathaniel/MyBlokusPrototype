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

    Dictionary<Vector3Int, CellData> placedSquares = new();

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
            returnVal.Add(gridPosition + new Vector3Int(cell.x, 0, cell.y));
        }
        return returnVal;
    }

    public bool CanPlaceObjectAt(Vector3Int gridPosition, int ID, int rotationNb, bool isMirrored)
    {
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

        List<Vector3Int> squarePositions = CalculateGridPositions(gridPosition, selectedSquares);
        List<Vector3Int> cornerPositions = CalculateGridPositions(gridPosition, selectedCorners);


        // RULE 1 : No out-of-bound piece 
        if (IsAnySquareOutOfBound(squarePositions))
            return false;

        // RULE 2 : First piece must be placed on starting cell
        // TODO

        // RULE 3 : No square must cover an already occupied cell
        // TODO
        //if (placedSquares.ContainsKey(position))
        //        return false;

        // RULE 4 : At least one corner must cover an already owned cell
        // TODO

        // RULE 5 : No square must cover nor touch an already owned cell
        // TODO

        return true;
    }

    private bool IsAnySquareOutOfBound(List<Vector3Int> squares)
    {
        foreach (Vector3Int square in squares)
        {
            if (square.x < -(gridLenght / 2) || square.x >= (gridLenght / 2)
                || square.z < -(gridHeight / 2) || square.z >= (gridHeight / 2))
                return true;
        }
        return false;
    }

    private List<Vector2Int> MirrorVector2List(List<Vector2Int> vectors)
    {


        return vectors;
    }

    private List<Vector2Int> RotateVector2List(List<Vector2Int> vectors, int rotationNb)
    {


        return vectors;
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
        Vector3 GridOrigin = new Vector3(-(gridLenght / 2), 0, -(gridHeight / 2));
        Vector3 local = world - GridOrigin;

        int x = Mathf.FloorToInt(local.x / cellLenght);
        int y = Mathf.FloorToInt(local.z / cellHeight);

        return new Vector3Int(x, y, 0);
    }

    public Vector3 CellToWorld(Vector3Int cell)
    {
        return new Vector3(
            -(gridLenght / 2) + (cell.x + 0.5f) * cellLenght,
            0,
            -(gridHeight / 2) + (cell.y + 0.5f) * cellHeight
        );
    }
}

public class CellData
{
    public List<Vector3Int> occupiedPositions;

    public int PlayerID { get; private set; }
    public int PlacedObjectIndex { get; private set; }

    public CellData(List<Vector3Int> occupiedPositions, int playerId, int placedObjectIndex)
    {
        this.occupiedPositions = occupiedPositions;
        PlayerID = playerId;
        PlacedObjectIndex = placedObjectIndex;
    }
}