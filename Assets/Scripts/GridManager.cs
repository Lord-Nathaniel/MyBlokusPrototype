using System;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager instance;

    [Header("Grid Settings")]
    public int gridLenght { get; private set; }
    public int gridHeight { get; private set; }

    [Header("Cell Size")]
    public float cellLenght { get; private set; }
    public float cellHeight { get; private set; }

    [Header("Cell Sprites")]
    //TODO after polishing grid shader

    [Header("Grid Management")]
    [SerializeField] private GameObject gridParent;

    Dictionary<Vector3Int, CellData> placedSquares = new();

    public void AddObjectAt(Vector3Int gridPosition,
                            Vector2Int objectSize,
                            int ID,
                            int placedObjectIndex)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        CellData data = new CellData(positionToOccupy, ID, placedObjectIndex);

        foreach (var position in positionToOccupy)
        {
            if (placedSquares.ContainsKey(position))
                throw new Exception($"Dictionary already contains this cell position {position}");
            placedSquares[position] = data;
        }
    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> returnVal = new();
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                returnVal.Add(gridPosition + new Vector3Int(x, 0, y));
            }
        }
        return returnVal;
    }

    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        foreach (var position in positionToOccupy)
        {
            if (placedSquares.ContainsKey(position))
                return false;
        }
        return true;
    }

    public int GetRepresentationIndex(Vector3Int gridPosition)
    {
        if (placedSquares.ContainsKey(gridPosition) == false)
            return -1;
        return placedSquares[gridPosition].PlacedObjectIndex;
    }

    internal void RemoveObjectAt(Vector3Int gridPosition)
    {
        foreach (var position in placedSquares[gridPosition].occupiedPositions)
        {
            placedSquares.Remove(position);
        }
    }
}

public class CellData
{
    public List<Vector3Int> occupiedPositions;

    public int ID { get; private set; }
    public int PlacedObjectIndex { get; private set; }

    public CellData(List<Vector3Int> occupiedPositions, int iD, int placedObjectIndex)
    {
        this.occupiedPositions = occupiedPositions;
        ID = iD;
        PlacedObjectIndex = placedObjectIndex;
    }
}