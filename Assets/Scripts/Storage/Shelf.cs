using System.Collections.Generic;
using UnityEngine;

public class Shelf : MonoBehaviour
{
    private List<ShelfSpawn> _placment = new List<ShelfSpawn>();

    public List<Vector3Int> occupiedSpaces = new List<Vector3Int>();

    [SerializeField]private GameObject _itemStoragePoint;

    private void Start()
    {
        foreach (Transform child in _itemStoragePoint.transform)
        {
            ShelfSpawn col = child.GetComponent<ShelfSpawn>();
            if (col != null)
            {
                _placment.Add(col);
            }
        }
    }




}
