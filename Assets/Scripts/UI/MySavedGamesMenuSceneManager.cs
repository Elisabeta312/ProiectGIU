using UnityEngine;

using UnityEngine.SceneManagement;
public class MySavedGamesMenuSceneManager : MonoBehaviour
{
      public void Save1NextScene()
    {
        
        SceneManager.LoadScene("MainGameScene");
    }

    public void Save2NextScene()
    {
        SceneManager.LoadScene("MainGameScene");
    }

    public void Save3NextScene()
    {
        SceneManager.LoadScene("MainGameScene");
    }

    public void MainMenuNextScene() {
    SceneManager.LoadScene("UiMenu");
}
}
