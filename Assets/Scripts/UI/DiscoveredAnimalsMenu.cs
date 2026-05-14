
using UnityEngine;
using UnityEngine.SceneManagement;
public class DiscoveredAnimalsMenu : MonoBehaviour
{
    public GameObject discoveredAnimalsUI;

    public void CloseDiscoveredAnimalsUI()
    {
         Debug.Log("Acum inchid UI-ul pentru discovered animals");
        discoveredAnimalsUI.SetActive(false);
    }
}