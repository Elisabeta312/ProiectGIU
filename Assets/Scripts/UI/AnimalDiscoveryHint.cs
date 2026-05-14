using UnityEngine;

public class AnimalDiscoveryHint : MonoBehaviour
{
    public Transform player;
    public float detectionDistance = 8f;
    public GameObject pressEHint;

    void Start()
    {
        if (pressEHint != null)
        {
            pressEHint.SetActive(false);
        }
    }

    void Update()
    {
        bool hasNearbyAnimal = HasNearbyUndiscoveredAnimal();

        if (pressEHint != null)
        {
            pressEHint.SetActive(hasNearbyAnimal);
        }
    }

    bool HasNearbyUndiscoveredAnimal()
    {
        if (player == null) return false;

        Collider[] nearbyColliders = Physics.OverlapSphere(player.position, detectionDistance);

        foreach (Collider nearbyCollider in nearbyColliders)
        {
            DiscoverableAnimal animal = nearbyCollider.GetComponentInParent<DiscoverableAnimal>();

            if (animal != null && !animal.discovered)
            {
                return true;
            }
        }

        return false;
    }
}
