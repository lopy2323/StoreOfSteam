using System;
using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using System.Runtime.CompilerServices;

public class BuildingManager : MonoBehaviour
{
    private Vector3 mousepos;
    [SerializeField] private GameObject lol;

    public BuildObject[] buildObjects;

    public Dictionary<string, Building> BuildingDict = new Dictionary<string, Building>();

    private Vector3Int _oldOrigin;

    private enum BuildState
    {
        Idle,
        Building
    }

    private BuildState currentBuildState = BuildState.Building;

    private string _currentBuilding = "Scrap";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (BuildObject buildObject in buildObjects)
        {
            Building building = new Building
            {
                FinalPrefab = buildObject.FinalPrefab,
                HoloPrefab = buildObject.HoloPrefab,
                width = buildObject.width,
                length = buildObject.length,
                height = buildObject.height
            };
            BuildingDict.Add(buildObject.Name, building);
            Debug.Log("Added " + buildObject.Name + " to BuildingDict");
        }
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
                //Instantiate(lol, hitPosition, Quaternion.identity);
                List<Vector3Int> occupiedPositions;
                Vector3Int res;
                (occupiedPositions, res) = CalculateBuilding(_currentBuilding, hitPosition);

                foreach (Vector3Int item in occupiedPositions)
                {
                    Debug.Log("Checking tile at " + item);
                    TileData data = GridManager.instance.GetTileData(item);
                    if (data == null) Debug.Log("fuck");
                    if (data.occupied == true)
                    {
                        Debug.Log("Cannot build here, tile occupied at " + item); return;
                    }
                }
                Instantiate(BuildingDict[_currentBuilding].FinalPrefab, GridManager.instance.tilemap.GetCellCenterWorld(res), Quaternion.identity);
                foreach (Vector3Int item in occupiedPositions)
                {
                    var data = GridManager.instance.GetTileData(item);
                    data.occupied = true;
                }
            }
        }
    }


    // width is z length is x
    private (List<Vector3Int>,Vector3Int) CalculateBuilding(string buildingname,Vector3 Worldposbuild)
    {
        List<Vector3Int> occupiedpositions = new List<Vector3Int>();

        Debug.Log("Calculating building positions for " + (BuildingDict[buildingname].length - 1.00f) / 2.00f);

        Vector3 cal = new Vector3(Worldposbuild.x + ((BuildingDict[buildingname].length - 1.00f) / 2.00f),0,Worldposbuild.z - ((BuildingDict[buildingname].width - 1.00f) / 2.00f));

        Vector3Int origin = GridManager.instance.tilemap.WorldToCell(cal);

        Vector3Int currentlengthnode = origin;
        
        for (int i = 0; i < BuildingDict[buildingname].length; i++ )
        {
            occupiedpositions.Add(currentlengthnode);
            Vector3Int currentwidthnode = currentlengthnode;
            for (int j = 0; j < BuildingDict[buildingname].width - 1; j++)
            {
                currentwidthnode.y += 1;
                occupiedpositions.Add(currentwidthnode);
            }
            currentlengthnode.x += 1;
        }
        return (occupiedpositions,origin);
    }
}
