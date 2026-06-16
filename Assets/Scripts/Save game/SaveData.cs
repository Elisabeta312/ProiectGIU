using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public string saveName;
    public int slotIndex;
    public string sceneName;
    public string savedAt;

    public float playerPosX;
    public float playerPosY;
    public float playerPosZ;

    public float playerRotX;
    public float playerRotY;
    public float playerRotZ;

    public List<string> discoveredAnimals = new List<string>();
    public List<string> acquiredKeys = new List<string>();
    public List<string> openedCaveEntrances = new List<string>();
    public bool allAnimalsScreenAlreadyShown;
}