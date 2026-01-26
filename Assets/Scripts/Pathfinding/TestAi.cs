using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class TestAi : MonoBehaviour
{
    [SerializeField] private float speed = 1.25f;
    [SerializeField] private GameObject targetPosition;
    public bool showPathLines = false;
    private bool processingPath = false;
    Vector3[] path;
    int targetIndex;
    public int personID;
    string targetType; // "home" or "job"

    private void Awake()
    {
        // Find the PeopleManagerScript in the scene
        showPathLines = FindFirstObjectByType<DebugManager>().pathLines;
    }
    void Update()
    {
        UpdatePathLine();

        if (Input.GetKeyDown(KeyCode.G))
        {
            MovePerson(1, targetPosition.transform.position, "testTarget");
        }
    }

    //-- requestPath
    public void MovePerson(int id, Vector3 targetPosition, string target)
    {   
        processingPath = true;
        personID = id;
        targetType = target;
        PathRequestManager.RequestPath(transform.position, targetPosition, OnPathFound);
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        processingPath = false;
        if (pathSuccessful && newPath != null && newPath.Length > 0)
        {
            path = newPath;
            targetIndex = 0; // Reset index!
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }
    //-- end request path

    IEnumerator FollowPath()
    {
        if (path == null || path.Length == 0)
            yield break;

        Vector3 currentWaypoint = path[targetIndex];
        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length && !processingPath)
                {
                    Destroy(gameObject); // Destroy the GameObject after reaching the destination
                    yield break; // Reached the end of the path
                }
                // Only update currentWaypoint if targetIndex is valid
                if (targetIndex < path.Length)
                    currentWaypoint = path[targetIndex];
            }
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            yield return null;
        }
    }

    void UpdatePathLine()
    {
        var lr = GetComponent<LineRenderer>();
        if (!showPathLines)
        {
            if (lr != null)
                lr.positionCount = 0; // Clear the line when not showing path lines
            return;
        }
        if (path == null || path.Length == 0)
        {
            lr.positionCount = 0;
            return;
        }
        lr.positionCount = path.Length - targetIndex + 1;
        // Offset Z by -0.1f to draw behind sprites
        lr.SetPosition(0, transform.position + new Vector3(0, 0, -0.1f));
        for (int i = targetIndex; i < path.Length; i++)
        {
            lr.SetPosition(i - targetIndex + 1, path[i] + new Vector3(0, 0, -0.1f));
        }
        lr.widthMultiplier = 0.1f;
    }
}
