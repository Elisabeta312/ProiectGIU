using UnityEngine;
using UnityEngine.SceneManagement;
public class NewGameButton : MonoBehaviour
{
      public void NextScene()
    {
        Debug.Log("New Game apasat");
        SceneManager.LoadScene("MainGameScene");
    }
}
