using UnityEngine;
using UnityEngine.SceneManagement;
public class NewGameButton : MonoBehaviour
{
      public void NextScene()
    {
        
        SceneManager.LoadScene("MainGameScene");
    }
}
