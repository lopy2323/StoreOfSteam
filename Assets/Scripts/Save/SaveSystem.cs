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
    public void SaveData<T>(T data)
    {
        string json = JsonConvert.SerializeObject(data);
        using (StreamWriter writer = new StreamWriter(Application.persistentDataPath + Path.AltDirectorySeparatorChar + "savadata.json"))
        {
            Debug.Log("Saving data: " + json);
            writer.Write(json);
        }
    }
    public T LoadData<T>()
    {
        string path = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "savadata.json";
        if (File.Exists(path))
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string json = reader.ReadToEnd();
                Debug.Log("Loading data: " + json);
                T data = JsonConvert.DeserializeObject<T>(json);
                return data;
            }
        }
        else
        {
            Debug.LogWarning("Save file not found at: " + path);
            return default(T);
        }
    }
}
