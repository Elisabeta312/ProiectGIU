using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuSceneManager : MonoBehaviour
{
    public void NewGameNextScene()
    {
        MultiplayerSession.StartSingleplayer();
        SceneManager.LoadScene("MainGameScene");
    }

    public void SavedGamesNextScene()
    {
        MultiplayerSession.StartSingleplayer();
        SceneManager.LoadScene("SavesMenu");
    }

    public void OpenMultiplayerMenu()
    {
        MultiplayerSession.StartSingleplayer();
        SceneManager.LoadScene("multyui");
    }

    public void doExitGame()
    {
        Application.Quit();
    }
}