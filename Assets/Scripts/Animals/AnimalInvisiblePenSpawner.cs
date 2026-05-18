using UnityEngine;

public class AnimalInvisiblePenSpawner : MonoBehaviour
{
    [Header("Animal")]
    public string spawnId = "Goat";
    public int defaultSpawnCount = 2;

    [Tooltip("Daca ramane gol, foloseste acest GameObject ca template.")]
    public GameObject animalPrefab;

    [Header("Invisible Pen")]
    public Collider penCollider;

    [Header("Terrain")]
    public Terrain terrain;
    public LayerMask groundMask = ~0;
    public bool useTerrainHeight = true;
    public float yOffset = 0f;
    public float rayStartHeight = 80f;
    public float rayDistance = 200f;

    [Header("Rules")]
    public bool spawnOnlyOnNewGame = true;
    public bool allowDirectSceneTesting = true;
    public bool hideTemplateAtRuntime = true;
    public bool removePenFromSpawnedAnimals = true;

    [Header("Roaming")]
    public bool addRoamerToSpawnedAnimals = true;
    public float roamSpeed = 1.8f;
    public float rotationSpeed = 6f;
    public float waitMinSeconds = 1f;
    public float waitMaxSeconds = 4f;
    public float destinationTolerance = 1.2f;

    [Header("Animation")]
    public bool controlAnimator = true;
    public string movingBoolParameter = "isWalking";
    public string speedFloatParameter = "Speed";

    private bool hasSpawned = false;

    private void Awake()
    {
        AutoFindPenColliderIfNeeded();

        if (penCollider != null)
        {
            penCollider.isTrigger = true;
        }

        if (terrain == null)
        {
            terrain = Terrain.activeTerrain;
        }
    }

    private void Start()
    {
        AutoFindPenColliderIfNeeded();

        if (penCollider == null)
        {
            Debug.LogWarning(name + " has no invisible pen collider assigned.");
            return;
        }

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

        if (penCollider == null)
        {
            Debug.LogWarning(name + " cannot spawn because penCollider is missing.");
            return;
        }

        GameObject template = animalPrefab;

        if (template == null)
        {
            template = gameObject;
        }

        int spawnCount = Mathf.Max(0, defaultSpawnCount);

        Bounds penBounds = penCollider.bounds;
        Vector3 roamCenter = penBounds.center;
        Vector3 roamSize = penBounds.size;

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 spawnPosition = GetRandomGroundPointInsidePen(penBounds);

            GameObject spawnedAnimal = Instantiate(
                template,
                spawnPosition,
                Quaternion.Euler(0f, Random.Range(0f, 360f), 0f)
            );

            spawnedAnimal.name = spawnId + "_Spawned_" + (i + 1);
            spawnedAnimal.SetActive(true);

            AnimalInvisiblePenSpawner cloneSpawner = spawnedAnimal.GetComponent<AnimalInvisiblePenSpawner>();

            if (cloneSpawner != null)
            {
                Destroy(cloneSpawner);
            }

            TerrestrialAnimalRoamer oldRoamer = spawnedAnimal.GetComponent<TerrestrialAnimalRoamer>();

            if (oldRoamer != null)
            {
                Destroy(oldRoamer);
            }

            if (removePenFromSpawnedAnimals && penCollider != null)
            {
                Transform clonePen = FindChildByName(spawnedAnimal.transform, penCollider.gameObject.name);

                if (clonePen != null)
                {
                    Destroy(clonePen.gameObject);
                }
            }

            if (addRoamerToSpawnedAnimals)
            {
                TerrestrialAnimalRoamer roamer = spawnedAnimal.AddComponent<TerrestrialAnimalRoamer>();

                roamer.SetRoamArea(roamCenter, roamSize);

                roamer.terrain = terrain;
                roamer.groundMask = groundMask;
                roamer.useTerrainHeight = useTerrainHeight;
                roamer.yOffset = yOffset;
                roamer.rayStartHeight = rayStartHeight;
                roamer.rayDistance = rayDistance;

                roamer.moveSpeed = roamSpeed;
                roamer.rotationSpeed = rotationSpeed;
                roamer.waitMinSeconds = waitMinSeconds;
                roamer.waitMaxSeconds = waitMaxSeconds;
                roamer.destinationTolerance = destinationTolerance;

                roamer.controlAnimator = controlAnimator;
                roamer.movingBoolParameter = movingBoolParameter;
                roamer.speedFloatParameter = speedFloatParameter;
            }
        }

        Debug.Log("Spawned " + spawnCount + " animals of type " + spawnId);
    }

    private Vector3 GetRandomGroundPointInsidePen(Bounds penBounds)
    {
        for (int attempt = 0; attempt < 40; attempt++)
        {
            float randomX = Random.Range(penBounds.min.x, penBounds.max.x);
            float randomZ = Random.Range(penBounds.min.z, penBounds.max.z);

            Vector3 point = new Vector3(randomX, penBounds.center.y, randomZ);

            if (!IsPointInsideBoundsXZ(point, penBounds))
            {
                continue;
            }

            point.y = GetGroundY(point) + yOffset;

            return point;
        }

        Vector3 fallback = penBounds.center;
        fallback.y = GetGroundY(fallback) + yOffset;
        return fallback;
    }

    private float GetGroundY(Vector3 position)
    {
        if (useTerrainHeight && terrain != null)
        {
            return terrain.SampleHeight(position) + terrain.transform.position.y;
        }

        Vector3 rayOrigin = new Vector3(position.x, position.y + rayStartHeight, position.z);
        Ray ray = new Ray(rayOrigin, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, groundMask, QueryTriggerInteraction.Ignore))
        {
            return hit.point.y;
        }

        return position.y;
    }

    private bool IsPointInsideBoundsXZ(Vector3 point, Bounds bounds)
    {
        return point.x >= bounds.min.x &&
               point.x <= bounds.max.x &&
               point.z >= bounds.min.z &&
               point.z <= bounds.max.z;
    }

    private void AutoFindPenColliderIfNeeded()
    {
        if (penCollider != null)
        {
            return;
        }

        Collider[] childColliders = GetComponentsInChildren<Collider>(true);

        foreach (Collider childCollider in childColliders)
        {
            if (childCollider.gameObject == gameObject)
            {
                continue;
            }

            if (childCollider.name.ToLower().Contains("pen") ||
                childCollider.name.ToLower().Contains("tarc") ||
                childCollider.name.ToLower().Contains("invisible"))
            {
                penCollider = childCollider;
                return;
            }
        }

        foreach (Collider childCollider in childColliders)
        {
            if (childCollider.gameObject != gameObject)
            {
                penCollider = childCollider;
                return;
            }
        }
    }

    private Transform FindChildByName(Transform root, string childName)
    {
        foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == childName)
            {
                return child;
            }
        }

        return null;
    }

    private void OnDrawGizmosSelected()
    {
        AutoFindPenColliderIfNeeded();

        if (penCollider == null)
        {
            return;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(penCollider.bounds.center, penCollider.bounds.size);

        Gizmos.color = new Color(1f, 1f, 0f, 0.12f);
        Gizmos.DrawCube(penCollider.bounds.center, penCollider.bounds.size);
    }
}