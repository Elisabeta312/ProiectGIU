using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnimalDetailsUI : MonoBehaviour
{
    public GameObject detailsPanel;

    public Image animalIcon;
    public TMP_Text animalTitle;
    public TMP_Text animalDescription;

    [Header("Optional Locked Feedback")]
    public TMP_Text lockedMessageText;
    public float lockedMessageDuration = 1.5f;

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
    public Sprite lizardSprite;
    public Sprite batSprite;
    public Sprite elephantSprite;
    public Sprite kiwiSprite;
    public Sprite waspSprite;
    public Sprite guppySprite;
    public Sprite elkSprite;
    public Sprite rattiSprite;

    public Sprite penguinSprite;
    public Sprite horseSprite;
    public Sprite dogSprite;
    public Sprite chickenSprite;
    public Sprite catSprite;
    public Sprite boarSprite;
    public Sprite deerSprite;
    public Sprite foxSprite;

    private Coroutine lockedMessageRoutine;

    void Awake()
    {
        if (detailsPanel != null)
        {
            detailsPanel.SetActive(false);
        }

        if (lockedMessageText != null)
        {
            lockedMessageText.gameObject.SetActive(false);
        }
    }

    public void ShowAnimal(string animalName)
    {
        if (!DiscoveredAnimalsRegistry.IsDiscovered(animalName))
        {
            ShowLockedMessage();
            return;
        }

        if (detailsPanel != null)
        {
            detailsPanel.SetActive(true);
        }

        if (animalTitle != null)
        {
            animalTitle.text = animalName;
        }

        if (animalDescription != null)
        {
            animalDescription.text = GetDescription(animalName);
        }

        if (animalIcon != null)
        {
            animalIcon.sprite = GetSprite(animalName);
            animalIcon.enabled = animalIcon.sprite != null;
        }
    }

    private void ShowLockedMessage()
    {
        if (lockedMessageText == null)
        {
            return;
        }

        if (lockedMessageRoutine != null)
        {
            StopCoroutine(lockedMessageRoutine);
        }

        lockedMessageRoutine = StartCoroutine(ShowLockedMessageRoutine());
    }

    private System.Collections.IEnumerator ShowLockedMessageRoutine()
    {
        lockedMessageText.text = "Discover this animal first.";
        lockedMessageText.gameObject.SetActive(true);

        yield return new WaitForSeconds(lockedMessageDuration);

        lockedMessageText.gameObject.SetActive(false);
        lockedMessageRoutine = null;
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

        if (animalName == "Lizard")
            return "Lizards are small reptiles that can live in warm areas, forests, and rocky places. They move quickly and use camouflage to stay safe from predators.";

        if (animalName == "Bat")
            return "Bats are the only mammals capable of true flight. They are active mainly at night and use echolocation to find their way and catch insects.";

        if (animalName == "Elephant")
            return "Elephants are the largest land animals in the world. They are intelligent, social, and use their trunks for feeding, drinking, and communication.";

        if (animalName == "Kiwi")
            return "Kiwis are small flightless birds native to New Zealand. They have a long beak and are mostly active during the night.";

        if (animalName == "Wasp")
            return "Wasps are flying insects known for their narrow bodies and ability to sting. They play an important role in controlling other insect populations.";

        if (animalName == "Guppy")
            return "Guppies are small freshwater fish famous for their bright colors and graceful swimming. They are popular in aquariums around the world.";

        if (animalName == "Elk")
            return "Elks are large members of the deer family. They live in forests and open grasslands and are known for the impressive antlers of the males.";

        if (animalName == "Ratti")
            return "Rats are intelligent and adaptable small mammals found in many different environments. They are excellent climbers and can quickly explore new areas in search of food and shelter.";

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

        if (animalName == "Fox")
            return "Foxes are clever and adaptable mammals that live in forests, grasslands, and mountain regions. They are known for their sharp senses, quick movements, and beautiful bushy tails.";

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
        if (animalName == "Lizard") return lizardSprite;
        if (animalName == "Bat") return batSprite;
        if (animalName == "Elephant") return elephantSprite;
        if (animalName == "Kiwi") return kiwiSprite;
        if (animalName == "Wasp") return waspSprite;
        if (animalName == "Guppy") return guppySprite;
        if (animalName == "Elk") return elkSprite;
        if (animalName == "Ratti") return rattiSprite;

        if (animalName == "Penguin") return penguinSprite;
        if (animalName == "Horse") return horseSprite;
        if (animalName == "Dog") return dogSprite;
        if (animalName == "Chicken") return chickenSprite;
        if (animalName == "Cat") return catSprite;
        if (animalName == "Boar") return boarSprite;
        if (animalName == "Deer") return deerSprite;
        if (animalName == "Fox") return foxSprite;

        return null;
    }

    public void ClosePanel()
    {
        if (detailsPanel != null)
        {
            detailsPanel.SetActive(false);
        }
    }
}