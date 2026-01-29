using System.Collections.Generic;
using UnityEngine;

public class Shelf : MonoBehaviour
{
    public List<ShelfSpawn> StorageSpaces = new List<ShelfSpawn>();

    [SerializeField] private List<Item> _storedItems = new List<Item>();

    private void Start()
    {

    }

    public void UpdateShelfItems()
    {
        _storedItems.Clear();
        foreach (ShelfSpawn shelfSpawn in StorageSpaces)
        {
            if (shelfSpawn != null)
            {
                Item item = shelfSpawn.ShelfItem;
                if (item != null)
                {
                    _storedItems.Add(item);
                }
            }
        }
    }
}
