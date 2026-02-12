using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NPC : MonoBehaviour
{
    private class NpcData
    {
        public string name;
        public int amounttoget;
        public NpcData(string name, int amounttoget)
        {
            this.name = name;
            this.amounttoget = amounttoget;
        }
    }
    private bool End = false;
    private float nextActionTime = 1f;
    private float timer = 0f;
    private List<NpcData> shoppinglist = new List<NpcData>();
    private Shelf _shelf;
    public TypeOfCustomer typeOfCustomer = TypeOfCustomer.Workerclass;

    [SerializeField] private float speed = 1.25f;
    [SerializeField] private GameObject targetPosition;
    public bool showPathLines = false;
    private bool processingPath = false;
    Vector3[] path;
    int targetIndex;
    public int personID;
    private bool selectedNpc = false;

    private Vector3? _currentDestination;
    private bool _isFollowingPath;

    [SerializeField] private float waypointTolerance = 0.05f;


    public GameObject endpoint;

    public enum TypeOfCustomer
    {
        Workerclass,
        Richclass,
        businessclass
    }
    private void Start()
    {
        ItemSelectList(typeOfCustomer);
        Npcfase1();
    }
    private void ItemSelectList(TypeOfCustomer type)
    {
        foreach(var keyValuePair in ItemManager.instance.itemDictionary)
        {
            int amount = UnityEngine.Random.Range(1, 5);
            string name = keyValuePair.Key;
            shoppinglist.Add(new NpcData(name, amount));
        }
    }

    //fase 1

    private void Npcfase1()
    {
        if (shoppinglist.Count != 0)
        {
            List<Shelf> shelves = ItemManager.instance.GetShelfs(shoppinglist[0].name);
            if (shelves.Count == 0)
            {
               if (shoppinglist.Count != 1)
                {
                    shoppinglist.Remove(shoppinglist[0]);
                }
               if (shoppinglist.Count == 1)
                {
                    Npcfase5();
                }
            }
            if (selectedNpc == false)
            {
                Shelf nearestshelf = null;
                var nearestDistance = float.MaxValue;
                foreach (Shelf shelfsItem in shelves)
                {
                    float distance = Vector3.Distance(transform.position, shelfsItem.transform.position);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestshelf = shelfsItem;
                    }
                }
                _shelf = nearestshelf;
                selectedNpc = true;

            }
            if (selectedNpc == true)
            {
                Vector3 target = _shelf._targetStandingPoint[0].position;

                if (Vector3.Distance(transform.position, target) <= waypointTolerance)
                {
                    if (shoppinglist[0] != null)
                        Npcfase3();
                    return;
                }

                RequestPathIfNeeded(target);
            }
        }
        if (shoppinglist.Count == 0)
        {
            Npcfase5();
        }
    }

    // fase2

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
    IEnumerator FollowPath()
    {
        if (path == null || path.Length == 0)
        {
            _isFollowingPath = false;
            yield break;
        }

        _isFollowingPath = true;

        Vector3 currentWaypoint = path[targetIndex];
        while (true)
        {
            if (Vector3.Distance(transform.position, currentWaypoint) <= waypointTolerance)
            {
                targetIndex++;
                if (targetIndex >= path.Length && !processingPath)
                {
                    _isFollowingPath = false;
                    if (shoppinglist.Count != 0)
                    {
                        Npcfase3();
                    }
                    if (shoppinglist.Count == 0 && End == true)
                    {
                        Destroy(gameObject);
                    }
                    
                    if (shoppinglist.Count == 0 && End == false)
                    {
                        Npcfase5(); 
                    }
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

    private void RequestPathIfNeeded(Vector3 target)
    {
        if (processingPath)
            return;

        if (_isFollowingPath && _currentDestination.HasValue && Vector3.Distance(_currentDestination.Value, target) <= waypointTolerance)
            return;

        _currentDestination = target;
        processingPath = true;
        PathRequestManager.RequestPath(transform.position, target, OnPathFound);
    }

    // fase 3

    void Update()
    {
        Debug.Log("update" + timer);
        timer += Time.deltaTime;
        if (timer >= nextActionTime)
        {
            Npcfase1();
        }
    }

    private void Npcfase3()
    {
        if (timer >= nextActionTime)
        {
            Debug.Log("log");
            timer = 0f;
            if (shoppinglist[0].amounttoget != 0)
            {
                int removed = _shelf.TakeItem(shoppinglist[0].name, 1);
                if (removed != 0)
                {
                    shoppinglist[0].amounttoget -= 1;
                    timer = 0f;
                    Npcfase3();
                }
                if (removed == 0)
                {
                    shoppinglist.Add(new NpcData(shoppinglist[0].name, shoppinglist[0].amounttoget));
                    Npcfase4();
                }
                if (shoppinglist[0].amounttoget == 0)
                {
                    Npcfase4();
                }
            }
        }
    }

    //fase 4

    private void Npcfase4()
    {
        shoppinglist.RemoveAt(0);
        selectedNpc = false;
        Npcfase1();
    }

    //fase 5
    private void Npcfase5()
    {
        shoppinglist.Clear();
        End = true;
        RequestPathIfNeeded(endpoint.transform.position);
    }
}