using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(BoxCollider))]
public class TerrestrialAnimalSpawner : MonoBehaviour
{
    [Header("Animal")]
    public string spawnId = "Goat";

    [Tooltip("Daca ramane gol, scriptul foloseste acest GameObject ca template.")]
    public GameObject animalPrefab;

    [Header("Spawn")]
    public int defaultSpawnCount = 5;
    public int minSpawnCount = 0;
    public int maxSpawnCount = 50;

    [Header("Runtime Rules")]
    public bool spawnOnlyOnNewGame = true;

    [Tooltip("Bifeaza true daca vrei sa testezi direct din MainGameScene fara sa pornesti din UIMenu -> New Game.")]
    public bool allowDirectSceneTesting = true;

    [Tooltip("Ascunde animalul original dupa ce creeaza copiile.")]
    public bool hideTemplateAtRuntime = true;

    [Header("Ground Detection")]
    public LayerMask groundMask = ~0;
    public float rayStartHeight = 80f;
    public float rayDistance = 200f;

    [Header("Roaming")]
    public bool addRoamerToSpawnedAnimals = true;
    public float roamSpeed = 1.8f;
    public float waitMinSeconds = 1f;
    public float waitMaxSeconds = 4f;
    public float destinationTolerance = 1.2f;

    private BoxCollider spawnArea;
    private bool hasSpawned = false;

    private void Awake()
    {
        spawnArea = GetComponent<BoxCollider>();
        spawnArea.isTrigger = true;
    }

    private void Start()
    {
        bool canSpawn = true;

        if (spawnOnlyOnNewGame)
        {
            canSpawn = GameSessionState.IsNewGameStart || allowDirectSceneTesting;
        }

        if (!canSpawn)
        {
            if (hideTemplateAtRuntime)
            {
                gameObject.SetActive(false);
            }

            return;
        }

        SpawnAnimals();

        if (hideTemplateAtRuntime)
        {
            gameObject.SetActive(false);
        }
    }

    public void SpawnAnimals()
    {
        if (hasSpawned)
        {
            return;
        }

        hasSpawned = true;

        int spawnCount = Mathf.Clamp(defaultSpawnCount, minSpawnCount, maxSpawnCount);

        GameObject template = animalPrefab;

        if (template == null)
        {
            template = gameObject;
        }

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 spawnPosition = GetRandomPointInsideBoxCollider();

            GameObject spawnedAnimal = Instantiate(
                template,
                spawnPosition,
                Quaternion.Euler(0f, Random.Range(0f, 360f), 0f)
            );

            spawnedAnimal.name = spawnId + "_Spawned_" + (i + 1);
            spawnedAnimal.SetActive(true);

            TerrestrialAnimalSpawner cloneSpawner = spawnedAnimal.GetComponent<TerrestrialAnimalSpawner>();

            if (cloneSpawner != null)
            {
                Destroy(cloneSpawner);
            }

            if (addRoamerToSpawnedAnimals)
            {
                TerrestrialAnimalRoamer roamer = spawnedAnimal.GetComponent<TerrestrialAnimalRoamer>();

                if (roamer == null)
                {
                    roamer = spawnedAnimal.AddComponent<TerrestrialAnimalRoamer>();
                }

                roamer.SetRoamArea(GetWorldBoxCenter(), GetWorldBoxSizeXZ());
                roamer.moveSpeed = roamSpeed;
                roamer.waitMinSeconds = waitMinSeconds;
                roamer.waitMaxSeconds = waitMaxSeconds;
                roamer.destinationTolerance = destinationTolerance;
                roamer.groundMask = groundMask;
            }

            NavMeshAgent agent = spawnedAnimal.GetComponent<NavMeshAgent>();

            if (agent != null && agent.enabled)
            {
                agent.Warp(spawnPosition);
            }
        }

        Debug.Log("Spawned " + spawnCount + " animals of type " + spawnId);
    }

    private Vector3 GetRandomPointInsideBoxCollider()
    {
        Vector3 boxCenter = GetWorldBoxCenter();
        Vector3 boxSize = GetWorldBoxSize();

        float randomX = Random.Range(boxCenter.x - boxSize.x * 0.5f, boxCenter.x + boxSize.x * 0.5f);
        float randomZ = Random.Range(boxCenter.z - boxSize.z * 0.5f, boxCenter.z + boxSize.z * 0.5f);

        Vector3 rayOrigin = new Vector3(randomX, boxCenter.y + rayStartHeight, randomZ);
        Ray ray = new Ray(rayOrigin, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, groundMask, QueryTriggerInteraction.Ignore))
        {
            return hit.point;
        }

        return new Vector3(randomX, transform.position.y, randomZ);
    }

    private Vector3 GetWorldBoxCenter()
    {
        return transform.TransformPoint(spawnArea.center);
    }

    private Vector3 GetWorldBoxSize()
    {
        Vector3 scaledSize = Vector3.Scale(spawnArea.size, transform.lossyScale);
        return scaledSize;
    }

    private Vector2 GetWorldBoxSizeXZ()
    {
        Vector3 size = GetWorldBoxSize();
        return new Vector2(size.x, size.z);
    }

    private void OnDrawGizmosSelected()
    {
        BoxCollider box = GetComponent<BoxCollider>();

        if (box == null)
        {
            return;
        }

        Gizmos.color = Color.yellow;

        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.DrawWireCube(box.center, box.size);

        Gizmos.color = new Color(1f, 1f, 0f, 0.12f);
        Gizmos.DrawCube(box.center, box.size);

        Gizmos.matrix = oldMatrix;
    }
}