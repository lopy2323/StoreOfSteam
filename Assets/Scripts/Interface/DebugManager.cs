using UnityEngine;

public class DebugManager : MonoBehaviour
{
    public bool pathLines = false;

    public void PathLines()
    {
        pathLines = !pathLines;

        //Debug.Log("Path Lines: " + (pathLines ? "ON" : "OFF"));

        var Ais = GameObject.FindObjectsByType<TestAi>(FindObjectsSortMode.None);

        //Debug.Log("Total People Found: " + people.Length);
        foreach (var Ai in Ais)
        {
            Ai.showPathLines = pathLines;
        }
    }
}
