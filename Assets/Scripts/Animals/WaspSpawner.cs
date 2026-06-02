using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class WaspSpawner : MonoBehaviour
{
    [Header("Wasp source")]
    public GameObject waspTemplateFromScene;
    public bool hideOriginalWasp = true;

    [Header("Hive")]
    public Transform hivePoint;

    [Header("Spawn")]
    public int numberOfWasps = 10;
    public float spawnSpread = 0.3f;

    [Header("Swarm area")]
    public float swarmRadius = 5f;
    public float minHeight = 0.5f;
    public float maxHeight = 3f;

    [Header("Movement")]
    public float minSpeed = 2f;
    public float maxSpeed = 5f;
    public float turnSpeed = 8f;
    public float targetReachDistance = 0.35f;
    public float minWaitTime = 0.05f;
    public float maxWaitTime = 0.4f;

    [Header("Day time")]
    public MonoBehaviour lightingManager;
    public bool autoFindLightingManager = true;
    public string timeOfDayFieldName = "timeOfDay";
    public float dayStartsAt = 5f;
    public float dayEndsAt = 19f;
    public float checkEverySeconds = 1f;

    [Header("Debug")]
    public bool showDebugLogs = true;

    private readonly List<GameObject> spawnedWasps = new List<GameObject>();
    private float checkTimer;
    private bool originalHidden;
    private bool wasReturning;

    private void Start()
    {
        if (waspTemplateFromScene == null)
        {
            Debug.LogError("WaspSpawner: lipseste Wasp Template From Scene.");
            enabled = false;
            return;
        }

        if (hivePoint == null)
        {
            Debug.LogError("WaspSpawner: lipseste Hive Point.");
            enabled = false;
            return;
        }

        if (lightingManager == null && autoFindLightingManager)
        {
            lightingManager = FindLightingManager();
        }

        if (hideOriginalWasp && !originalHidden)
        {
            waspTemplateFromScene.SetActive(false);
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
        RemoveNullWasps();

        bool isDay = IsDay();

        if (showDebugLogs)
        {
            float hour;
            bool hasHour = TryGetTimeOfDay(out hour);

            Debug.Log(
                "WaspSpawner: hasHour=" + hasHour +
                ", hour=" + hour +
                ", isDay=" + isDay +
                ", wasps=" + spawnedWasps.Count
            );
        }

        if (isDay)
        {
            wasReturning = false;

            if (spawnedWasps.Count == 0)
            {
                SpawnWasps();
            }

            return;
        }

        if (spawnedWasps.Count > 0 && !wasReturning)
        {
            SendWaspsBackToHive();
        }
    }

    private bool IsDay()
    {
        float hour;

        if (!TryGetTimeOfDay(out hour))
        {
            Debug.LogError("WaspSpawner: Nu pot citi Time Of Day. Verifica Lighting Manager si Time Of Day Field Name.");
            return false;
        }

        if (dayStartsAt < dayEndsAt)
        {
            return hour >= dayStartsAt && hour < dayEndsAt;
        }

        return hour >= dayStartsAt || hour < dayEndsAt;
    }

    private void SpawnWasps()
    {
        RemoveNullWasps();

        if (spawnedWasps.Count > 0)
        {
            return;
        }

        for (int i = 0; i < numberOfWasps; i++)
        {
            Vector3 offset = Random.insideUnitSphere * spawnSpread;
            offset.y = Mathf.Abs(offset.y);

            GameObject wasp = Instantiate(
                waspTemplateFromScene,
                hivePoint.position + offset,
                Quaternion.Euler(0f, Random.Range(0f, 360f), 0f)
            );

            wasp.SetActive(true);
            wasp.name = waspTemplateFromScene.name + "_Spawned_" + (i + 1);

            WaspSwarmBehaviour behaviour = wasp.GetComponent<WaspSwarmBehaviour>();

            if (behaviour == null)
            {
                behaviour = wasp.AddComponent<WaspSwarmBehaviour>();
            }

            behaviour.hivePoint = hivePoint;
            behaviour.swarmRadius = swarmRadius;
            behaviour.minHeight = minHeight;
            behaviour.maxHeight = maxHeight;
            behaviour.speed = Random.Range(minSpeed, maxSpeed);
            behaviour.turnSpeed = turnSpeed;
            behaviour.targetReachDistance = targetReachDistance;
            behaviour.minWaitTime = minWaitTime;
            behaviour.maxWaitTime = maxWaitTime;
            behaviour.StartSwarming();

            spawnedWasps.Add(wasp);
        }

        Debug.Log("WaspSpawner: spawned " + numberOfWasps + " wasps.");
    }

    private void SendWaspsBackToHive()
    {
        wasReturning = true;

        for (int i = spawnedWasps.Count - 1; i >= 0; i--)
        {
            if (spawnedWasps[i] == null)
            {
                spawnedWasps.RemoveAt(i);
                continue;
            }

            WaspSwarmBehaviour behaviour = spawnedWasps[i].GetComponent<WaspSwarmBehaviour>();

            if (behaviour != null)
            {
                behaviour.ReturnToHiveAndDisappear();
            }
            else
            {
                Destroy(spawnedWasps[i]);
                spawnedWasps.RemoveAt(i);
            }
        }

        Debug.Log("WaspSpawner: wasps returning to hive.");
    }

    private void RemoveNullWasps()
    {
        for (int i = spawnedWasps.Count - 1; i >= 0; i--)
        {
            if (spawnedWasps[i] == null)
            {
                spawnedWasps.RemoveAt(i);
            }
        }
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
            string fieldName = fields[i].Name.ToLower();

            if ((fieldName.Contains("time") || fieldName.Contains("hour")) && fields[i].FieldType == typeof(float))
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

        FieldInfo field = type.GetField(
            memberName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
        );

        if (field != null)
        {
            value = field.GetValue(target);
            return true;
        }

        PropertyInfo property = type.GetProperty(
            memberName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
        );

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

    private void OnDrawGizmosSelected()
    {
        if (hivePoint == null)
        {
            return;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(hivePoint.position + Vector3.up * ((minHeight + maxHeight) / 2f), swarmRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(hivePoint.position, 0.35f);
    }
}