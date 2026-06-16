using UnityEngine;
using System.Collections.Generic;

public static class AnimalCollectionGoal
{
    private static readonly string[] requiredAnimals =
    {
        "Bear",
        "Wolf",
        "Rabbit",
        "Tiger",
        "Goat",
        "Sheep",
        "Fish",
        "ClownFish",
        "Butterfly",
        "Lizard",
        "Bat",
        "Elephant",
        "Kiwi",
        "Wasp",
        "Guppy",
        "Elk",
        "Ratti",
        "Spider",
        "Spider Black",
        "Penguin",
        "Horse",
        "Dog",
        "Chicken",
        "Cat",
        "Boar",
        "Deer",
        "Fox",
        "Dodo",
        // comentez pasarile care se pot gasi doar in pestera
      //  "Paradise Parrot",
       // "Passenger Pigeon",
      //  "Common Snipe",
      //  "Golden Plover",
      //  "Starling",
       // "Spangled Cotinga",
      //  "Great Tit",
      //  "Blue and Yellow Macaw",
      //  "Great Auk",
        "Frog"
    };

    public static bool AreAllAnimalsDiscovered()
    {
     
        foreach (string animalName in requiredAnimals)
        {
            if (!DiscoveredAnimalsRegistry.IsDiscovered(animalName))
            {
                return false;
            }
        } 

        return true;
    }

    public static int GetRequiredAnimalCount()
    {
        return requiredAnimals.Length;
    }

    public static int GetDiscoveredRequiredAnimalCount()
    {
        int count = 0;

        foreach (string animalName in requiredAnimals)
        {
            if (DiscoveredAnimalsRegistry.IsDiscovered(animalName))
            {
                count++;
            }
        }

        return count;
    }

    public static List<string> GetMissingAnimals()
    {
        List<string> missingAnimals = new List<string>();

        foreach (string animalName in requiredAnimals)
        {
            if (!DiscoveredAnimalsRegistry.IsDiscovered(animalName))
            {
                missingAnimals.Add(animalName);
            }
        }

        return missingAnimals;
    }
}