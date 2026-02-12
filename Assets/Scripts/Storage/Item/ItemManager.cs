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

    public void SaveItems()
    {
        List<ItemSaveData> list = new List<ItemSaveData>();
        foreach (Item item in items)
        {
            ItemSaveData itemSaveData = new ItemSaveData
            {
                itemName = item.itemName,
                currentPrice = itemDictionary[item.itemName].currentPrice,
                inflation = itemDictionary[item.itemName].inflation
            };
            list.Add(itemSaveData);
        }

        SaveSystem.instance.SaveSection("TestSave", "ItemData", list);
    }

    public void LoadItems()
    {
        foreach (Item item in items)
        {
            var list = SaveSystem.instance.LoadSection<List<ItemSaveData>>("TestSave", "shelfData");
            foreach (ItemSaveData itemSaveData in list)
            {
                if (itemSaveData.itemName == item.itemName)
                {
                    if (itemDictionary.ContainsKey(itemSaveData.itemName))
                    {
                        itemDictionary[itemSaveData.itemName].currentPrice = itemSaveData.currentPrice;
                        itemDictionary[itemSaveData.itemName].inflation = itemSaveData.inflation;
                    }
                    else
                    {
                        Debug.LogWarning($"Item name {itemSaveData.itemName} not found in itemDictionary.");
                    }
                }
            }
        }
    }
}

[System.Serializable]
public struct ItemSaveData
{
    public string itemName;
    public float currentPrice;
    public float inflation;
}
