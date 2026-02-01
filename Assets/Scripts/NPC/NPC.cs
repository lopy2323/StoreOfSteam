using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NPC : MonoBehaviour
{

    private float scrore = 100f;
    private List<string> shoppinglist = new List<string>();
    private Shelf _shelf;
    public TypeOfCustomer typeOfCustomer = TypeOfCustomer.Workerclass;

    [SerializeField] private float speed = 1.25f;
    [SerializeField] private GameObject targetPosition;
    public bool showPathLines = false;
    private bool processingPath = false;
    Vector3[] path;
    int targetIndex;
    public int personID;
    public enum TypeOfCustomer
    {
        Workerclass,
        Richclass,
        businessclass
    }
    private void Start()
    {
        List<Shelf> shelves = ItemManager.instance.GetShelfs("lol");
        if (shelves.Count == 0)
        {
            Debug.Log("No shelf found with item: " + "lol");
            return;
        }
        if (shelves.Count > 0)
        {
            float distance = Vector3.Distance(transform.position, shelves[0].transform.position);
            for (int i = 1; i < shelves.Count; i++)
            {
                float newdistance = Vector3.Distance(transform.position, shelves[i].transform.position);
                if (newdistance < distance)
                {
                    distance = newdistance;
                    _shelf = shelves[i];
                }
            }
        }
        movetoShelf();
    }
    private void movetoShelf()
    {
        Vector3 target = _shelf._targetStandingPoint[0].position;

        PathRequestManager.RequestPath(transform.position, target,OnPathFound);
    }
    
    private void getItem()
        {
        _shelf.TakeItem("lol",1);
        shoppinglist.RemoveAt(0);
        Debug.Log("Item taken from shelf, remaining items: " + shoppinglist.Count);
    }
    private void ItemSelectList(TypeOfCustomer type)
    {
        shoppinglist.Add("lol");
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
                    getItem();
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
}