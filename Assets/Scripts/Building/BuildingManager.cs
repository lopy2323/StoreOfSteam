using System;
using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;

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
    private Transform _buildingTransform;

    private int _currentRotation = 0;
    private int _oldRotation = 0;
    private Quaternion _currentQuaternion = Quaternion.Euler(0, 0, 0);

    private List<Vector3Int> _oldlist = new List<Vector3Int>();

    private bool rotating = false;

    public enum BuildState
    {
        Idle,
        Building
    }

    public BuildState currentBuildState = BuildState.Idle;

    public string _currentBuilding;
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
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            LoadShelfs();
        }


            if (Input.GetKeyDown(KeyCode.R))
        {
            rotating = true;
            _currentRotation += 1;
            if (_currentRotation >= 4)
            {
                _currentRotation = 0;

            }
        }
        if (currentBuildState == BuildState.Building)
        {
            if (instantiated == false)
            {
                _currentHoloBuilding = Instantiate(BuildingDict[_currentBuilding].HoloPrefab, Vector3.zero, _currentQuaternion);
                _currentHoloMeshRenderer = _currentHoloBuilding.GetComponentInChildren<MeshRenderer>();
                instantiated = true;

            }
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (_currentHoloMeshRenderer.material.color == Color.red)
                {
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
                    GameObject shelf = Instantiate(BuildingDict[_currentBuilding].FinalPrefab, _currentHoloBuilding.transform.position, _currentQuaternion);
                    Shelf NameGiving = shelf.GetComponent<Shelf>();
                    NameGiving.ShelfName = _currentBuilding;

                    instantiated = false;
                    Destroy(_currentHoloBuilding);
                    _currentHoloBuilding = null;
                    _currentHoloMeshRenderer = null;

                }
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 6))
            {
                bool canBuildHere = false;
                Vector3 hitPosition = hit.point;
                List<Vector3Int> occupiedPositions;
                Vector3Int res;
                (occupiedPositions, res) = CalculateBuilding(_currentBuilding, hitPosition);
                if (res != _oldOrigin || _currentRotation != _oldRotation)
                {
                    _oldlist.Clear();
                    _oldRotation = _currentRotation;
                    _currentQuaternion = Quaternion.Euler(0, _currentRotation * 90, 0);
                    if (_currentHoloBuilding != null)
                    {
                        _currentHoloBuilding.transform.rotation = _currentQuaternion;

                    }
                    if (_currentRotation >= 4)
                    {
                        _currentQuaternion = Quaternion.Euler(0, 0, 0);

                    }


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
    private (List<Vector3Int>, Vector3Int) CalculateBuilding(string buildingname, Vector3 Worldposbuild)
    {
        List<Vector3Int> occupiedpositions = new List<Vector3Int>();
        Vector3 cal = new Vector3(0, 0, 0);
        float _tempWidth = BuildingDict[buildingname].width;
        float _tempLength = BuildingDict[buildingname].length;
        if (_currentRotation == 1 || _currentRotation == 3)
        {
            _tempWidth = BuildingDict[buildingname].length;
            _tempLength = BuildingDict[buildingname].width;
        }
        if (_currentRotation == 0)
        {
            cal = new Vector3(Worldposbuild.x + ((_tempLength - 1.00f) / 2.00f), 0, Worldposbuild.z - ((_tempWidth - 1.00f) / 2.00f));
        }
        if (_currentRotation == 1)
        {
            cal = new Vector3(Worldposbuild.x - ((_tempLength - 1.00f) / 2.00f), 0, Worldposbuild.z - ((_tempWidth - 1.00f) / 2.00f));
        }
        if (_currentRotation == 2)
        {
            cal = new Vector3(Worldposbuild.x - ((_tempLength - 1.00f) / 2.00f), 0, Worldposbuild.z + ((_tempWidth - 1.00f) / 2.00f));
        }
        if (_currentRotation == 3)
        {
            cal = new Vector3(Worldposbuild.x + ((_tempLength - 1.00f) / 2.00f), 0, Worldposbuild.z + ((_tempWidth - 1.00f) / 2.00f));
        }

        Vector3Int origin = GridManager.instance.tilemap.WorldToCell(cal);

        Vector3Int currentlengthnode = origin;

        for (int i = 0; i < _tempLength; i++)
        {
            occupiedpositions.Add(currentlengthnode);
            Vector3Int currentwidthnode = currentlengthnode;
            for (int j = 0; j < _tempWidth - 1; j++)
            {
                if (_currentRotation == 3 || _currentRotation == 2)
                {
                    currentwidthnode.y -= 1;
                }
                else
                {
                    currentwidthnode.y += 1;
                }
                occupiedpositions.Add(currentwidthnode);
            }
            if (_currentRotation == 2 || _currentRotation == 1)
            {
                currentlengthnode.x += 1;
            }
            else
            {
                currentlengthnode.x -= 1;
            }
        }
        return (occupiedpositions, origin);
    }

    public void StopBuilding()
    {
        if (_currentHoloBuilding != null)
        {
            Destroy(_currentHoloBuilding);
            instantiated = false;
            _currentHoloBuilding = null;
            _currentHoloMeshRenderer = null;
        }
    }

    private void LoadShelfs()
    {
        var list = SaveSystem.instance.LoadSection<List<ShelfData>>("TestSave", "shelfData");
        Debug.Log("Loaded " + list.Count + " shelves.");
        foreach (var item in list)
        {
            GameObject spawned = Instantiate(BuildingDict[item.name].FinalPrefab, new Vector3(item.position[0], item.position[1], item.position[2]), Quaternion.Euler(item.rotation[0], item.rotation[1], item.rotation[2]));
            Shelf shelfScript = spawned.GetComponent<Shelf>();
            shelfScript.ShelfName = item.name;
            List<ShelfSpawn> shelfspawns = new List<ShelfSpawn>();
            shelfspawns.AddRange(spawned.GetComponentsInChildren<ShelfSpawn>());
            shelfspawns.Reverse();
            for (int i = 0; i < shelfspawns.Count; i++)
            {
                for (int j = 0; j < item.SubShelfs[i].amount; j++)
                {
                    shelfspawns[i].SpawnItem(ItemManager.instance.scriptableItemDictionary[item.SubShelfs[i].itemName], null);
                }
            }
        }
    }
    public GameObject itemPrefab;

}