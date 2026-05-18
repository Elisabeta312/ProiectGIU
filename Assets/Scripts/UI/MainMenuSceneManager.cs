using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenuSceneManager : MonoBehaviour
{
      public void NewGameNextScene()
    {
        
        SceneManager.LoadScene("MainGameScene");
    }

    public void SavedGamesNextScene()
    {
        SceneManager.LoadScene("SavesMenu");
    }

    public void doExitGame() {
    Application.Quit();
}
}
