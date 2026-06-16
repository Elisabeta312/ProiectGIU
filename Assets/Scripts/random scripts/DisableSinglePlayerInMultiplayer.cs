using UnityEngine;

public class DisableSinglePlayerInMultiplayer : MonoBehaviour
{
    private void Awake()
    {
        if (MultiplayerSession.IsMultiplayer)
            gameObject.SetActive(false);
    }
}