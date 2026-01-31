using System.Collections.Generic;
using UnityEngine;

public class TestNPC : MonoBehaviour
{
    [SerializeField] private ItemManager _itemManager;
    private Shelf _shelf;
    private Transform[] targetStandingPoint;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Trying to remove item from shelf");


            List<Shelf> shelfs = _itemManager.GetShelfs("lol");
            if (shelfs.Count == 0)
            {
                _shelf = null;
            }
            foreach (Shelf shelfsItem in shelfs)
            {
                _shelf = shelfsItem;
            }
            if (_shelf != null)
            {
                int removed = _shelf.TakeItem("lol", 1);
                Debug.Log("Removed items: " + removed);
            }

            Debug.Log("found shelf: " + _shelf);
        }
    }
}
