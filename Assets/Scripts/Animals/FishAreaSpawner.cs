using UnityEngine;

public class FishAreaSpawner : MonoBehaviour
{
    [Header("Fish Prefabs")]
    public GameObject[] fishPrefabs;
    public int fishCount = 20;

    [Header("Swim Area")]
    public Vector3 areaSize = new Vector3(20f, 8f, 20f);

    [Header("Fish Movement Settings")]
    public float minSpeed = 1.2f;
    public float maxSpeed = 3.5f;
    public float turnSpeed = 2.5f;
    public float targetReachDistance = 0.6f;

    [Header("Spawn Settings")]
    public bool randomRotationOnSpawn = true;

    private void Start()
    {
        SpawnFish();
    }

    private void SpawnFish()
    {
        if (fishPrefabs == null || fishPrefabs.Length == 0)
        {
            Debug.LogWarning("No fish prefabs assigned in FishAreaSpawner.");
            return;
        }

        for (int i = 0; i < fishCount; i++)
        {
            GameObject prefab = fishPrefabs[Random.Range(0, fishPrefabs.Length)];

            if (prefab == null)
                continue;

            Vector3 spawnPosition = GetRandomPointInsideArea();

            Quaternion spawnRotation = Quaternion.identity;

            if (randomRotationOnSpawn)
            {
                spawnRotation = Quaternion.Euler(
                    0f,
                    Random.Range(0f, 360f),
                    0f
                );
            }

            GameObject fish = Instantiate(prefab, spawnPosition, spawnRotation);

            FishSwim3D swim = fish.GetComponent<FishSwim3D>();

            if (swim == null)
            {
                swim = fish.AddComponent<FishSwim3D>();
            }

            swim.areaCenter = transform;
            swim.areaSize = areaSize;
            swim.minSpeed = minSpeed;
            swim.maxSpeed = maxSpeed;
            swim.turnSpeed = turnSpeed;
            swim.targetReachDistance = targetReachDistance;
        }
    }

    public Vector3 GetRandomPointInsideArea()
    {
        Vector3 center = transform.position;

        float x = Random.Range(-areaSize.x / 2f, areaSize.x / 2f);
        float y = Random.Range(-areaSize.y / 2f, areaSize.y / 2f);
        float z = Random.Range(-areaSize.z / 2f, areaSize.z / 2f);

        return center + new Vector3(x, y, z);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, areaSize);
    }
}