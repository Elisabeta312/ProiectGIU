using UnityEngine;

public class BirdSquareSpawner : MonoBehaviour
{
    [Header("Bird Prefab")]
    public GameObject birdPrefab;

    [Header("Spawn Settings")]
    public int numberOfBirds = 10;

    [Header("Random Altitude")]
    public float minSpawnHeight = 5f;
    public float maxSpawnHeight = 12f;

    [Header("Square Constraint")]
    public Vector2 squareSize = new Vector2(30f, 30f);

    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float turnSmoothSpeed = 8f;

    [Header("Random Scale")]
    public bool randomScale = false;
    public float minScale = 0.8f;
    public float maxScale = 1.2f;

    private void Start()
    {
        SpawnBirds();
    }

    private void SpawnBirds()
    {
        if (birdPrefab == null)
        {
            Debug.LogWarning("Bird prefab is missing on " + gameObject.name);
            return;
        }

        if (maxSpawnHeight < minSpawnHeight)
        {
            maxSpawnHeight = minSpawnHeight;
        }

        for (int i = 0; i < numberOfBirds; i++)
        {
            float randomHeight = Random.Range(minSpawnHeight, maxSpawnHeight);
            Vector3 randomPosition = GetRandomPointInsideSquare(randomHeight);

            GameObject bird = Instantiate(
                birdPrefab,
                randomPosition,
                Quaternion.identity
            );

            bird.name = birdPrefab.name + "_Generated_" + i;

            if (randomScale)
            {
                float scale = Random.Range(minScale, maxScale);
                bird.transform.localScale = Vector3.one * scale;
            }

            BirdSquareMover mover = bird.GetComponent<BirdSquareMover>();

            if (mover == null)
            {
                mover = bird.AddComponent<BirdSquareMover>();
            }

            mover.center = transform;
            mover.squareSize = squareSize;
            mover.moveSpeed = moveSpeed;
            mover.turnSmoothSpeed = turnSmoothSpeed;
            mover.fixedHeight = randomHeight;
        }
    }

    private Vector3 GetRandomPointInsideSquare(float height)
    {
        float halfX = squareSize.x * 0.5f;
        float halfZ = squareSize.y * 0.5f;

        float randomX = Random.Range(-halfX, halfX);
        float randomZ = Random.Range(-halfZ, halfZ);

        Vector3 center = transform.position;

        return new Vector3(
            center.x + randomX,
            center.y + height,
            center.z + randomZ
        );
    }

    private void OnValidate()
    {
        if (maxSpawnHeight < minSpawnHeight)
        {
            maxSpawnHeight = minSpawnHeight;
        }

        if (numberOfBirds < 0)
        {
            numberOfBirds = 0;
        }

        if (minScale < 0.01f)
        {
            minScale = 0.01f;
        }

        if (maxScale < minScale)
        {
            maxScale = minScale;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Vector3 minCenter = transform.position + Vector3.up * minSpawnHeight;
        Vector3 maxCenter = transform.position + Vector3.up * maxSpawnHeight;

        Vector3 size = new Vector3(squareSize.x, 0.1f, squareSize.y);

        Gizmos.DrawWireCube(minCenter, size);
        Gizmos.DrawWireCube(maxCenter, size);

        DrawVerticalCornerLines();
    }

    private void DrawVerticalCornerLines()
    {
        float halfX = squareSize.x * 0.5f;
        float halfZ = squareSize.y * 0.5f;

        Vector3 center = transform.position;

        Vector3[] corners =
        {
            new Vector3(center.x - halfX, center.y, center.z - halfZ),
            new Vector3(center.x - halfX, center.y, center.z + halfZ),
            new Vector3(center.x + halfX, center.y, center.z - halfZ),
            new Vector3(center.x + halfX, center.y, center.z + halfZ)
        };

        for (int i = 0; i < corners.Length; i++)
        {
            Vector3 bottom = corners[i] + Vector3.up * minSpawnHeight;
            Vector3 top = corners[i] + Vector3.up * maxSpawnHeight;

            Gizmos.DrawLine(bottom, top);
        }
    }
}