using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using Unity.VisualScripting;


public class GridManager : MonoBehaviour
{
    static public GridManager instance;
    public Tilemap tilemap;
    public Dictionary<Vector3Int, TileData> tileDataMap = new Dictionary<Vector3Int, TileData>();

    void Awake()
    {
        instance = this;
        ScanTilemap();
    }

    void ScanTilemap()
    {
        BoundsInt bounds = tilemap.cellBounds;

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (!tilemap.HasTile(pos)) continue;

            TileData data = new TileData(
                true,
                tilemap.CellToWorld(pos) + tilemap.cellSize / 2f,
                pos.x,
                pos.y,
                pos,
                false
            );
            tileDataMap[pos] = data;
        }
    }

    public TileData GetTileData(Vector3Int pos)
    {
        tileDataMap.TryGetValue(pos, out var data);
        return data;
    }
}

