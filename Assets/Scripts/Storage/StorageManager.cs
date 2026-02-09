using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class StorageManager : MonoBehaviour
{
    private bool isStorageOpen = true;

    [SerializeField] private LayerMask Storagelayer;
    [SerializeField] private float repeatInterval = 0.2f;

    public Item item;

    private float _nextFireTime;

    public PackageScript activePackageScript;

    void Update()
    {
        if (isStorageOpen == true)
        {
            if (Input.GetKey(KeyCode.Mouse0) && Time.time >= _nextFireTime)
            {
                _nextFireTime = Time.time + repeatInterval;
                HandleStorageClick();
            }
        }
        if (isStorageOpen == true)
        {
            if (Input.GetKey(KeyCode.Mouse1) && Time.time >= _nextFireTime)
            {
                _nextFireTime = Time.time + repeatInterval;
                HandleStorageClickRight();
            }
        }

        SaveUpdate();
    }

    private void HandleStorageClick()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 2f, Storagelayer))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.TryGetComponent<ShelfSpawn>(out var shelfSpawn))
            {
                if (activePackageScript.packageAmount == 0) return;

                shelfSpawn.SpawnItem(item, activePackageScript);
            }
        }
    }

    private void HandleStorageClickRight()
    {
        if (activePackageScript.packageAmount >= activePackageScript.maxPackageAmount) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 2f, Storagelayer))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.TryGetComponent<ShelfSpawn>(out var shelfSpawn))
            {
                shelfSpawn.RemoveItem(item, OnRemoveItem, activePackageScript.transform.position);
            }
        }
    }

    private void OnRemoveItem(bool _gotItemBack)
    {
            activePackageScript.packageAmount++;
            activePackageScript.Updatetext();
    }

    private void SaveUpdate()
    {
        Debug.Log("Press F4 to save shelf data, F5 to load shelf data.");
        if (Input.GetKeyDown(KeyCode.F4))
        {
            var list = new List<ShelfData>();
            GameObject[] shelfObjects = GameObject.FindGameObjectsWithTag("Shelf");
            foreach (GameObject shelf in shelfObjects)
            {
                ShelfSpawn shelfSpawn = shelf.GetComponent<ShelfSpawn>();
                ShelfData data = new ShelfData
                {
                    transform = shelf.transform,
                    name = shelf.name,
                    SubShelfs = new List<SubshelfData>()
                };
                if (shelfSpawn.ShelfItem != null)
                {
                    SubshelfData subData = new SubshelfData
                    {
                        item = shelfSpawn.ShelfItem,
                        amount = shelfSpawn.amountOfItems
                    };
                    data.SubShelfs.Add(subData);
                }
                list.Add(data);
            }

            Debug.Log("Saving " + list.Count + " shelves.");
            SaveSystem.instance.SaveSection("TestSave", "shelfData", list);
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {

        }
    }
}

[System.Serializable]
public struct ShelfData
{
    public Transform transform;
    public string name;
    public List<SubshelfData> SubShelfs;
}

public struct SubshelfData
{
    public Item item;
    public int amount;
}
