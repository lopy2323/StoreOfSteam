using Unity.VisualScripting.ReorderableList.Element_Adder_Menu;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TrainTrackPoint))]
public class TrainTrackPointEditor : Editor
{
    [SerializeField] private GameObject bezierPointPrefab;
    private static TrainTrackPoint firstSelected;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TrainTrackPoint point = (TrainTrackPoint)target;

        if (firstSelected == null)
        {
            if (GUILayout.Button("Start Selection"))
            {
                firstSelected = point;
                EditorGUIUtility.PingObject(point.gameObject);
            }
        }
        else if (firstSelected != point)
        {
            if (GUILayout.Button("Connect To Selected"))
            {
                AddConnection(firstSelected, point);
                AddConnection(point, firstSelected);
                firstSelected = null;
            }

            if (GUILayout.Button("Connect To Selected BEZIER"))
            {
                AddConnection(firstSelected, point);
                AddConnection(point, firstSelected);

                AddBezierConnection(firstSelected, point);
                AddBezierConnection(point, firstSelected);
                firstSelected = null;
            }
            if (GUILayout.Button("Disconnect To Selected"))
            {
                RemoveConnection(firstSelected, point);
                RemoveConnection(point, firstSelected);
                firstSelected = null;
            }
        }
        else
        {
            if (GUILayout.Button("Cancel Connection"))
            {
                firstSelected = null;
            }
        }
    }

    private void AddConnection(TrainTrackPoint from, TrainTrackPoint to)
    {
        var so = new SerializedObject(from);
        var connections = so.FindProperty("trackConnections");

        for (int i = 0; i < connections.arraySize; i++)
        {
            var element = connections.GetArrayElementAtIndex(i);
            var connectionPoint = element.FindPropertyRelative("connectionPoint").objectReferenceValue;
            if (connectionPoint == to.gameObject)
            {
                return;
            }
        }

        int newIndex = connections.arraySize;
        connections.InsertArrayElementAtIndex(newIndex);

        var connectionStruct = connections.GetArrayElementAtIndex(newIndex);

        connectionStruct.FindPropertyRelative("connectionPoint").objectReferenceValue = to.gameObject;
        connectionStruct.FindPropertyRelative("bezierPoint").objectReferenceValue = null;

        so.ApplyModifiedProperties();
    }

    private void AddBezierConnection(TrainTrackPoint from, TrainTrackPoint to)
    {
        // Find or create the connection on 'from'
        var soFrom = new SerializedObject(from);
        var fromConnections = soFrom.FindProperty("trackConnections");
        SerializedProperty fromConnection = null;
        for (int i = 0; i < fromConnections.arraySize; i++)
        {
            var element = fromConnections.GetArrayElementAtIndex(i);
            if (element.FindPropertyRelative("connectionPoint").objectReferenceValue == to.gameObject)
            {
                fromConnection = element;
                break;
            }
        }
        if (fromConnection == null)
        {
            fromConnections.InsertArrayElementAtIndex(fromConnections.arraySize);
            fromConnection = fromConnections.GetArrayElementAtIndex(fromConnections.arraySize - 1);
            fromConnection.FindPropertyRelative("connectionPoint").objectReferenceValue = to.gameObject;
        }

        // Find or create the connection on 'to'
        var soTo = new SerializedObject(to);
        var toConnections = soTo.FindProperty("trackConnections");
        SerializedProperty toConnection = null;
        for (int i = 0; i < toConnections.arraySize; i++)
        {
            var element = toConnections.GetArrayElementAtIndex(i);
            if (element.FindPropertyRelative("connectionPoint").objectReferenceValue == from.gameObject)
            {
                toConnection = element;
                break;
            }
        }
        if (toConnection == null)
        {
            toConnections.InsertArrayElementAtIndex(toConnections.arraySize);
            toConnection = toConnections.GetArrayElementAtIndex(toConnections.arraySize - 1);
            toConnection.FindPropertyRelative("connectionPoint").objectReferenceValue = from.gameObject;
        }

        var fromBezier = fromConnection.FindPropertyRelative("bezierPoint").objectReferenceValue as GameObject;
        var toBezier = toConnection.FindPropertyRelative("bezierPoint").objectReferenceValue as GameObject;

        GameObject bezierPoint = fromBezier ?? toBezier;
        if (bezierPoint == null)
        {
            if (bezierPointPrefab == null)
            {
                Debug.LogError("BezierPointPrefab is not assigned!");
                return;
            }
            bezierPoint = (GameObject)PrefabUtility.InstantiatePrefab(bezierPointPrefab);
            bezierPoint.transform.position = (from.transform.position + to.transform.position) / 2f;
        }

        fromConnection.FindPropertyRelative("bezierPoint").objectReferenceValue = bezierPoint;
        toConnection.FindPropertyRelative("bezierPoint").objectReferenceValue = bezierPoint;

        soFrom.ApplyModifiedProperties();
        soTo.ApplyModifiedProperties();
    }

    private void RemoveConnection(TrainTrackPoint from, TrainTrackPoint to)
    {
        var so = new SerializedObject(from);
        var connections = so.FindProperty("trackConnections");

        for (int i = 0; i < connections.arraySize; i++)
        {
            var element = connections.GetArrayElementAtIndex(i);
            if (element.FindPropertyRelative("connectionPoint").objectReferenceValue == to.gameObject)
            {
                DestroyImmediate(element.FindPropertyRelative("bezierPoint").objectReferenceValue as GameObject, true);
                connections.DeleteArrayElementAtIndex(i);
                break;
            }
        }

        so.ApplyModifiedProperties();
    }
}
