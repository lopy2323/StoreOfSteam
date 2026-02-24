using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class TrainMapManager : MonoBehaviour
{
    [SerializeField] private GameObject trainGameobject;
    [SerializeField] private float speed;

    [SerializeField] private StationInfo _startStation;
    [SerializeField] private StationInfo _endStation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("T key pressed, calculating route...");
            CalculateRoute(_startStation, _endStation);
        }
    }

    private void CalculateRoute(StationInfo start, StationInfo end)
    {
        Debug.Log($"Calculating route from {start.stationName} to {end.stationName}...");

        Queue<TrainTrackPoint> queue = new Queue<TrainTrackPoint>();
        HashSet<TrainTrackPoint> visited = new HashSet<TrainTrackPoint>();
        Dictionary<TrainTrackPoint, TrainTrackPoint> cameFrom = new Dictionary<TrainTrackPoint, TrainTrackPoint>();

        TrainTrackPoint startPoint = start.stationConnections;
        TrainTrackPoint endPoint = end.stationConnections;

        queue.Enqueue(startPoint);
        visited.Add(startPoint);

        bool found = false;

        while (queue.Count > 0)
        {
            TrainTrackPoint current = queue.Dequeue();

            if (current == endPoint)
            {
                found = true;
                break;
            }

            StationInfo station = current.GetComponent<StationInfo>();
            if (station != null && current != startPoint && current != endPoint)
            {
                continue;
            }

            foreach (TrainTrackPoint neighbor in GetNeighbours(current))
            {
                if (neighbor == null || visited.Contains(neighbor))
                    continue;

                queue.Enqueue(neighbor);
                visited.Add(neighbor);
                cameFrom[neighbor] = current;
            }
        }

        if (found)
        {
            List<TrainTrackPoint> path = new List<TrainTrackPoint>();
            TrainTrackPoint current = endPoint;
            while (current != startPoint)
            {
                path.Add(current);
                current = cameFrom[current];
            }
            path.Add(startPoint);
            path.Reverse();

            Debug.Log($"Route found with {path.Count} points:");
            foreach (var point in path)
            {
                Debug.Log($"- {point.gameObject.name}");
            }

            StartCoroutine(MoveTrainAlongPath(BuildVector3Path(path)));
        }
        else
        {
            Debug.LogWarning("No path found.");
        }
    }

    private List<TrainTrackPoint> GetNeighbours(TrainTrackPoint TrackNode)
    {
        List<TrainTrackPoint> neighbours = new List<TrainTrackPoint>();
        foreach (var node in TrackNode.trackConnections)
        {
            if (node.connectionPoint != null)
            {
                var neighbor = node.connectionPoint.GetComponent<TrainTrackPoint>();
                if (neighbor != null)
                {
                    neighbours.Add(neighbor);
                }
            }
        }
        return neighbours;
    }

    private List<Vector3> CalcBezierCurve(Vector3 start, Vector3 control, Vector3 end)
    {
        List<Vector3> curve = new List<Vector3>();
        int segments = 20;
        Vector3 previousPoint = start;
        for (int i = 1; i <= segments; i++)
        {
            float t = i / (float)segments;
            Vector3 pointOnCurve = CalculateQuadraticBezierPoint(t, start, control, end);
            previousPoint = pointOnCurve;
            curve.Add(pointOnCurve);
        }
        return curve;
    }

    private Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        return uu * p0 + 2 * u * t * p1 + tt * p2;
    }
    private List<Vector3> BuildVector3Path(List<TrainTrackPoint> path)
    {
        List<Vector3> result = new List<Vector3>();
        for (int i = 0; i < path.Count - 1; i++)
        {
            for (int j = 0; j < path[i].trackConnections.Length; j++)
            {
                if (path[i].trackConnections[j].connectionPoint == path[i + 1].gameObject)
                {
                    Vector3 start = path[i].transform.position;
                    Vector3 end = path[i + 1].transform.position;
                    if (path[i].trackConnections[j].bezierPoint == null)
                    {
                        result.Add(start);
                        break; // No bezier, move to next segment
                    }
                    Vector3 control = path[i].trackConnections[j].bezierPoint.transform.position;
                    List<Vector3> curve = CalcBezierCurve(start, control, end);
                    result.AddRange(curve);
                    break; // Found the connection, move to next segment
                }
            }
        }
        return result;
    }
    IEnumerator MoveTrainAlongPath(List<Vector3> path)
    {
        if (path == null || path.Count < 2)
            yield break;

        int currentIndex = 0;
        float t = 0f;

        while (currentIndex < path.Count - 1)
        {
            Vector3 start = path[currentIndex];
            Vector3 end = path[currentIndex + 1];
            float segmentLength = Vector3.Distance(start, end);
            t = 0f;

            while (t < 1f)
            {
                t += (Time.deltaTime * speed) / segmentLength;
                trainGameobject.transform.position = Vector3.Lerp(start, end, t);
                yield return null;
            }

            currentIndex++;
        }

        // Snap to final position
        trainGameobject.transform.position = path[path.Count - 1];
    }
}


