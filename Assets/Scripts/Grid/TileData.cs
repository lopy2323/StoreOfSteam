using UnityEngine;
using System.Collections;
using NUnit.Framework;

[System.Serializable]
public class TileData
{
    public bool walkable;
    public Vector3 worldPosition;
    public Vector3Int cellPosition;
    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;
    public TileData parent;
    public bool occupied;
    public TileData(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, Vector3Int _cellPos, bool _occupied)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
        cellPosition = _cellPos;
        occupied = _occupied;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
}