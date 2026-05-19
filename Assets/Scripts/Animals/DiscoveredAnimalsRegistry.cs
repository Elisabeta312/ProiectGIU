using System.Collections.Generic;
using System.IO;
using UnityEngine;

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
    private class JournalSaveData
    {
        public List<string> discoveredAnimals = new List<string>();
    }

    public static void Discover(string animalName)
    {
        if (string.IsNullOrWhiteSpace(animalName))
        {
            return;
        }

        if (!discoveredAnimals.Contains(animalName))
        {
            discoveredAnimals.Add(animalName);
            Save();
        }
    }

    public static bool IsDiscovered(string animalName)
    {
        if (string.IsNullOrWhiteSpace(animalName))
        {
            return false;
        }

        return discoveredAnimals.Contains(animalName);
    }

    public static List<string> GetDiscoveredAnimals()
    {
        return new List<string>(discoveredAnimals);
    }

    public static void SetDiscoveredAnimals(List<string> animals)
    {
        discoveredAnimals.Clear();

        if (animals != null)
        {
            foreach (string animalName in animals)
            {
                if (!string.IsNullOrWhiteSpace(animalName))
                {
                    discoveredAnimals.Add(animalName);
                }
            }
        }

        ApplyDiscoveryStateToSceneAnimals();
        Save();
    }

    public static void ApplyDiscoveryStateToSceneAnimals()
    {
        DiscoverableAnimal[] animalsInScene = Object.FindObjectsByType<DiscoverableAnimal>(FindObjectsSortMode.None);

        foreach (DiscoverableAnimal animal in animalsInScene)
        {
            if (animal == null)
            {
                continue;
            }

            animal.RefreshDiscoveryState();
        }
    }

    public static void Save()
    {
        JournalSaveData data = new JournalSaveData();
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
            ApplyDiscoveryStateToSceneAnimals();
            return;
        }

        string json = File.ReadAllText(SavePath);
        JournalSaveData data = JsonUtility.FromJson<JournalSaveData>(json);

        if (data == null || data.discoveredAnimals == null)
        {
            discoveredAnimals = new HashSet<string>();
        }
        else
        {
            discoveredAnimals = new HashSet<string>(data.discoveredAnimals);
        }

        ApplyDiscoveryStateToSceneAnimals();
    }

    public static void ResetSave()
    {
        discoveredAnimals.Clear();

        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
        }

        ApplyDiscoveryStateToSceneAnimals();
    }
}