using UnityEngine;

public class ElkSpawner : MonoBehaviour
{
    public GameObject elkTemplateFromScene;
    public bool hideOriginalElk = true;

    public Terrain groundTerrain;
    public float groundOffset = 0.05f;

    public Vector2 areaSize = new Vector2(80f, 80f);

    public float minActiveTime = 2f;
    public float maxActiveTime = 10f;
    public float minHiddenTime = 8f;
    public float maxHiddenTime = 20f;
    public bool spawnOnStart = true;

    public float minSpeed = 3f;
    public float maxSpeed = 6f;
    public float turnSpeed = 6f;
    public float targetReachDistance = 1f;
    public float minWaitTime = 0.2f;
    public float maxWaitTime = 1.5f;

    public bool disableOtherMovementScripts = true;
    public bool showDebugLogs = true;

    private GameObject currentElk;
    private float timer;
    private bool isElkActive;
    private Vector3 areaCenter;
    private bool originalHidden;

    private void Start()
    {
        areaCenter = transform.position;

        if (elkTemplateFromScene == null)
        {
            Debug.LogError("ElkSpawner: lipseste Elk Template From Scene.");
            enabled = false;
            return;
        }

        if (groundTerrain == null)
        {
            Debug.LogError("ElkSpawner: lipseste Ground Terrain.");
            enabled = false;
            return;
        }

        if (hideOriginalElk && !originalHidden)
        {
            elkTemplateFromScene.SetActive(false);
            originalHidden = true;
        }

        if (spawnOnStart)
        {
            SpawnElk();
        }
        else
        {
            StartHiddenTimer();
        }
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer > 0f)
        {
            return;
        }

        if (isElkActive)
        {
            RemoveElk();
            StartHiddenTimer();
        }
        else
        {
            SpawnElk();
        }
    }

    private void SpawnElk()
    {
        Vector3 spawnPosition = GetRandomPointOnGround();

        currentElk = Instantiate(
            elkTemplateFromScene,
            spawnPosition,
            Quaternion.Euler(0f, Random.Range(0f, 360f), 0f)
        );

        currentElk.SetActive(true);
        currentElk.name = elkTemplateFromScene.name + "_Spawned";

        if (disableOtherMovementScripts)
        {
            DisableConflictingScripts(currentElk);
        }

        ElkRandomRunner runner = currentElk.GetComponent<ElkRandomRunner>();

        if (runner == null)
        {
            runner = currentElk.AddComponent<ElkRandomRunner>();
        }

        runner.areaCenter = areaCenter;
        runner.areaSize = areaSize;
        runner.groundTerrain = groundTerrain;
        runner.groundOffset = groundOffset;
        runner.speed = Random.Range(minSpeed, maxSpeed);
        runner.turnSpeed = turnSpeed;
        runner.targetReachDistance = targetReachDistance;
        runner.minWaitTime = minWaitTime;
        runner.maxWaitTime = maxWaitTime;

        isElkActive = true;
        timer = Random.Range(minActiveTime, maxActiveTime);

        if (showDebugLogs)
        {
            Debug.Log("ElkSpawner: spawned at " + spawnPosition + " for " + timer + " seconds.");
        }
    }

    private void RemoveElk()
    {
        if (currentElk != null)
        {
            Destroy(currentElk);
        }

        currentElk = null;
        isElkActive = false;

        if (showDebugLogs)
        {
            Debug.Log("ElkSpawner: removed elk.");
        }
    }

    private void StartHiddenTimer()
    {
        timer = Random.Range(minHiddenTime, maxHiddenTime);

        if (showDebugLogs)
        {
            Debug.Log("ElkSpawner: hidden for " + timer + " seconds.");
        }
    }

    private void DisableConflictingScripts(GameObject elk)
    {
        MonoBehaviour[] scripts = elk.GetComponentsInChildren<MonoBehaviour>(true);

        for (int i = 0; i < scripts.Length; i++)
        {
            if (scripts[i] == null)
            {
                continue;
            }

            string scriptName = scripts[i].GetType().Name;

            if (scriptName == "CreatureMover" || scriptName == "MovePlayerInput")
            {
                scripts[i].enabled = false;
            }
        }
    }

    private Vector3 GetRandomPointOnGround()
    {
        float randomX = Random.Range(-areaSize.x / 2f, areaSize.x / 2f);
        float randomZ = Random.Range(-areaSize.y / 2f, areaSize.y / 2f);

        Vector3 position = new Vector3(
            areaCenter.x + randomX,
            areaCenter.y,
            areaCenter.z + randomZ
        );

        return GetGroundPosition(position);
    }

    private Vector3 GetGroundPosition(Vector3 position)
    {
        float y = groundTerrain.transform.position.y + groundTerrain.SampleHeight(position);
        return new Vector3(position.x, y + groundOffset, position.z);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Vector3 center = Application.isPlaying ? areaCenter : transform.position;
        Vector3 size = new Vector3(areaSize.x, 2f, areaSize.y);

        Gizmos.DrawWireCube(center + Vector3.up, size);
    }
}