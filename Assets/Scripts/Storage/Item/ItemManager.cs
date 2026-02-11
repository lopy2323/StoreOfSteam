using JetBrains.Annotations;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    static public ItemManager instance;
    public Dictionary<string, ItemData> itemDictionary;
    public Dictionary<string, Item> scriptableItemDictionary;
    [SerializeField]private Item[] items;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
    void Start()
    {
        scriptableItemDictionary = new Dictionary<string, Item>();

        foreach (Item item in items)
        {
            if (!scriptableItemDictionary.ContainsKey(item.itemName))
            {
                scriptableItemDictionary.Add(item.itemName, item);
            }
            else
            {
                Debug.LogWarning($"Duplicate item name found: {item.itemName}. Please ensure all item names are unique.");
            }
        }

        itemDictionary = new Dictionary<string, ItemData>();

        foreach (Item item in items)
        {

            if (!itemDictionary.ContainsKey(item.itemName))
            {
                ItemData newItemData = new ItemData
                {
                    itemName = item.itemName,
                    marketPrice = item.marketPrice,
                    currentPrice = item.marketPrice, 
                    inflation = 0f,
                    shelfs = new List<Shelf>()
                };
                itemDictionary.Add(item.itemName, newItemData);
            }
            else
            {
                Debug.LogWarning($"Duplicate item name found: {item.itemName}. Please ensure all item names are unique.");
            }
        }
    }

    private void Update()
    {
        foreach (var itemData in itemDictionary.Values)
        {
            //Debug.Log(itemData.itemName + " has shelfs: " + itemData.shelfs.Count);
        }
    }


    public void AddShelfPosition(List<Item> items, Shelf shelf)
    {
        foreach (Item item in items)
        {
            if (itemDictionary.ContainsKey(item.name))
            {
                if(itemDictionary[item.name].shelfs.Contains(shelf)) continue;
                itemDictionary[item.name].shelfs.Add(shelf);
            }
            else
            {
                Debug.LogWarning($"Item name {item.name} not found in itemDictionary.");
            }
        }
    }

    public void RemoveShelfPosition(List<Item> items, Shelf shelf)
    {
        foreach (Item item in items)
        {
            if (itemDictionary.ContainsKey(item.name))
            {
                if (!itemDictionary[item.name].shelfs.Contains(shelf)) continue;
                itemDictionary[item.name].shelfs.Remove(shelf);
            }
            else
            {
                Debug.LogWarning($"Item name {item.name} not found in itemDictionary.");
            }
        }
    }

    public List<Shelf> GetShelfs(string itemName)
    {
        if (itemDictionary.ContainsKey(itemName))
        {
            return itemDictionary[itemName].shelfs;
        }
        else
        {
            Debug.LogWarning($"Item name {itemName} not found in itemDictionary.");
            return new List<Shelf>();
        }
    }
}
