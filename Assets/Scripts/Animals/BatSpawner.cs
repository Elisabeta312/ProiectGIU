using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class BatSpawner : MonoBehaviour
{
    public GameObject batTemplateFromScene;
    public bool hideOriginalBat = true;

    public int numberOfBats = 10;

    public Vector2 areaSize = new Vector2(30f, 30f);
    public float minAltitude = 0f;
    public float maxAltitude = 8f;

    public float minSpeed = 2f;
    public float maxSpeed = 5f;
    public float turnSpeed = 4f;
    public float targetReachDistance = 0.7f;
    public float minWaitTime = 0.2f;
    public float maxWaitTime = 1.2f;

    public bool spawnOnlyAtNight = true;
    public MonoBehaviour lightingManager;
    public bool autoFindLightingManager = true;
    public string timeOfDayFieldName = "timeOfDay";
    public float nightStartsAt = 23f;
    public float nightEndsAt = 6f;
    public float checkEverySeconds = 1f;
    public bool removeDuringDay = true;
    public bool showDebugLogs = true;

    private readonly List<GameObject> spawnedBats = new List<GameObject>();
    private float checkTimer;
    private Vector3 areaCenter;
    private bool originalHidden;

    private void Start()
    {
        if (batTemplateFromScene == null)
        {
            Debug.LogError("BatSpawner: lipseste batTemplateFromScene.");
            enabled = false;
            return;
        }

        if (lightingManager == null && autoFindLightingManager)
        {
            lightingManager = FindLightingManager();
        }

        areaCenter = batTemplateFromScene.transform.position;

        if (hideOriginalBat && !originalHidden)
        {
            batTemplateFromScene.SetActive(false);
            originalHidden = true;
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
        RemoveNullBats();

        bool isNight = IsNight();

        if (showDebugLogs)
        {
            float hour;
            bool hasHour = TryGetTimeOfDay(out hour);
            Debug.Log("BatSpawner: hasHour=" + hasHour + ", hour=" + hour + ", isNight=" + isNight + ", count=" + spawnedBats.Count);
        }

        if (!spawnOnlyAtNight)
        {
            if (spawnedBats.Count == 0)
            {
                SpawnBats();
            }

            return;
        }

        if (isNight && spawnedBats.Count == 0)
        {
            SpawnBats();
        }

        if (!isNight && spawnedBats.Count > 0 && removeDuringDay)
        {
            RemoveSpawnedBats();
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

    private void SpawnBats()
    {
        RemoveNullBats();

        if (spawnedBats.Count > 0)
        {
            return;
        }

        for (int i = 0; i < numberOfBats; i++)
        {
            Vector3 spawnPosition = GetRandomPointInArea();

            GameObject bat = Instantiate(
                batTemplateFromScene,
                spawnPosition,
                Quaternion.Euler(0f, Random.Range(0f, 360f), 0f)
            );

            bat.SetActive(true);
            bat.name = batTemplateFromScene.name + "_Spawned_" + (i + 1);

            BatRandomFlyer flyer = bat.GetComponent<BatRandomFlyer>();

            if (flyer == null)
            {
                flyer = bat.AddComponent<BatRandomFlyer>();
            }

            flyer.areaCenter = areaCenter;
            flyer.areaSize = areaSize;
            flyer.minAltitude = minAltitude;
            flyer.maxAltitude = maxAltitude;
            flyer.speed = Random.Range(minSpeed, maxSpeed);
            flyer.turnSpeed = turnSpeed;
            flyer.targetReachDistance = targetReachDistance;
            flyer.minWaitTime = minWaitTime;
            flyer.maxWaitTime = maxWaitTime;

            spawnedBats.Add(bat);
        }

        Debug.Log("BatSpawner: spawned " + numberOfBats);
    }

    private void RemoveSpawnedBats()
    {
        for (int i = spawnedBats.Count - 1; i >= 0; i--)
        {
            if (spawnedBats[i] != null)
            {
                Destroy(spawnedBats[i]);
            }
        }

        spawnedBats.Clear();
        Debug.Log("BatSpawner: removed bats");
    }

    private void RemoveNullBats()
    {
        for (int i = spawnedBats.Count - 1; i >= 0; i--)
        {
            if (spawnedBats[i] == null)
            {
                spawnedBats.RemoveAt(i);
            }
        }
    }

    private Vector3 GetRandomPointInArea()
    {
        float x = Random.Range(-areaSize.x / 2f, areaSize.x / 2f);
        float z = Random.Range(-areaSize.y / 2f, areaSize.y / 2f);
        float y = Random.Range(minAltitude, maxAltitude);

        return new Vector3(areaCenter.x + x, areaCenter.y + y, areaCenter.z + z);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 center;

        if (Application.isPlaying)
        {
            center = areaCenter;
        }
        else if (batTemplateFromScene != null)
        {
            center = batTemplateFromScene.transform.position;
        }
        else
        {
            center = transform.position;
        }

        Gizmos.color = Color.cyan;
        Vector3 boxCenter = center + Vector3.up * ((minAltitude + maxAltitude) / 2f);
        Vector3 boxSize = new Vector3(areaSize.x, maxAltitude - minAltitude, areaSize.y);
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }
}