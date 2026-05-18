using System.IO;
using UnityEngine;

public static class SaveSystem
{
    public const int MaxSlots = 3;
    public const string FolderName = "Saves Log";

    public static string GetSaveFolderPath()
    {
#if UNITY_EDITOR
        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        string path = Path.Combine(projectRoot, FolderName);
#else
        string gameFolder = Directory.GetParent(Application.dataPath).FullName;
        string path = Path.Combine(gameFolder, FolderName);
#endif

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        return path;
    }

    public static string GetSlotPath(int slotIndex)
    {
        return Path.Combine(GetSaveFolderPath(), "save_slot_" + slotIndex + ".json");
    }

    public static bool SlotExists(int slotIndex)
    {
        return File.Exists(GetSlotPath(slotIndex));
    }

    public static void SaveGame(SaveData data)
    {
        if (data == null)
        {
            Debug.Log("Save data is null.");
            return;
        }

        string json = JsonUtility.ToJson(data, true);
        string path = GetSlotPath(data.slotIndex);

        File.WriteAllText(path, json);

        Debug.Log("Game saved to: " + path);
        Debug.Log("Saved name: " + data.saveName);
    }

    public static SaveData LoadGame(int slotIndex)
    {
        string path = GetSlotPath(slotIndex);

        if (!File.Exists(path))
        {
            return null;
        }

        string json = File.ReadAllText(path);

        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        return JsonUtility.FromJson<SaveData>(json);
    }

    public static void DeleteGame(int slotIndex)
    {
        string path = GetSlotPath(slotIndex);

        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}