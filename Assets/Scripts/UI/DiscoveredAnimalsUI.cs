using UnityEngine;
using UnityEngine.UI;

public class DiscoveredAnimalsUI : MonoBehaviour
{
    [Header("Page 1 Icons")]
    public Image bearIcon;
    public Image wolfIcon;
    public Image rabbitIcon;
    public Image tigerIcon;
    public Image goatIcon;
    public Image sheepIcon;
    public Image fishIcon;
    public Image clownFishIcon;
    public Image butterflyIcon;

    [Header("Page 2 Icons")]
    public Image spiderIcon;
    public Image spiderBlackIcon;
    public Image penguinIcon;
    public Image horseIcon;
    public Image dogIcon;
    public Image chickenIcon;
    public Image catIcon;
    public Image boarIcon;
    public Image deerIcon;

    void OnEnable()
    {
        UpdateIcons();
    }

    public void UpdateIcons()
    {
        SetIcon(bearIcon, "Bear");
        SetIcon(wolfIcon, "Wolf");
        SetIcon(rabbitIcon, "Rabbit");
        SetIcon(tigerIcon, "Tiger");
        SetIcon(goatIcon, "Goat");
        SetIcon(sheepIcon, "Sheep");
        SetIcon(fishIcon, "Fish");
        SetIcon(clownFishIcon, "Clown Fish");
        SetIcon(butterflyIcon, "Butterfly");

        SetIcon(spiderIcon, "Spider");
        SetIcon(spiderBlackIcon, "Spider Black");
        SetIcon(penguinIcon, "Penguin");
        SetIcon(horseIcon, "Horse");
        SetIcon(dogIcon, "Dog");
        SetIcon(chickenIcon, "Chicken");
        SetIcon(catIcon, "Cat");
        SetIcon(boarIcon, "Boar");
        SetIcon(deerIcon, "Deer");
    }

    void SetIcon(Image icon, string animalName)
    {
        if (icon == null)
            return;

        bool discovered = DiscoveredAnimalsRegistry.IsDiscovered(animalName);

        icon.color = discovered ? Color.white : Color.black;
    }
}