using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MultiplayerRoomMenu : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string gameSceneName = "MainGameScene";
    [SerializeField] private string mainMenuSceneName = "UIMenu";

    [Header("Room Buttons")]
    [SerializeField] private Button room1Button;
    [SerializeField] private Button room2Button;
    [SerializeField] private Button room3Button;
    [SerializeField] private Button backButton;

    [Header("Room Texts")]
    [SerializeField] private TMP_Text room1Text;
    [SerializeField] private TMP_Text room2Text;
    [SerializeField] private TMP_Text room3Text;

    private void Awake()
    {
        AutoFindReferences();
        SetupButtons();
        UpdateRoomTexts();
    }

    private void Update()
    {
        UpdateRoomTexts();
    }

    private void AutoFindReferences()
    {
        if (room1Button == null)
            room1Button = FindButton("Save 1 Button");

        if (room2Button == null)
            room2Button = FindButton("Save 2 Button");

        if (room3Button == null)
            room3Button = FindButton("Save 3 Button");

        if (backButton == null)
            backButton = FindButton("ExitButton");

        if (room1Text == null && room1Button != null)
            room1Text = room1Button.GetComponentInChildren<TMP_Text>(true);

        if (room2Text == null && room2Button != null)
            room2Text = room2Button.GetComponentInChildren<TMP_Text>(true);

        if (room3Text == null && room3Button != null)
            room3Text = room3Button.GetComponentInChildren<TMP_Text>(true);
    }

    private Button FindButton(string objectName)
    {
        GameObject obj = GameObject.Find(objectName);

        if (obj == null)
        {
            Debug.LogWarning("Button object not found: " + objectName);
            return null;
        }

        Button button = obj.GetComponent<Button>();

        if (button == null)
            Debug.LogWarning("Object exists but has no Button component: " + objectName);

        return button;
    }

    private void SetupButtons()
    {
        if (room1Button != null)
        {
            room1Button.onClick.RemoveAllListeners();
            room1Button.onClick.AddListener(JoinRoom1);
        }

        if (room2Button != null)
        {
            room2Button.onClick.RemoveAllListeners();
            room2Button.onClick.AddListener(JoinRoom2);
        }

        if (room3Button != null)
        {
            room3Button.onClick.RemoveAllListeners();
            room3Button.onClick.AddListener(JoinRoom3);
        }

        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(BackToMainMenu);
        }
    }

    private void UpdateRoomTexts()
    {
        int room1Count = 0;
        int room2Count = 0;
        int room3Count = 0;

        if (LobbyPresenceManager.Instance != null)
        {
            room1Count = LobbyPresenceManager.Instance.GetRoomCount("room1");
            room2Count = LobbyPresenceManager.Instance.GetRoomCount("room2");
            room3Count = LobbyPresenceManager.Instance.GetRoomCount("room3");
        }

        if (room1Text != null)
            room1Text.text = "Room1: " + room1Count + " players";

        if (room2Text != null)
            room2Text.text = "Room2: " + room2Count + " players";

        if (room3Text != null)
            room3Text.text = "Room3: " + room3Count + " players";
    }

    public void JoinRoom1()
    {
        JoinRoom("room1");
    }

    public void JoinRoom2()
    {
        JoinRoom("room2");
    }

    public void JoinRoom3()
    {
        JoinRoom("room3");
    }

    private void JoinRoom(string roomName)
    {
        Debug.Log("Selected multiplayer room: " + roomName);

        MultiplayerSession.StartMultiplayer(roomName);

        SceneManager.LoadScene(gameSceneName);
    }

    public void BackToMainMenu()
    {
        MultiplayerSession.StartSingleplayer();
        SceneManager.LoadScene(mainMenuSceneName);
    }
}