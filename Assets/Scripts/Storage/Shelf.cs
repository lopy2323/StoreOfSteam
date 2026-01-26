using System.Collections.Generic;
using UnityEngine;

public class Shelf : MonoBehaviour
{
    private List<Collider> _placment = new List<Collider>();

    public List<Vector3Int> occupiedSpaces = new List<Vector3Int>();

    [SerializeField]private GameObject _itemStoragePoint;

    private void Start()
    {
        foreach (Transform child in _itemStoragePoint.transform)
        {
            Collider col = child.GetComponent<Collider>();
            if (col != null)
            {
                _placment.Add(col);
            }
        }
    }




}
