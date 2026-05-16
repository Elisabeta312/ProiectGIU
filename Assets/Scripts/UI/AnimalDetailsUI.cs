using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnimalDetailsUI : MonoBehaviour
{
    public GameObject detailsPanel;

    public Image animalIcon;
    public TMP_Text animalTitle;
    public TMP_Text animalDescription;

    [Header("Animal Sprites")]
    public Sprite wolfSprite;
    public Sprite bearSprite;
    public Sprite rabbitSprite;
    public Sprite tigerSprite;

    public Sprite spiderSprite;
    public Sprite spiderBlackSprite;

    public Sprite goatSprite;
    public Sprite sheepSprite;
    public Sprite fishSprite;
    public Sprite clownFishSprite;
    public Sprite butterflySprite;

    public Sprite penguinSprite;
    public Sprite horseSprite;
    public Sprite dogSprite;
    public Sprite chickenSprite;
    public Sprite catSprite;
    public Sprite boarSprite;
    public Sprite deerSprite;

    void Awake()
    {
        detailsPanel.SetActive(false);
    }

    public void ShowAnimal(string animalName)
    {
        detailsPanel.SetActive(true);

        animalTitle.text = animalName;
        animalDescription.text = GetDescription(animalName);
        animalIcon.sprite = GetSprite(animalName);

        animalIcon.enabled = animalIcon.sprite != null;
    }

    string GetDescription(string animalName)
    {
        if (animalName == "Wolf")
            return "Wolves are intelligent animals that live and hunt in packs. They communicate through howls and are important predators in forest ecosystems.";

        if (animalName == "Bear")
            return "Bears are large mammals that usually live in forests and mountain areas. They are strong, curious animals and can search for food across long distances.";

        if (animalName == "Rabbit")
            return "Rabbits are small, fast animals often found in grassy areas. They use their speed and hearing to avoid predators.";

        if (animalName == "Tiger")
            return "Tigers are powerful wild cats known for their strength, striped fur, and solitary lifestyle. They are skilled hunters and usually live in dense forests.";

        if (animalName == "Spider")
            return "Spiders are small creatures often found near trees, rocks, and dark areas. Many spiders build webs to catch insects.";

        if (animalName == "Spider Black")
            return "Black spiders prefer hidden places such as rocks, grass, and forest shadows. They are small but important for controlling insect populations.";

        if (animalName == "Goat")
            return "Goats are agile animals that can climb rocky areas and hills. They are curious, social, and often live in groups.";

        if (animalName == "Sheep")
            return "Sheep are calm herbivores that usually live in groups. They graze on grass and are often found in open fields.";

        if (animalName == "Fish")
            return "Fish live in water and move using their fins. They are an important part of lakes, rivers, and aquatic ecosystems.";

        if (animalName == "Clown Fish")
            return "Clown fish are colorful tropical fish that live near coral reefs. They are known for their bright orange color and their relationship with sea anemones.";

        if (animalName == "Frog")
            return "Frogs live near water and are known for jumping and croaking. They are amphibians, which means they can live both in water and on land.";

        if (animalName == "Butterfly")
            return "Butterflies are colorful insects often found near flowers. They help pollinate plants and make the environment feel more alive.";

        if (animalName == "Penguin")
            return "Penguins are birds that cannot fly but are excellent swimmers. They live in cold regions and spend much of their time in the water.";

        if (animalName == "Horse")
            return "Horses are strong and fast animals often found in open plains and farms. They are intelligent and have lived alongside humans for centuries.";

        if (animalName == "Dog")
            return "Dogs are loyal and social animals known for their strong connection with humans. They can be playful, protective, and very intelligent.";

        if (animalName == "Chicken")
            return "Chickens are small farm birds commonly raised for eggs and food. They usually live in groups and spend much of their time searching for food on the ground.";

        if (animalName == "Cat")
            return "Cats are agile and curious animals known for their independence and hunting skills. They are often active during the night.";

        if (animalName == "Boar")
            return "Boars are wild pigs that live in forests and grasslands. They are strong animals that use their tusks for defense and searching for food.";

        if (animalName == "Deer")
            return "Deer are graceful herbivores commonly found in forests and open fields. They are fast runners and use their hearing to detect danger.";

        if (animalName == "Bird")
            return "Birds are animals with feathers and wings. They can often be found in forests, near trees, or flying above open areas.";

        return "Information about this animal will be added soon.";
    }

    Sprite GetSprite(string animalName)
    {
        if (animalName == "Wolf") return wolfSprite;
        if (animalName == "Bear") return bearSprite;
        if (animalName == "Rabbit") return rabbitSprite;
        if (animalName == "Tiger") return tigerSprite;

        if (animalName == "Spider") return spiderSprite;
        if (animalName == "Spider Black") return spiderBlackSprite;

        if (animalName == "Goat") return goatSprite;
        if (animalName == "Sheep") return sheepSprite;
        if (animalName == "Fish") return fishSprite;
        if (animalName == "Clown Fish") return clownFishSprite;
        if (animalName == "Butterfly") return butterflySprite;

        if (animalName == "Penguin") return penguinSprite;
        if (animalName == "Horse") return horseSprite;
        if (animalName == "Dog") return dogSprite;
        if (animalName == "Chicken") return chickenSprite;
        if (animalName == "Cat") return catSprite;
        if (animalName == "Boar") return boarSprite;
        if (animalName == "Deer") return deerSprite;

        return null;
    }

    public void ClosePanel()
    {
        detailsPanel.SetActive(false);
    }
}