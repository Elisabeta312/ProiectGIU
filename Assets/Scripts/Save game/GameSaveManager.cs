using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSaveManager : MonoBehaviour
{
    public static GameSaveManager Instance;

    private static SaveData pendingSaveDraft;

    [Header("Player")]
    public GameObject player;

    [Header("Scenes")]
    public string savesMenuSceneName = "SavesMenu";
    public string mainMenuSceneName = "UIMenu";

    private SaveData pendingLoadData;

    public static bool HasPendingSaveDraft()
    {
        return pendingSaveDraft != null;
    }

    public static SaveData GetPendingSaveDraft()
    {
        return pendingSaveDraft;
    }

    public static void ClearPendingSaveDraft()
    {
        pendingSaveDraft = null;
    }

    public static void ClearAllPendingData()
    {
        pendingSaveDraft = null;

        if (Instance != null)
        {
            Instance.pendingLoadData = null;
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        FindPlayerIfNeeded();
        DiscoveredAnimalsRegistry.ApplyDiscoveryStateToSceneAnimals();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void BeginSaveAndExit()
    {
        FindPlayerIfNeeded();

        if (player == null)
        {
            Debug.Log("Cannot save. Player was not found.");
            return;
        }

        pendingSaveDraft = CaptureCurrentGameData("New Save");

        Debug.Log("Save draft created. Going to SavesMenu.");

        SceneManager.LoadScene(savesMenuSceneName);
    }

    public void CommitPendingSaveToSlot(int slotIndex, string saveName)
    {
        if (pendingSaveDraft == null)
        {
            Debug.Log("No pending save draft.");
            return;
        }

        if (slotIndex < 1 || slotIndex > SaveSystem.MaxSlots)
        {
            Debug.Log("Invalid save slot: " + slotIndex);
            return;
        }

        if (string.IsNullOrWhiteSpace(saveName))
        {
            saveName = "Save " + slotIndex;
        }

        pendingSaveDraft.slotIndex = slotIndex;
        pendingSaveDraft.saveName = saveName;
        pendingSaveDraft.savedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        Debug.Log("Committing save. Slot: " + slotIndex + ", Name: " + saveName);

        SaveSystem.SaveGame(pendingSaveDraft);

        pendingSaveDraft = null;

        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void SaveCurrentGame(int slotIndex, string saveName)
    {
        FindPlayerIfNeeded();

        if (player == null)
        {
            Debug.Log("Cannot save. Player was not found.");
            return;
        }

        SaveData data = CaptureCurrentGameData(saveName);

        data.slotIndex = slotIndex;

        if (string.IsNullOrWhiteSpace(data.saveName))
        {
            data.saveName = "Save " + slotIndex;
        }

        SaveSystem.SaveGame(data);
    }

    public void LoadGameFromSlot(int slotIndex)
    {
        GameSessionState.MarkLoadGame();

        SaveData data = SaveSystem.LoadGame(slotIndex);

        if (data == null)
        {
            Debug.Log("Slot " + slotIndex + " is empty.");
            return;
        }

        pendingSaveDraft = null;
        pendingLoadData = data;

        if (SceneManager.GetActiveScene().name != data.sceneName)
        {
            SceneManager.LoadScene(data.sceneName);
        }
        else
        {
            StartCoroutine(ApplyLoadedDataNextFrame());
        }
    }

    private SaveData CaptureCurrentGameData(string saveName)
    {
        SaveData data = new SaveData();

        data.saveName = saveName;
        data.slotIndex = 0;
        data.sceneName = SceneManager.GetActiveScene().name;
        data.savedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        Vector3 pos = player.transform.position;
        Vector3 rot = player.transform.eulerAngles;

        data.playerPosX = pos.x;
        data.playerPosY = pos.y;
        data.playerPosZ = pos.z;

        data.playerRotX = rot.x;
        data.playerRotY = rot.y;
        data.playerRotZ = rot.z;

        data.discoveredAnimals = DiscoveredAnimalsRegistry.GetDiscoveredAnimals();

        data.acquiredKeys = PlayerKeyInventory.GetAcquiredKeys();
        data.openedCaveEntrances = CaveEntranceStateRegistry.GetOpenedEntrances();

        if (QuestFeedbackUI.Instance != null)
        {
            data.allAnimalsScreenAlreadyShown = QuestFeedbackUI.Instance.GetAllAnimalsScreenAlreadyShown();
        }

        return data;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindPlayerIfNeeded();

        if (pendingLoadData != null)
        {
            StartCoroutine(ApplyLoadedDataNextFrame());
        }
        else
        {
            DiscoveredAnimalsRegistry.ApplyDiscoveryStateToSceneAnimals();
        }
    }

    private IEnumerator ApplyLoadedDataNextFrame()
    {
        yield return null;

        FindPlayerIfNeeded();

        if (player == null)
        {
            Debug.Log("Could not load. Player was not found.");
            pendingLoadData = null;
            yield break;
        }

        player.transform.position = new Vector3(
            pendingLoadData.playerPosX,
            pendingLoadData.playerPosY,
            pendingLoadData.playerPosZ
        );

        player.transform.eulerAngles = new Vector3(
            pendingLoadData.playerRotX,
            pendingLoadData.playerRotY,
            pendingLoadData.playerRotZ
        );

        DiscoveredAnimalsRegistry.SetDiscoveredAnimals(pendingLoadData.discoveredAnimals);

        PlayerKeyInventory.SetAcquiredKeys(pendingLoadData.acquiredKeys);
        CaveEntranceStateRegistry.SetOpenedEntrances(pendingLoadData.openedCaveEntrances);

        if (QuestFeedbackUI.Instance != null)
            {
                QuestFeedbackUI.Instance.SetAllAnimalsScreenAlreadyShown(pendingLoadData.allAnimalsScreenAlreadyShown);
            }

        Debug.Log("Loaded save: " + pendingLoadData.saveName);

        pendingLoadData = null;
    }

    private void FindPlayerIfNeeded()
    {
        if (player != null)
        {
            return;
        }

        GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");

        if (foundPlayer != null)
        {
            player = foundPlayer;
        }
    }
}