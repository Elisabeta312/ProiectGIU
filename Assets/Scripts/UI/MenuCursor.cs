using UnityEngine;


public class MenuCursor : MonoBehaviour
{
    void Start()
    
    {
        Debug.Log("MenuCursor ruleaza");

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}

