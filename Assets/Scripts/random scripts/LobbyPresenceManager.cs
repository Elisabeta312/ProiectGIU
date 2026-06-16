using Normal.Realtime;
using UnityEngine;

public class LobbyPresenceManager : MonoBehaviour
{
    public static LobbyPresenceManager Instance { get; private set; }

    [Header("Normcore")]
    [SerializeField] private Realtime realtime;

    [Header("Lobby")]
    [SerializeField] private string lobbyRoomName = "giu-lobby";

    private GameObject currentPresenceObject;
    private string pendingRoomName = "";
    private string currentRoomName = "";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (realtime == null)
            realtime = GetComponent<Realtime>();
    }

    private void Start()
    {
        if (realtime == null)
        {
            Debug.LogError("LobbyPresenceManager: Realtime missing.");
            return;
        }

        realtime.didConnectToRoom += OnConnectedToLobby;

        if (!realtime.connected)
        {
            Debug.Log("Connecting to lobby room: " + lobbyRoomName);
            realtime.Connect(lobbyRoomName);
        }
    }

    private void OnConnectedToLobby(Realtime connectedRealtime)
    {
        Debug.Log("Connected to lobby room: " + lobbyRoomName);

        if (!string.IsNullOrWhiteSpace(pendingRoomName))
        {
            string roomToEnter = pendingRoomName;
            pendingRoomName = "";
            EnterGameplayRoom(roomToEnter);
        }
    }

    public void EnterGameplayRoom(string roomName)
    {
        if (string.IsNullOrWhiteSpace(roomName))
            return;

        if (realtime == null)
        {
            Debug.LogError("LobbyPresenceManager: Realtime missing.");
            return;
        }

        if (!realtime.connected)
        {
            Debug.LogWarning("Lobby not connected yet. Queuing presence for: " + roomName);
            pendingRoomName = roomName;
            return;
        }

        if (currentPresenceObject != null)
        {
            Realtime.Destroy(currentPresenceObject);
            currentPresenceObject = null;
        }

        string prefabName = GetPresencePrefabName(roomName);

        currentPresenceObject = Realtime.Instantiate(
            prefabName,
            Vector3.zero,
            Quaternion.identity,
            true,
            true,
            true,
            realtime
        );

        if (currentPresenceObject != null)
            DontDestroyOnLoad(currentPresenceObject);

        currentRoomName = roomName;

        Debug.Log("Presence created for gameplay room: " + roomName);
    }

    public void LeaveGameplayRoom()
    {
        if (currentPresenceObject != null)
        {
            Realtime.Destroy(currentPresenceObject);
            currentPresenceObject = null;
        }

        currentRoomName = "";
        pendingRoomName = "";

        Debug.Log("Presence removed.");
    }

    public int GetRoomCount(string roomName)
    {
        RoomPresenceMarker[] markers = FindObjectsByType<RoomPresenceMarker>(FindObjectsSortMode.None);

        int count = 0;

        foreach (RoomPresenceMarker marker in markers)
        {
            if (marker != null && marker.roomName == roomName)
                count++;
        }

        return count;
    }

    private string GetPresencePrefabName(string roomName)
    {
        if (roomName == "room1")
            return "PresenceRoom1";

        if (roomName == "room2")
            return "PresenceRoom2";

        if (roomName == "room3")
            return "PresenceRoom3";

        return "PresenceRoom1";
    }

    private void OnDestroy()
    {
        if (realtime != null)
            realtime.didConnectToRoom -= OnConnectedToLobby;
    }

    private void OnApplicationQuit()
    {
        LeaveGameplayRoom();
    }
}