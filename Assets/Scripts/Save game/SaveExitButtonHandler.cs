using UnityEngine;

public class SaveExitButtonHandler : MonoBehaviour
{
    public void SaveAndExit()
    {
        if (GameSaveManager.Instance == null)
        {
            GameObject saveManagerObject = new GameObject("SaveManager");
            saveManagerObject.AddComponent<GameSaveManager>();
        }

        GameSaveManager.Instance.BeginSaveAndExit();
    }
}