using UnityEngine;
using System.Collections.Generic;

public static class DiscoveredAnimalsRegistry
{
    private static HashSet<string> discoveredAnimals = new HashSet<string>();

    public static void Discover(string animalName)
    {
        discoveredAnimals.Add(animalName);
    }

    public static bool IsDiscovered(string animalName)
    {
        return discoveredAnimals.Contains(animalName);
    }
}
