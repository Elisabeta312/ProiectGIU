using UnityEngine;

public class DiscoverableAnimal : MonoBehaviour
{
    
    public string animalName = "Wolf";
    public bool discovered = false;

    public void Discover()
    {
        if (discovered) return;

        discovered = true;
        Debug.Log("Animalul " + animalName + " a fost descoperit");
        DiscoveredAnimalsRegistry.Discover(animalName);
    }
}
