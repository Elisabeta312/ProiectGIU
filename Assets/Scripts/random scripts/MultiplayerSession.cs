using UnityEngine;

public static class MultiplayerSession

{
    public static bool IsMultiplayer { get; private set; } = false;
    public static string RoomName { get; private set; } = "";

    public static void StartSingleplayer()
    {
        IsMultiplayer = false;
        RoomName = "";
    }

    public static void StartMultiplayer(string roomName)
    {
        IsMultiplayer = true;
        RoomName = roomName;
    }
}