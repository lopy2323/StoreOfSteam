using System.Collections.Generic;
using UnityEngine;

public class Shelf : MonoBehaviour
{
    public List<ShelfSpawn> StorageSpaces = new List<ShelfSpawn>();

    public List<Item> storedItems = new List<Item>();
    public Transform[] _targetStandingPoint;
    [SerializeField] private StorageManager _storageManager;

    private ItemManager _itemManager;
    public string ShelfName = "please input shelf prefab name here";

    private void Awake()
    {
        _itemManager = FindAnyObjectByType<ItemManager>();
    }

    public void UpdateShelfItems()
    {
        _itemManager.RemoveShelfPosition(storedItems, this);
        storedItems.Clear();
        foreach (ShelfSpawn shelfSpawn in StorageSpaces)
        {
            if (shelfSpawn != null)
            {
                Item item = shelfSpawn.ShelfItem;
                if (item != null)
                {
                    storedItems.Add(item);
                    _itemManager.AddShelfPosition(storedItems, this);
                }
            }
        }
    }

    public int TakeItem(string itemName, int removeAmount)
    {
        foreach (ShelfSpawn shelfSpawn in StorageSpaces)
        {
            if (shelfSpawn != null)
            {
                Item item = shelfSpawn.ShelfItem;
                if (item != null && item.itemName == itemName)
                {
                    int removed = shelfSpawn.RemoveItemNPC(item, removeAmount);
                    UpdateShelfItems();
                    
                    return removeAmount - removed;
                }
            }
        }
        return 0;
    }
}
