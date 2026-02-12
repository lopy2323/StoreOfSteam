using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class SaveSystem : MonoBehaviour
{
    static public SaveSystem instance;
    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            if (GridManager.instance != null)
                GridManager.instance.LoadGrid();

            if (ItemManager.instance != null)
                ItemManager.instance.LoadItems();

            if (BuildingManager.instance != null)
                BuildingManager.instance.LoadShelfs();
        }


        if (Input.GetKeyDown(KeyCode.F4))
        {
            if (GridManager.instance != null)
                GridManager.instance.SaveGrid();

            if (ItemManager.instance != null)
                ItemManager.instance.SaveItems();

            if (StorageManager.instance != null)
                StorageManager.instance.SaveShelfs();

        }
    }
    public void SaveSection<T>(string saveFolder, string sectionName, T data)
    {
        Debug.Log($"Saving section: {sectionName} in folder: {saveFolder}");
        string dir = Path.Combine(Application.persistentDataPath, saveFolder);
        Directory.CreateDirectory(dir);
        string path = Path.Combine(dir, sectionName + ".json");
        File.WriteAllText(path, JsonConvert.SerializeObject(data, Formatting.Indented));
    }

    public T LoadSection<T>(string saveFolder, string sectionName)
    {
        string path = Path.Combine(Application.persistentDataPath, saveFolder, sectionName + ".json");
        if (File.Exists(path))
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
        return default;
    }
}
