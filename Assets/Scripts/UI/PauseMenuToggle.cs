using UnityEngine;

public class PauseMenuToggle : MonoBehaviour
{
    public GameObject PauseMenu;
    public GameObject DiscoveredAnimalsMenu;

    public MonoBehaviour playerMovement;
    public MonoBehaviour mouseLook;

    private bool isPaused = false;

    void Start()
    {
        DiscoveredAnimalsMenu.SetActive(false);
        SetPaused(false);
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (DiscoveredAnimalsMenu.activeInHierarchy)
            {
                DiscoveredAnimalsMenu.SetActive(false);
                return;
            }

            SetPaused(!isPaused);
        }
    }

    void SetPaused(bool paused)
    {
        isPaused = paused;

        PauseMenu.SetActive(paused);
        Time.timeScale = paused ? 0f : 1f;

        if (playerMovement != null)
            playerMovement.enabled = !paused;

        if (mouseLook != null)
            mouseLook.enabled = !paused;

        Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = paused;
    }
}
