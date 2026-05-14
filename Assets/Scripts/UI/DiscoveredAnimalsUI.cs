using UnityEngine;
using UnityEngine.UI;

public class DiscoveredAnimalsUI : MonoBehaviour
{
    public Image bearIcon;
    public Image wolfIcon;
    public Image foxIcon;
    public Image deerIcon;
    public Image rabbitIcon;

    void OnEnable()
    {
        UpdateIcons();
    }

    void UpdateIcons()
    {
        SetIcon(bearIcon, "Bear");
        SetIcon(wolfIcon, "Wolf");
        SetIcon(foxIcon, "Fox");
        SetIcon(deerIcon, "Deer");
        SetIcon(rabbitIcon, "Rabbit");
    }

    void SetIcon(Image icon, string animalName)
    {
        bool discovered = DiscoveredAnimalsRegistry.IsDiscovered(animalName);

        if (discovered)
        {
            icon.color = Color.white;
        }
        else
        {
            icon.color = Color.black;
        }
    }
}
