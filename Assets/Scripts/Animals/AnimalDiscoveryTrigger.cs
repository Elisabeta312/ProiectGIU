using UnityEngine;

public class AnimalDiscoveryTrigger : MonoBehaviour
{
    [Header("Detection")]
    public float discoveryDistance = 4f;
    public LayerMask animalMask = ~0;
    public Transform detectionOrigin;

    [Header("Input")]
    public KeyCode discoveryKey = KeyCode.E;

    [Header("Journal Feedback")]
    public JournalDiscoveryHighlighter journalHighlighter;

    private DiscoverableAnimal currentAnimal;

    private void Start()
    {
        if (detectionOrigin == null)
        {
            detectionOrigin = transform;
        }
    }

    private void Update()
    {
        FindClosestUndiscoveredAnimal();

        if (Input.GetKeyDown(discoveryKey))
        {
            TryDiscoverCurrentAnimal();
        }
    }

    private void FindClosestUndiscoveredAnimal()
    {
        currentAnimal = null;

        Vector3 origin = detectionOrigin.position;

        Collider[] hits = Physics.OverlapSphere(
            origin,
            discoveryDistance,
            animalMask,
            QueryTriggerInteraction.Collide
        );

        float closestDistance = float.MaxValue;

        foreach (Collider hit in hits)
        {
            if (hit == null)
            {
                continue;
            }

            DiscoverableAnimal animal = hit.GetComponentInParent<DiscoverableAnimal>();

            if (animal == null)
            {
                animal = hit.GetComponent<DiscoverableAnimal>();
            }

            if (animal == null)
            {
                continue;
            }

            animal.RefreshDiscoveryState();

            if (!animal.CanBeDiscovered())
            {
                continue;
            }

            float distance = Vector3.Distance(origin, animal.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                currentAnimal = animal;
            }
        }
    }

    private void TryDiscoverCurrentAnimal()
    {
        if (currentAnimal == null)
        {
            return;
        }

        bool discoveredNow = currentAnimal.Discover();

        if (!discoveredNow)
        {
            return;
        }

        if (journalHighlighter != null)
        {
            journalHighlighter.FlashJournal();
        }
        if (AnimalCollectionGoal.AreAllAnimalsDiscovered())
{
    if (QuestFeedbackUI.Instance != null)
    {
        QuestFeedbackUI.Instance.ShowAllAnimalsDiscoveredScreen();
    }
}

        currentAnimal = null;
    }

    private void OnDrawGizmosSelected()
    {
        Transform origin = detectionOrigin;

        if (origin == null)
        {
            origin = transform;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin.position, discoveryDistance);
    }
}