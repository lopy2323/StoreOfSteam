using JetBrains.Annotations;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    private Dictionary<string, ItemData> itemDictionary;
    [SerializeField]private Item[] items;


    void Start()
    {
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
