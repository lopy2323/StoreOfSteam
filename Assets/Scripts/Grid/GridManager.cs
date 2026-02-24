using NUnit.Framework.Internal;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;


public class GridManager : MonoBehaviour
{
    static public GridManager instance;
    public Grid grid;
    public List<BoxCollider> SizeGrid = new List<BoxCollider>();
    public Dictionary<Vector3Int, TileData> tileDataMap = new Dictionary<Vector3Int, TileData>();
    public GameObject buildObject;

    static bool savegame = false;

    void Awake()
    {
        instance = this;
        if (!savegame)
        {
            CalculateGrid();
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


    
    public void CalculateGrid()
    {
        foreach (var col in SizeGrid)
        {
            var bounds = col.bounds;


            int width = Mathf.FloorToInt(bounds.size.x);
            int height = Mathf.FloorToInt(bounds.size.z);
            float error = 0.02f;
            Vector3 topLeft = new Vector3(bounds.min.x + error, bounds.center.y, bounds.max.z - error);
            Vector3Int gridPos = grid.WorldToCell(topLeft);

            Debug.Log(height);
            for (int z = 0; z < height; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vector3Int pos = (new Vector3Int(gridPos.x + x, gridPos.y - z, gridPos.z));
                    tileDataMap[pos] = new TileData(
                        true,
                        grid.GetCellCenterWorld(pos),
                        pos.x,
                        pos.y,
                        pos,
                        false
                    );
                }
            }
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

