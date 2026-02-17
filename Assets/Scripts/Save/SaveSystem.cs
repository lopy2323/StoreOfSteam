using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System.Diagnostics;

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
            var stopwatch = Stopwatch.StartNew();

            if (GridManager.instance != null)
                GridManager.instance.LoadGrid();

            if (ItemManager.instance != null)
                ItemManager.instance.LoadItems();

            if (BuildingManager.instance != null)
                BuildingManager.instance.LoadShelfs();

            if (StorageManager.instance != null)
                StorageManager.instance.LoadPackages();

            stopwatch.Stop();
            UnityEngine.Debug.Log($"Load operations took {stopwatch.ElapsedMilliseconds} ms");
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            var stopwatch = Stopwatch.StartNew();

            if (GridManager.instance != null)
                GridManager.instance.SaveGrid();

            if (ItemManager.instance != null)
                ItemManager.instance.SaveItems();

            if (StorageManager.instance != null)
                StorageManager.instance.SaveShelfs();
                StorageManager.instance.SavePackages();

            stopwatch.Stop();
            UnityEngine.Debug.Log($"Save operations took {stopwatch.ElapsedMilliseconds} ms");
        }
    }
    public void SaveSection<T>(string saveFolder, string sectionName, T data)
    {
        UnityEngine.Debug.Log($"Saving section: {sectionName} in folder: {saveFolder}");
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
