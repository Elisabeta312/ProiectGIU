using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class RatSpawner : MonoBehaviour
{
    public GameObject ratPrefab;
    public Transform trashTarget;

    public Terrain[] groundTerrains;
    public bool autoFindTerrainsIfEmpty = true;
    public float groundOffset = 0.05f;

    public int numberOfRats = 8;
    public float minSpawnRadius = 1.5f;
    public float maxSpawnRadius = 4f;

    public float minCircleRadius = 1.5f;
    public float maxCircleRadius = 4f;
    public float minMoveSpeed = 1.2f;
    public float maxMoveSpeed = 2.5f;
    public float turnSpeed = 8f;
    public bool randomClockwiseDirection = true;

    public bool spawnOnlyAtNight = true;
    public MonoBehaviour lightingManager;
    public bool autoFindLightingManager = true;
    public string timeOfDayFieldName = "timeOfDay";
    public float nightStartsAt = 23f;
    public float nightEndsAt = 6f;
    public float checkEverySeconds = 1f;
    public bool removeDuringDay = true;
    public bool showDebugLogs = true;

    private readonly List<GameObject> spawnedRats = new List<GameObject>();
    private float checkTimer;

    private void Start()
    {
        if (ratPrefab == null)
        {
            Debug.LogError("RatSpawner: lipseste ratPrefab.");
            enabled = false;
            return;
        }

        if (trashTarget == null)
        {
            Debug.LogError("RatSpawner: lipseste trashTarget.");
            enabled = false;
            return;
        }

        if (lightingManager == null && autoFindLightingManager)
        {
            lightingManager = FindLightingManager();
        }

        if ((groundTerrains == null || groundTerrains.Length == 0) && autoFindTerrainsIfEmpty)
        {
            groundTerrains = Object.FindObjectsByType<Terrain>(FindObjectsSortMode.None);
        }

        CheckState();
    }

    private void Update()
    {
        checkTimer -= Time.deltaTime;

        if (checkTimer > 0f)
        {
            return;
        }

        checkTimer = checkEverySeconds;
        CheckState();
    }

    private void CheckState()
    {
        RemoveNullRats();

        bool isNight = IsNight();

        if (showDebugLogs)
        {
            float hour;
            bool hasHour = TryGetTimeOfDay(out hour);
            Debug.Log("RatSpawner: hasHour=" + hasHour + ", hour=" + hour + ", isNight=" + isNight + ", count=" + spawnedRats.Count);
        }

        if (!spawnOnlyAtNight)
        {
            if (spawnedRats.Count == 0)
            {
                SpawnRats();
            }

            return;
        }

        if (isNight && spawnedRats.Count == 0)
        {
            SpawnRats();
        }

        if (!isNight && spawnedRats.Count > 0 && removeDuringDay)
        {
            RemoveSpawnedRats();
        }
    }

    private bool IsNight()
    {
        if (!spawnOnlyAtNight)
        {
            return true;
        }

        float hour;

        if (!TryGetTimeOfDay(out hour))
        {
            return false;
        }

        if (nightStartsAt > nightEndsAt)
        {
            return hour >= nightStartsAt || hour < nightEndsAt;
        }

        return hour >= nightStartsAt && hour < nightEndsAt;
    }

    private bool TryGetTimeOfDay(out float hour)
    {
        hour = 0f;

        if (lightingManager == null)
        {
            return false;
        }

        object value;

        if (TryReadMember(lightingManager, timeOfDayFieldName, out value))
        {
            return ConvertToFloat(value, out hour);
        }

        string[] names =
        {
            "timeOfDay",
            "TimeOfDay",
            "currentTimeOfDay",
            "CurrentTimeOfDay",
            "time",
            "Time",
            "hour",
            "Hour"
        };

        for (int i = 0; i < names.Length; i++)
        {
            if (TryReadMember(lightingManager, names[i], out value))
            {
                if (ConvertToFloat(value, out hour))
                {
                    return true;
                }
            }
        }

        FieldInfo[] fields = lightingManager.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        for (int i = 0; i < fields.Length; i++)
        {
            string n = fields[i].Name.ToLower();

            if ((n.Contains("time") || n.Contains("hour")) && fields[i].FieldType == typeof(float))
            {
                hour = (float)fields[i].GetValue(lightingManager);
                return true;
            }
        }

        return false;
    }

    private bool TryReadMember(object target, string memberName, out object value)
    {
        value = null;

        if (target == null || string.IsNullOrEmpty(memberName))
        {
            return false;
        }

        System.Type type = target.GetType();

        FieldInfo field = type.GetField(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (field != null)
        {
            value = field.GetValue(target);
            return true;
        }

        PropertyInfo property = type.GetProperty(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (property != null && property.GetIndexParameters().Length == 0)
        {
            value = property.GetValue(target);
            return true;
        }

        return false;
    }

    private bool ConvertToFloat(object value, out float result)
    {
        result = 0f;

        if (value is float)
        {
            result = (float)value;
            return true;
        }

        if (value is int)
        {
            result = (int)value;
            return true;
        }

        if (value is double)
        {
            result = (float)(double)value;
            return true;
        }

        return false;
    }

    private MonoBehaviour FindLightingManager()
    {
        MonoBehaviour[] all = Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);

        for (int i = 0; i < all.Length; i++)
        {
            if (all[i] != null && all[i].GetType().Name.ToLower().Contains("lightingmanager"))
            {
                return all[i];
            }
        }

        return null;
    }

    private void SpawnRats()
    {
        RemoveNullRats();

        if (spawnedRats.Count > 0)
        {
            return;
        }

        for (int i = 0; i < numberOfRats; i++)
        {
            Vector3 spawnPosition = GetRandomSpawnPositionOnGround();

            GameObject rat = Instantiate(
                ratPrefab,
                spawnPosition,
                Quaternion.Euler(0f, Random.Range(0f, 360f), 0f)
            );

            rat.name = ratPrefab.name + "_Spawned_" + (i + 1);

            RatBehaviour behaviour = rat.GetComponent<RatBehaviour>();

            if (behaviour == null)
            {
                behaviour = rat.AddComponent<RatBehaviour>();
            }

            behaviour.trashTarget = trashTarget;
            behaviour.groundTerrains = groundTerrains;
            behaviour.groundOffset = groundOffset;
            behaviour.circleRadius = Random.Range(minCircleRadius, maxCircleRadius);
            behaviour.moveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);
            behaviour.turnSpeed = turnSpeed;
            behaviour.clockwise = randomClockwiseDirection ? Random.value > 0.5f : true;
            behaviour.startAngle = Random.Range(0f, 360f);

            spawnedRats.Add(rat);
        }

        Debug.Log("RatSpawner: spawned " + numberOfRats);
    }

    private void RemoveSpawnedRats()
    {
        for (int i = spawnedRats.Count - 1; i >= 0; i--)
        {
            if (spawnedRats[i] != null)
            {
                Destroy(spawnedRats[i]);
            }
        }

        spawnedRats.Clear();
        Debug.Log("RatSpawner: removed rats");
    }

    private void RemoveNullRats()
    {
        for (int i = spawnedRats.Count - 1; i >= 0; i--)
        {
            if (spawnedRats[i] == null)
            {
                spawnedRats.RemoveAt(i);
            }
        }
    }

    private Vector3 GetRandomSpawnPositionOnGround()
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float radius = Random.Range(minSpawnRadius, maxSpawnRadius);

        Vector3 offset = new Vector3(
            Mathf.Cos(angle) * radius,
            0f,
            Mathf.Sin(angle) * radius
        );

        return GetGroundPosition(trashTarget.position + offset);
    }

    private Vector3 GetGroundPosition(Vector3 position)
    {
        Terrain terrain = GetTerrainAtPosition(position);

        if (terrain == null)
        {
            return position;
        }

        float y = terrain.transform.position.y + terrain.SampleHeight(position);
        return new Vector3(position.x, y + groundOffset, position.z);
    }

    private Terrain GetTerrainAtPosition(Vector3 position)
    {
        if (groundTerrains == null)
        {
            return null;
        }

        for (int i = 0; i < groundTerrains.Length; i++)
        {
            Terrain terrain = groundTerrains[i];

            if (terrain == null || terrain.terrainData == null)
            {
                continue;
            }

            Vector3 terrainPosition = terrain.transform.position;
            Vector3 terrainSize = terrain.terrainData.size;

            bool insideX = position.x >= terrainPosition.x && position.x <= terrainPosition.x + terrainSize.x;
            bool insideZ = position.z >= terrainPosition.z && position.z <= terrainPosition.z + terrainSize.z;

            if (insideX && insideZ)
            {
                return terrain;
            }
        }

        return null;
    }
}