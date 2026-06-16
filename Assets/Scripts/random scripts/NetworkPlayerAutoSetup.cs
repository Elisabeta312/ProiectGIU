using Normal.Realtime;
using UnityEngine;
using System.Reflection;

public class NetworkPlayerAutoSetup : MonoBehaviour
{
    [Header("Auto setup names")]
    [SerializeField] private string movementScriptName = "PT_PlayerMovement";
    [SerializeField] private string discoveryScriptName = "AnimalDiscoveryTrigger";
    [SerializeField] private string robotVisualNamePart = "RandomModularRobots";

    private RealtimeView realtimeView;

    private void Awake()
    {
        realtimeView = GetComponent<RealtimeView>();
    }

    private void Start()
    {
        bool isLocalPlayer = realtimeView == null || realtimeView.isOwnedLocallyInHierarchy;

        SetupLocalOnlyScripts(isLocalPlayer);
        SetupCamera(isLocalPlayer);
        SetupVisuals(isLocalPlayer);
        FixAnimalDiscoveryReferences(isLocalPlayer);
    }

    private void SetupLocalOnlyScripts(bool isLocalPlayer)
    {
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();

        foreach (MonoBehaviour script in scripts)
        {
            if (script == null)
                continue;

            string scriptName = script.GetType().Name;

            if (scriptName == movementScriptName || scriptName == discoveryScriptName)
            {
                script.enabled = isLocalPlayer;
                Debug.Log(scriptName + " enabled = " + isLocalPlayer);
            }
        }
    }

    private void SetupCamera(bool isLocalPlayer)
    {
        Camera[] cameras = GetComponentsInChildren<Camera>(true);
        AudioListener[] audioListeners = GetComponentsInChildren<AudioListener>(true);

        foreach (Camera cam in cameras)
        {
            if (cam != null)
                cam.enabled = isLocalPlayer;
        }

        foreach (AudioListener listener in audioListeners)
        {
            if (listener != null)
                listener.enabled = isLocalPlayer;
        }
    }

    private void SetupVisuals(bool isLocalPlayer)
    {
        if (!isLocalPlayer)
            return;

        Transform[] children = GetComponentsInChildren<Transform>(true);

        foreach (Transform child in children)
        {
            if (child == null)
                continue;

            if (child == transform)
                continue;

            if (child.name.Contains(robotVisualNamePart))
            {
                child.gameObject.SetActive(false);
                Debug.Log("Local robot visual hidden: " + child.name);
                return;
            }
        }

        Debug.LogWarning("Robot visual was not found. Name should contain: " + robotVisualNamePart);
    }

    private void FixAnimalDiscoveryReferences(bool isLocalPlayer)
    {
        if (!isLocalPlayer)
            return;

        MonoBehaviour discoveryScript = FindScriptOnThisObject(discoveryScriptName);

        if (discoveryScript == null)
        {
            Debug.LogWarning("AnimalDiscoveryTrigger not found on NetworkPlayer.");
            return;
        }

        AssignTransformField(discoveryScript, "detectionOrigin", transform);
        AssignJournalHighlighter(discoveryScript);

        Debug.Log("AnimalDiscoveryTrigger references fixed for multiplayer player.");
    }

    private MonoBehaviour FindScriptOnThisObject(string scriptName)
    {
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();

        foreach (MonoBehaviour script in scripts)
        {
            if (script != null && script.GetType().Name == scriptName)
                return script;
        }

        return null;
    }

    private void AssignTransformField(MonoBehaviour targetScript, string fieldName, Transform value)
    {
        FieldInfo field = targetScript.GetType().GetField(
            fieldName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
        );

        if (field == null)
            return;

        if (field.FieldType == typeof(Transform))
            field.SetValue(targetScript, value);
    }

    private void AssignJournalHighlighter(MonoBehaviour discoveryScript)
    {
        MonoBehaviour highlighter = FindMonoBehaviourByTypeName("JournalDiscoveryHighlighter");

        if (highlighter == null)
        {
            Debug.LogWarning("JournalDiscoveryHighlighter not found in scene.");
            return;
        }

        FieldInfo[] fields = discoveryScript.GetType().GetFields(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
        );

        foreach (FieldInfo field in fields)
        {
            if (field.FieldType.Name == "JournalDiscoveryHighlighter")
            {
                field.SetValue(discoveryScript, highlighter);
                Debug.Log("JournalDiscoveryHighlighter assigned.");
                return;
            }
        }

        Debug.LogWarning("No JournalDiscoveryHighlighter field found inside AnimalDiscoveryTrigger.");
    }

    private MonoBehaviour FindMonoBehaviourByTypeName(string typeName)
    {
        MonoBehaviour[] allScripts = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);

        foreach (MonoBehaviour script in allScripts)
        {
            if (script != null && script.GetType().Name == typeName)
                return script;
        }

        return null;
    }
}