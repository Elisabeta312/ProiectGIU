using Normal.Realtime;
using UnityEngine;

public class NormcoreGameConnector : MonoBehaviour
{
    [Header("Normcore")]
    [SerializeField] private Realtime realtime;

    [Header("Player")]
    [SerializeField] private string playerPrefabName = "NetworkPlayer";

    [Header("Spawn")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Vector3 fallbackSpawnPosition = new Vector3(28.65f, 5f, 70.45f);

    private GameObject localPlayer;
    private bool presenceRegistered = false;

    private void Awake()
    {
        if (realtime == null)
            realtime = GetComponent<Realtime>();

        if (realtime == null)
            realtime = FindFirstObjectByType<Realtime>();
    }

    private void Start()
    {
        Debug.Log("NormcoreGameConnector started.");

        if (!MultiplayerSession.IsMultiplayer)
        {
            Debug.Log("Singleplayer mode. Normcore disabled.");
            enabled = false;
            return;
        }

        if (realtime == null)
        {
            Debug.LogError("NormcoreGameConnector: Realtime component is missing.");
            return;
        }

        if (string.IsNullOrWhiteSpace(MultiplayerSession.RoomName))
        {
            Debug.LogError("NormcoreGameConnector: RoomName is empty.");
            return;
        }

        realtime.didConnectToRoom += OnConnectedToRoom;

        Debug.Log("Connecting to gameplay room: " + MultiplayerSession.RoomName);
        realtime.Connect(MultiplayerSession.RoomName);
    }

    private void OnConnectedToRoom(Realtime connectedRealtime)
    {
        SpawnLocalPlayer(connectedRealtime);
    }

    private void SpawnLocalPlayer(Realtime connectedRealtime)
    {
        if (localPlayer != null)
            return;

        Vector3 spawnPosition = fallbackSpawnPosition;
        Quaternion spawnRotation = Quaternion.identity;

        if (spawnPoints != null && spawnPoints.Length > 0 && spawnPoints[0] != null)
        {
            spawnPosition = spawnPoints[0].position;
            spawnRotation = spawnPoints[0].rotation;
        }

        localPlayer = Realtime.Instantiate(
            playerPrefabName,
            spawnPosition,
            spawnRotation,
            true,
            true,
            true,
            connectedRealtime
        );

        Debug.Log("Spawned NetworkPlayer in room: " + MultiplayerSession.RoomName);

        RegisterPresence();
    }

    private void RegisterPresence()
    {
        if (presenceRegistered)
            return;

        if (LobbyPresenceManager.Instance == null)
        {
            Debug.LogWarning("LobbyPresenceManager missing. Room count will not update.");
            return;
        }

        LobbyPresenceManager.Instance.EnterGameplayRoom(MultiplayerSession.RoomName);

        presenceRegistered = true;

        Debug.Log("Registered gameplay presence for: " + MultiplayerSession.RoomName);
    }

    private void OnDestroy()
    {
        if (realtime != null)
            realtime.didConnectToRoom -= OnConnectedToRoom;

        if (presenceRegistered && LobbyPresenceManager.Instance != null)
            LobbyPresenceManager.Instance.LeaveGameplayRoom();
    }
}