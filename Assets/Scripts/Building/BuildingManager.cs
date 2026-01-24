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
    private bool instantiated = false;
    private GameObject _currentHoloBuilding;
    private MeshRenderer _currentHoloMeshRenderer;

    private List<Vector3Int> _oldlist = new List<Vector3Int>();

    private enum BuildState
    {
        Idle,
        Building
    }

    private BuildState currentBuildState = BuildState.Building;

    private string _currentBuilding = "Test";
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
        if (currentBuildState == BuildState.Building)
        {
            if (instantiated == false)
            {
                _currentHoloBuilding = Instantiate(BuildingDict[_currentBuilding].HoloPrefab, Vector3.zero, Quaternion.identity);
                _currentHoloMeshRenderer = _currentHoloBuilding.GetComponentInChildren<MeshRenderer>();
                instantiated = true;

            }
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (_currentHoloMeshRenderer.material.color == Color.red)
                {
                    Debug.Log("Cannot build here!");
                }
                if (_currentHoloMeshRenderer.material.color == Color.green)
                {


                    foreach (Vector3Int item in _oldlist)
                    {
                        TileData data = GridManager.instance.GetTileData(item);
                        if (data.occupied == true)
                        {
                            return;
                        }
                        data.occupied = true;
                    }
                    Instantiate(BuildingDict[_currentBuilding].FinalPrefab,_currentHoloBuilding.transform.position, Quaternion.identity);
                    instantiated = false;
                    Destroy(_currentHoloBuilding);
                    _currentHoloBuilding = null;
                    _currentHoloMeshRenderer = null;
                    
                }
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                bool canBuildHere = false;
                Vector3 hitPosition = hit.point;
                List<Vector3Int> occupiedPositions;
                Vector3Int res;
                (occupiedPositions, res) = CalculateBuilding(_currentBuilding, hitPosition);
                if (res != _oldOrigin)
                {
                    _oldlist.Clear();
                    _oldOrigin = res;
                    foreach (Vector3Int item in occupiedPositions)
                    {
                        TileData data = GridManager.instance.GetTileData(item);
                        if (data.occupied == true)
                        {
                            canBuildHere = false;
                            break;
                        }
                        canBuildHere = true;
                    }
                    if (canBuildHere == true)
                    {
                        _currentHoloBuilding.transform.position = GridManager.instance.tilemap.GetCellCenterWorld(res);
                        _currentHoloMeshRenderer.material.color = Color.green;
                        foreach (var item in occupiedPositions)
                        {
                            _oldlist.Add(item);
                        }
                    }
                    if (canBuildHere == false)
                    {
                        _currentHoloBuilding.transform.position = GridManager.instance.tilemap.GetCellCenterWorld(res);
                        _currentHoloMeshRenderer.material.color = Color.red;
                    }
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
