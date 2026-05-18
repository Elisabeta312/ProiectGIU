using UnityEngine;

public class DiscoverableAnimal : MonoBehaviour
{
    public string animalName = "Wolf";
    public bool discovered = false;

    private void Start()
    {
        discovered = DiscoveredAnimalsRegistry.IsDiscovered(animalName);
    }

    public void Discover()
    {
        if (discovered)
        {
            return;
        }

        discovered = true;
        Debug.Log("Animalul " + animalName + " a fost descoperit");
        DiscoveredAnimalsRegistry.Discover(animalName);
    }
}