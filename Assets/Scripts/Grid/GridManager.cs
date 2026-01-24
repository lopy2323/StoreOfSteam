using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using Unity.VisualScripting;


public class GridManager : MonoBehaviour
{
    static public GridManager instance;
    public Tilemap tilemap;
    public Dictionary<Vector3Int, TileData> tileDataMap = new Dictionary<Vector3Int, TileData>();
    public GameObject buildObject;

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
                tilemap.GetCellCenterWorld(pos),
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
        tileDataMap.TryGetValue(pos, out TileData data);
        return data;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) { 
            foreach (var kvp in tileDataMap)
            {
                if (kvp.Value.occupied == true)
                {
                   Vector3 buildpos = new Vector3(kvp.Value.worldPosition.x, 1, kvp.Value.worldPosition.z);
                    Instantiate(buildObject, buildpos, Quaternion.identity);
                }
            }
        }
    }
}

