using Unity.Mathematics;
using UnityEngine;

public class ShelfSpawn : MonoBehaviour
{
    [SerializeField]Collider shelfCollider;
    [SerializeField]private Item _item;

    private float _shelfHeight;
    private float _shelfWidth;
    private float _shelfDepth;

    private Vector3 lastItem;

    private int _amountofitemsz;
    private int _amountofitemsx;

    private int _itemindexx = 0;
    private int _itemindexz = 0;


    private void Start()
    {
        _shelfHeight = shelfCollider.bounds.size.y;
        _shelfWidth = shelfCollider.bounds.size.x;
        _shelfDepth = shelfCollider.bounds.size.z;
    }
    public void SpawnItem()
    {
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

        int _maxamount = _amountofitemsx * _amountofitemsz;
    }
}
