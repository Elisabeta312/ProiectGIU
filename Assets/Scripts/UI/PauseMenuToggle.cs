using UnityEngine;

public class PauseMenuToggle : MonoBehaviour
{
    [Header("Main Game Screen UI")]
    public GameObject PauseMenu;
    public GameObject DiscoveredAnimalsMenu;
    public GameObject TutorialScreen;

    [Header("Cave Related Screen UI")]
    public GameObject allAnimalsDiscoveredScreenUI;
    public GameObject keyScreenUI;
    public GameObject caveOpenScreenUI;

    public MonoBehaviour playerMovement;
    public MonoBehaviour mouseLook;

    private bool isPaused = false;

    void Start()
    {
        TutorialScreen.SetActive(true);
        DiscoveredAnimalsMenu.SetActive(false);
        SetPaused(false);
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
{
    if (TutorialScreen.activeInHierarchy)
    {
        TutorialScreen.SetActive(false);
        return;
    }

    if (DiscoveredAnimalsMenu.activeInHierarchy)
    {
        DiscoveredAnimalsMenu.SetActive(false);
        return;
    }

    if (allAnimalsDiscoveredScreenUI != null && allAnimalsDiscoveredScreenUI.activeInHierarchy)
    {
        allAnimalsDiscoveredScreenUI.SetActive(false);
        return;
    }

    if (keyScreenUI != null && keyScreenUI.activeInHierarchy)
    {
        keyScreenUI.SetActive(false);
        return;
    }

    if (caveOpenScreenUI != null && caveOpenScreenUI.activeInHierarchy)
    {
        caveOpenScreenUI.SetActive(false);
        return;
    }

    SetPaused(!isPaused);
}

        if (Input.GetKeyUp(KeyCode.T))
        {
            if (!TutorialScreen.activeInHierarchy)
            {
                TutorialScreen.SetActive(true);
                
                return;
            }
            else {
                TutorialScreen.SetActive(false);
                
                return;
            }

            
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
