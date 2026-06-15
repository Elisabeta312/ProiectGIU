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
    public Image lizardIcon;
    public Image batIcon;
    public Image elephantIcon;
    public Image kiwiIcon;
    public Image waspIcon;
    public Image guppyIcon;
    public Image elkIcon;
    public Image rattiIcon;

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
    public Image foxIcon;
    public Image dodoIcon;
    public Image paradiseParrotIcon;
    public Image passengerPigeonIcon;
    public Image commonSnipeIcon;
    public Image goldenPloverIcon;
    public Image starlingIcon;
    public Image waxwingIcon;
    public Image greatTitIcon;
    public Image macawIcon;
    public Image greatAukIcon;


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
        SetIcon(lizardIcon, "Lizard");
        SetIcon(batIcon, "Bat");
        SetIcon(elephantIcon, "Elephant");
        SetIcon(kiwiIcon, "Kiwi");
        SetIcon(waspIcon, "Wasp");
        SetIcon(guppyIcon, "Guppy");
        SetIcon(elkIcon, "Elk");
        SetIcon(rattiIcon, "Ratti");

        SetIcon(spiderIcon, "Spider");
        SetIcon(spiderBlackIcon, "Spider Black");
        SetIcon(penguinIcon, "Penguin");
        SetIcon(horseIcon, "Horse");
        SetIcon(dogIcon, "Dog");
        SetIcon(chickenIcon, "Chicken");
        SetIcon(catIcon, "Cat");
        SetIcon(boarIcon, "Boar");
        SetIcon(deerIcon, "Deer");
        SetIcon(foxIcon, "Fox");
        SetIcon(dodoIcon, "Dodo");
        SetIcon(paradiseParrotIcon, "Paradise Parrot");
        SetIcon(passengerPigeonIcon, "Passenger Pigeon");
        SetIcon(commonSnipeIcon, "Common Snipe");
        SetIcon(goldenPloverIcon, "Golden Plover");
        SetIcon(starlingIcon, "Starling");
        SetIcon(waxwingIcon, "Waxwing");
        SetIcon(greatTitIcon, "Great Tit");
        SetIcon(macawIcon, "Blue and Yellow Macaw");
        SetIcon(greatAukIcon, "Great Auk");
    }

    void SetIcon(Image icon, string animalName)
    {
        if (icon == null)
            return;

        bool discovered = DiscoveredAnimalsRegistry.IsDiscovered(animalName);

        icon.color = discovered ? Color.white : Color.black;
    }
}