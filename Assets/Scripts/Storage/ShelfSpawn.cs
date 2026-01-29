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

    public void SpawnItem(Item _item)
    {
        if(ShelfItem != _item && ShelfItem != null)
        {
            return;
        }
        if (ShelfItem == null)
        {
            ShelfItem = _item;
            shelfScript.UpdateShelfItems();
        }

        int maxAmountWidth = Mathf.FloorToInt(_shelfWidth / (_item.width + 0.05f));
        int maxAmountDepth = Mathf.FloorToInt(_shelfDepth / (_item.depth + 0.05f));

        if (amountOfItems < maxAmountWidth * maxAmountDepth)
        {
            int xIndex = amountOfItems % maxAmountWidth;
            int zIndex = amountOfItems / maxAmountWidth;

            // Calculate even spacing for the row
            float totalItemWidth = maxAmountWidth * _item.width;
            float totalGapWidth = _shelfWidth - totalItemWidth;
            float gapBetween = maxAmountWidth > 1 ? totalGapWidth / (maxAmountWidth - 1) : 0f;

            float xOffset = xIndex * (_item.width + gapBetween) + (_item.width / 2) - (_shelfWidth / 2);
            float zOffset = zIndex * (_item.depth + 0.05f) + (_item.depth / 2) - (_shelfDepth / 2);
            float yOffset = (_item.height / 2) + 0.01f - (_shelfHeight / 2);
        if (_item == null)
        {
            return;
        }
        if (_itemindexz <= _amountofitemsz)
        {
            if (lastItem == new Vector3(0,0,0))
            {
                float startX = _shelfWidth * 0.5f + _item.width * 0.5f;
                float startZ = _shelfDepth * 0.5f + _item.depth * 0.5f;

                Vector3 spawnPos = new Vector3(startX, 0, startZ) + transform.position;
                Instantiate(_item.itemPrefab, spawnPos, quaternion.identity);

            Vector3 localOffset = (transform.right * xOffset) + (transform.forward * zOffset) + (transform.up * yOffset);
            Vector3 spawnPosition = shelfCollider.bounds.center + localOffset;

            _spawnedItems.Add(Instantiate(_item.itemPrefab, spawnPosition, transform.rotation));
            amountOfItems++;
        }
        else
        {
            Debug.Log("No more space on the shelf!");
            }
        }



    }
    public void addItem(Item it)
    {
        _item = it;
    }
    private void calculateAmountOfItems()
    {
        _amountofitemsx = Mathf.FloorToInt(_shelfWidth / _item.width + 0.05f);
        _amountofitemsz = Mathf.FloorToInt(_shelfDepth / _item.depth + 0.03f);

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
}
