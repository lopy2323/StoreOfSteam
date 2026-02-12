using NUnit.Framework;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class StorageManager : MonoBehaviour
{
    static public StorageManager instance;

    private bool isStorageOpen = true;

    public bool managerEnabled = false;

    [SerializeField] private LayerMask Storagelayer;
    [SerializeField] private float repeatInterval = 0.2f;

    public Item item;

    private float _nextFireTime;

    public PackageScript activePackageScript;

    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (managerEnabled)
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
        }
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

    public void SaveShelfs()
    {
            var list = new List<ShelfData>();
            GameObject[] shelfObjects = GameObject.FindGameObjectsWithTag("Shelf");
            foreach (GameObject shelf in shelfObjects)
            {
                List<ShelfSpawn> shelfspawns = new List<ShelfSpawn>();
                shelfspawns.AddRange(shelf.GetComponentsInChildren<ShelfSpawn>());
                shelfspawns.Reverse();
                ShelfData data = new ShelfData
                {
                    position = new float[] { shelf.transform.position.x, shelf.transform.position.y, shelf.transform.position.z },
                    rotation = new float[] { shelf.transform.rotation.eulerAngles.x, shelf.transform.rotation.eulerAngles.y, shelf.transform.rotation.eulerAngles.z },
                    name = shelf.GetComponent<Shelf>().ShelfName,
                    SubShelfs = new List<SubshelfData>()
                };
                foreach (var item in shelfspawns)
                {
                    if (item.ShelfItem != null)
                    {
                        SubshelfData subData = new SubshelfData
                        {
                            itemName = item.ShelfItem.itemName,
                            amount = item.amountOfItems
                        };
                        data.SubShelfs.Add(subData);
                    }
                    else
                    {
                        SubshelfData subData = new SubshelfData
                        {
                            itemName = null,
                            amount = 0
                        };
                        data.SubShelfs.Add(subData);
                    }
                }
                list.Add(data);
            }

            Debug.Log("Saving " + list.Count + " shelves.");
            SaveSystem.instance.SaveSection("TestSave", "shelfData", list);
    }
}

[System.Serializable]
public struct ShelfData
{
    public float[] position;
    public float[] rotation;
    public string name;
    public List<SubshelfData> SubShelfs;
}

public struct SubshelfData
{
    public string itemName;
    public int amount;
}
