using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class DiscoveredAnimalsRegistry
{
    private static HashSet<string> discoveredAnimals = new HashSet<string>();

    private static string SavePath
    {
        get
        {
            return Path.Combine(Application.persistentDataPath, "discovered_animals.json");
        }
    }

    [System.Serializable]
    private class SaveData
    {
        public List<string> discoveredAnimals = new List<string>();
    }

    public static void Discover(string animalName)
    {
        if (!discoveredAnimals.Contains(animalName))
        {
            discoveredAnimals.Add(animalName);
            Save();
        }
    }

    public static bool IsDiscovered(string animalName)
    {
        return discoveredAnimals.Contains(animalName);
    }

    public static void Save()
    {
        SaveData data = new SaveData();
        data.discoveredAnimals = new List<string>(discoveredAnimals);

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);

        Debug.Log("Animals saved to: " + SavePath);
    }

    public static void Load()
    {
        if (!File.Exists(SavePath))
        {
            discoveredAnimals = new HashSet<string>();
            return;
        }

        string json = File.ReadAllText(SavePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        discoveredAnimals = new HashSet<string>(data.discoveredAnimals);
    }

    public static void ResetSave()
    {
        discoveredAnimals.Clear();

        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
        }
    }
}