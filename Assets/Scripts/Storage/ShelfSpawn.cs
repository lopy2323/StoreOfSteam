using UnityEngine;
using System.Collections.Generic;
using System;

public class ShelfSpawn : MonoBehaviour
{
    [SerializeField] private Collider shelfCollider;
    [SerializeField] private Shelf shelfScript;
    public Item ShelfItem = null;

    private float _shelfHeight;
    private float _shelfWidth;
    private float _shelfDepth;

    private int amountOfItems;
    private List<GameObject> _spawnedItems = new List<GameObject>();

    private void Start()
    {
        // Get the collider's bounds in local space
        Vector3 localSize = transform.InverseTransformVector(shelfCollider.bounds.size);

        _shelfWidth = Mathf.Abs(localSize.x);
        _shelfHeight = Mathf.Abs(localSize.y);
        _shelfDepth = Mathf.Abs(localSize.z);
    }

    public void SpawnItem(Item _item, PackageScript activePackageScript)
    {
        float edgeMargin = 0.05f; // Adjust this value as needed

        if (ShelfItem != _item && ShelfItem != null)
        {
            return;
        }
        if (ShelfItem == null)
        {
            ShelfItem = _item;
            shelfScript.UpdateShelfItems();
        }

        // Calculate available width and depth after accounting for edge margins
        float availableWidth = _shelfWidth - 2 * edgeMargin;
        float availableDepth = _shelfDepth - 2 * edgeMargin;

        int maxAmountWidth = Mathf.FloorToInt(availableWidth / (_item.width + 0.05f));
        int maxAmountDepth = Mathf.FloorToInt(availableDepth / (_item.depth + 0.05f));

        if (amountOfItems < maxAmountWidth * maxAmountDepth)
        {
            int xIndex = amountOfItems % maxAmountWidth;
            int zIndex = amountOfItems / maxAmountWidth;

            // Calculate even spacing for the row
            float totalItemWidth = maxAmountWidth * _item.width;
            float totalGapWidth = availableWidth - totalItemWidth;
            float gapBetween = maxAmountWidth > 1 ? totalGapWidth / (maxAmountWidth - 1) : 0f;

            float xOffset = edgeMargin + xIndex * (_item.width + gapBetween) + (_item.width / 2) - (_shelfWidth / 2);
            float zOffset = edgeMargin + zIndex * (_item.depth + 0.05f) + (_item.depth / 2) - (_shelfDepth / 2);
            float yOffset = (_item.height / 2) + 0.01f - (_shelfHeight / 2);

            Vector3 localOffset = (transform.right * xOffset) + (transform.forward * zOffset) + (transform.up * yOffset);
            Vector3 spawnPosition = shelfCollider.bounds.center + localOffset;

            _spawnedItems.Add(Instantiate(_item.itemPrefab, spawnPosition, transform.rotation));
            activePackageScript.packageAmount--;
            activePackageScript.Updatetext();
            amountOfItems++;
        }
        else
        {
            Debug.Log("No more space on the shelf!");
        }
    }

    public void RemoveItem(Item item, Action<bool> callback)
    {
        if (item != ShelfItem) return;
        if (amountOfItems > 0)
        {
            amountOfItems--;
            callback?.Invoke(true);
            GameObject itemToRemove = _spawnedItems[_spawnedItems.Count - 1];
            _spawnedItems.RemoveAt(_spawnedItems.Count - 1);
            Destroy(itemToRemove);
        }
        if (amountOfItems == 0)
        {
            this.ShelfItem = null;
            shelfScript.UpdateShelfItems();
            return;
        }
    }
    public int RemoveItemNPC(Item item, int removeAmount)
    {
        if (item != ShelfItem || amountOfItems == 0)
            return 0;

        int toRemove = Mathf.Min(removeAmount, amountOfItems);

        for (int i = 0; i < toRemove; i++)
        {
            amountOfItems--;
            GameObject itemToRemove = _spawnedItems[_spawnedItems.Count - 1];
            _spawnedItems.RemoveAt(_spawnedItems.Count - 1);
            Destroy(itemToRemove);
        }
        if (amountOfItems == 0)
        {
            this.ShelfItem = null;
        }

        return toRemove;
    }
}
