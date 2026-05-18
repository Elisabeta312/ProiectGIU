using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Scenes")]
    public string newGameSceneName = "MainGameScene";
    public string savesMenuSceneName = "SavesMenu";

    public void NewGame()
    {
        GameSaveManager.ClearAllPendingData();
        DiscoveredAnimalsRegistry.ResetSave();
        GameSessionState.MarkNewGame();

        SceneManager.LoadScene(newGameSceneName);
    }

    public void OpenSavesMenu()
    {
        GameSaveManager.ClearPendingSaveDraft();
        GameSessionState.MarkLoadGame();

        SceneManager.LoadScene(savesMenuSceneName);
    }

    public void PlayWithFriend()
    {
        Debug.Log("Play with a friend pressed.");
    }

    public void ExitGame()
    {
        Debug.Log("Exit game.");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}