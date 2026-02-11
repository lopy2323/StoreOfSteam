using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
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

    private float nextActionTime = 0.2f;
    private float timer = 0f;
    private float scrore = 100f;
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
        shoppinglist.Add(new NpcData("lol",5));
        shoppinglist.Add(new NpcData("lol", 5));
    }

    //fase 1

    private void Npcfase1()
    {
        if (shoppinglist.Count != 0)
        {
            List<Shelf> shelves = ItemManager.instance.GetShelfs(shoppinglist[0].name);
            if (shelves.Count == 0)
            {
                Debug.Log("No shelf found with item: " + shoppinglist[0]);
                return;
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

                if (target != transform.position)
                {
                    PathRequestManager.RequestPath(transform.position, target, OnPathFound);
                } else if (target == transform.position && shoppinglist[0] != null)
                {
                    Npcfase3();
                }
            }
        }
        if (shoppinglist.Count == 0)
        {
            Debug.Log("Shopping list is empty for NPC");
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
            yield break;
        }

        Vector3 currentWaypoint = path[targetIndex];
        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length && !processingPath)
                {
                    Npcfase3();
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

    // fase 3

    void Update()
    {
        if (Time.time >= nextActionTime)
        {
            nextActionTime += 1f;
            Npcfase1();
        }
    }

    private void Npcfase3()
    {
        if (timer >= nextActionTime)
        {
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
}