using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Tilemaps;
using System;

public class Pathfinding : MonoBehaviour
{
    [SerializeField] private bool showDebugCosts = false;

    Vector3[] waypoints = new Vector3[0];
    bool pathSuccess = false;

    private Vector2 colorNode;
    PathRequestManager RequestManager;

    [SerializeField] private GridManager gridManager;
    [SerializeField] private Tilemap tilemap;

    private void Awake()
    {
        RequestManager = GetComponent<PathRequestManager>();
    }

    public void StartFindPath(Vector3 startPos, Vector3 endPos)
    {

        if (gridManager != null && gridManager.tileDataMap != null)
        {
            Vector3Int startCell = tilemap.WorldToCell(startPos);
            Vector3Int endCell = tilemap.WorldToCell(endPos);
        }
        else
        {
            Debug.LogError("GridManager or tileDataMap is null in StartFindPath!");
        }
        StartCoroutine(FindPath(startPos, endPos));
    }


    // Add a struct to hold neighbor cost and position for debugging
    private struct NeighborDebugInfo
    {
        public Vector3 position;
        public int Fcost;
        public int Gcost;
        public int Hcost;
        public NeighborDebugInfo(Vector3 pos, int Fc, int Gc, int Hc)
        {
            position = pos;
            Fcost = Fc;
            Gcost = Gc;
            Hcost = Hc;
        }
    }

    // List to store neighbor debug info for the current pathfinding step
    private List<NeighborDebugInfo> neighborDebugInfos = new List<NeighborDebugInfo>();

    IEnumerator FindPath(Vector3 startWorldPos, Vector3 targetWorldPos)
    {
        // Reset all TileData costs and parent before pathfinding
        foreach (var tileData in gridManager.tileDataMap.Values)
        {
            tileData.gCost = 0;
            tileData.hCost = 0;
            tileData.parent = null;
        }
        neighborDebugInfos.Clear();

        TileData startNode = gridManager.GetTileData(tilemap.WorldToCell(startWorldPos));
        TileData endNode = gridManager.GetTileData(tilemap.WorldToCell(targetWorldPos));

        if (endNode.walkable)
        {
            List<TileData> OpenSet = new List<TileData>();
            HashSet<TileData> ClosedSet = new HashSet<TileData>();
            OpenSet.Add(startNode);

            while (OpenSet.Count > 0)
            {
                TileData currentNode = OpenSet[0];
                for (int i = 1; i < OpenSet.Count; i++)
                {
                    if (OpenSet[i].fCost < currentNode.fCost || OpenSet[i].fCost == currentNode.fCost && OpenSet[i].hCost < currentNode.hCost)
                    {
                        currentNode = OpenSet[i];
                    }
                }

                OpenSet.Remove(currentNode);
                ClosedSet.Add(currentNode);

                colorNode = currentNode.worldPosition;

                if (currentNode == endNode)
                {
                    pathSuccess = true;
                    break;
                }
                foreach (TileData neighbor in GetNeighbours(currentNode))
                {
                    if (neighbor == null)
                    {
                        Debug.LogWarning("Neighbor is null at position: " + currentNode.cellPosition);
                        continue;
                    }

                    if (!neighbor.walkable || ClosedSet.Contains(neighbor))
                    {
                        continue;
                    }
                    int newCostToNeighbor;
                    newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);

                    if (newCostToNeighbor < neighbor.gCost || !OpenSet.Contains(neighbor))
                    {
                        neighbor.gCost = newCostToNeighbor;
                        neighbor.hCost = GetDistance(neighbor, endNode);
                        neighbor.parent = currentNode;
                        if (!OpenSet.Contains(neighbor))
                            OpenSet.Add(neighbor);
                    }

                    // Add debug info for this neighbor
                    neighborDebugInfos.Add(new NeighborDebugInfo(
                        neighbor.worldPosition,
                        neighbor.fCost,
                        neighbor.gCost,
                        neighbor.hCost
                    ));
                }
            }
        }
        yield return null;
        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, endNode);
        }
        RequestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }

    Vector3[] RetracePath(TileData startNode, TileData endNode)
    {
        List<TileData> path = new List<TileData>();
        TileData currentNode = endNode;
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }


        Vector3[] waypoints = SimplifyPath(path);

        Array.Reverse(waypoints);
        return waypoints;
    }

    Vector3[] SimplifyPath(List<TileData> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        for (int i = 0; i < path.Count; i++)
        {
            // Always add every node's world position (optionally offset as before)
            waypoints.Add(path[i].worldPosition - new Vector3(0, 0, 0));
        }
        return waypoints.ToArray();
    }

    private List<TileData> GetNeighbours(TileData tileData)
    {
        List<TileData> neighbours = new List<TileData>();
        Vector3Int pos = tileData.cellPosition;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;
                if (x == 1 && y == 1 || x == -1 && y == -1 || x == 1 && y == -1 || x == -1 && y == 1)
                {
                    continue;
                }
                Vector3Int neighborPos = new Vector3Int(pos.x + x, pos.y + y, pos.z);
                TileData neighborNode = gridManager.GetTileData(neighborPos);
                if (neighborNode != null)
                {
                    neighbours.Add(neighborNode);
                }
            }
        }
        return neighbours;
    }

    int GetDistance(TileData nodeA, TileData nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    void OnDrawGizmos()
    {
        if (showDebugCosts)
        {
#if UNITY_EDITOR
            // Draw neighbor costs as labels at their world positions
            if (neighborDebugInfos != null)
            {
                foreach (var info in neighborDebugInfos)
                {
                    // Compose the cost string
                    string costText = $"F:{info.Fcost}\nG:{info.Gcost}\nH:{info.Hcost}";
                    // Draw the label at the neighbor's position
                    Handles.Label(info.position + new Vector3(-0.5f, 0, 0), costText, EditorStyles.boldLabel);
                }
            }
#endif
        }
    }
}
