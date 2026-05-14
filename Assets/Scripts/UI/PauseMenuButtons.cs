using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenuButtons : MonoBehaviour
{
    public GameObject discoveredAnimalsUI;

    public void openDiscoveredAnimalsUI()
    {
        discoveredAnimalsUI.SetActive(true);
    }
}
