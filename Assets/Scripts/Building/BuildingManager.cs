using System;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    private Vector3 mousepos;
    [SerializeField]private GameObject lol;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 hitPosition = hit.point;
                Vector3Int place = GridManager.instance.tilemap.WorldToCell(hitPosition);
                Vector3 fix = GridManager.instance.tilemap.GetCellCenterWorld(place);
                TileData placesed = GridManager.instance.GetTileData(place);
                Debug.Log("hi" + placesed);
                if (placesed.occupied != true)
                {
                    placesed.occupied = true;
                    Instantiate(lol, fix, Quaternion.identity);
                }
            }
        }
    }
}
