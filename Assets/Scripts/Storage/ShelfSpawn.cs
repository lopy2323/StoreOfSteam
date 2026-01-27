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

    private int amountOfItems;

    private void Start()
    {
        _shelfHeight = shelfCollider.bounds.size.y;
        _shelfWidth = shelfCollider.bounds.size.x;
        _shelfDepth = shelfCollider.bounds.size.z;
    }
    public void SpawnItem(Item _items)
    {
        _item = _items;

        int maxAmount = Mathf.FloorToInt(_shelfWidth / (_item.width + 0.05f));

        if (_item != null)
        {
            if (lastItem == new Vector3(0,0,0))
            {
                var firstItemPosition = shelfCollider.bounds.min.x;
                lastItem = new Vector3(firstItemPosition,2,0);
                Instantiate(_item.itemPrefab, lastItem, quaternion.identity, transform);
                amountOfItems++;
            }
            if (amountOfItems < maxAmount)
            {
                lastItem += new Vector3(_item.width + 0.05f, 0, 0);
                Instantiate(_item.itemPrefab, lastItem, quaternion.identity, transform);
                amountOfItems++;
            }
        }
    }

}
