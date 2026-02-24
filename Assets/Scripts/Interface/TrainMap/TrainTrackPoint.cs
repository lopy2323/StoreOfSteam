using System;
using UnityEngine;

public class TrainTrackPoint : MonoBehaviour
{

    [System.Serializable]

    public struct ConnectionsData
    {
        public GameObject connectionPoint;
        public GameObject bezierPoint;
    }

    public ConnectionsData[] trackConnections;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, 10f);

        foreach (var connection in trackConnections)
        {
            if (connection.connectionPoint != null)
            {
                if (connection.bezierPoint == null)
                {
                    Vector3 start = transform.position;
                    Vector3 end = connection.connectionPoint.transform.position;

                    Gizmos.DrawLine(start, end);
                }
                else
                {
                    Vector3 start = transform.position;
                    Vector3 control = connection.bezierPoint.transform.position;
                    Vector3 end = connection.connectionPoint.transform.position;
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(start, control);
                    Gizmos.color = Color.cyan;
                    DrawBezierCurve(start, control, end);
                }
            }
        }
    }

    private void DrawBezierCurve(Vector3 start, Vector3 control, Vector3 end)
    {
        int segments = 20;
        Vector3 previousPoint = start;
        for (int i = 1; i <= segments; i++)
        {
            float t = i / (float)segments;
            Vector3 pointOnCurve = CalculateQuadraticBezierPoint(t, start, control, end);
            Gizmos.DrawLine(previousPoint, pointOnCurve);
            previousPoint = pointOnCurve;
        }
    }

    private Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        return uu * p0 + 2 * u * t * p1 + tt * p2;
    }
}