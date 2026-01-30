using System.Collections.Generic;
using UnityEngine;

public class Shelf : MonoBehaviour
{
    public List<ShelfSpawn> StorageSpaces = new List<ShelfSpawn>();

    [SerializeField] private List<Item> _storedItems = new List<Item>();
    public Transform[] _targetStandingPoint;
    [SerializeField] private StorageManager _storageManager;

    private ItemManager _itemManager;

    private void Start()
    {
        _itemManager = FindAnyObjectByType<ItemManager>();
    }

    public void UpdateShelfItems()
    {
        _itemManager.RemoveShelfPosition(_storedItems, this);
        _storedItems.Clear();
        foreach (ShelfSpawn shelfSpawn in StorageSpaces)
        {
            if (shelfSpawn != null)
            {
                Item item = shelfSpawn.ShelfItem;
                if (item != null)
                {
                    _storedItems.Add(item);
                    _itemManager.AddShelfPosition(_storedItems, this);
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
                    return removed;
                }
            }
        }
        return 0;
    }
}
