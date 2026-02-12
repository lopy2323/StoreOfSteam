using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;


public class GridManager : MonoBehaviour
{
    static public GridManager instance;
    public Tilemap tilemap;
    public Dictionary<Vector3Int, TileData> tileDataMap = new Dictionary<Vector3Int, TileData>();
    public GameObject buildObject;

    static bool savegame = false;

    void Awake()
    {
        instance = this;
        if (!savegame)
        {
            ScanTilemap();
        }


        if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
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

    public void SaveGrid()
    {
        var list = new List<TileSaveData>();

        foreach (var kvp in tileDataMap)
        {
            list.Add(new TileSaveData
            {
                position = kvp.Key,
                occupied = kvp.Value.occupied
            });
        }

        SaveSystem.instance.SaveSection("TestSave", "gridData", list);
    }

    public void LoadGrid()
    {
        var list = SaveSystem.instance.LoadSection<List<TileSaveData>>("TestSave", "gridData");
        foreach (var item in list)
        {
            if (tileDataMap.ContainsKey(item.position))
            {
                tileDataMap[item.position].occupied = item.occupied;
            }
        }
        savegame = true;
    }
}

[System.Serializable]
public struct TileSaveData
{
    public Vector3Int position;
    public bool occupied;
}

