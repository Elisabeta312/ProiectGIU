using UnityEngine;

public class GameSaveLoader : MonoBehaviour
{
    void Awake()
    {
        DiscoveredAnimalsRegistry.Load();
    }
}