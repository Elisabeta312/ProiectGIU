using UnityEngine;

public class DiscoverableAnimal : MonoBehaviour
{
    [Header("Animal Info")]
    public string animalName = "Wolf";
    public string displayName = "";

    [Header("State")]
    public bool discovered = false;

    private void Start()
    {
        RefreshDiscoveryState();
    }

    public void RefreshDiscoveryState()
    {
        discovered = DiscoveredAnimalsRegistry.IsDiscovered(animalName);
    }

    public bool CanBeDiscovered()
    {
        RefreshDiscoveryState();
        return !discovered;
    }

    public string GetDisplayName()
    {
        if (!string.IsNullOrWhiteSpace(displayName))
        {
            return displayName;
        }

        return animalName;
    }

    public bool Discover()
    {
        RefreshDiscoveryState();

        if (discovered)
        {
            return false;
        }

        discovered = true;

        Debug.Log("Animalul " + animalName + " a fost descoperit");

        DiscoveredAnimalsRegistry.Discover(animalName);

        return true;
    }
}