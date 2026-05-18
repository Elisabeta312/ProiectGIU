using UnityEngine;

public class CursorStyle : MonoBehaviour
{
    public Sprite cursorSprite;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    private Texture2D cursorTexture;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        CreateTextureFromSprite();
        ApplyCursor();
    }

    void OnEnable()
    {
        ApplyCursor();
    }

    void Start()
    {
        ApplyCursor();
    }

    void LateUpdate()
    {
        if (Cursor.visible)
        {
            ApplyCursor();
        }
    }

    void CreateTextureFromSprite()
    {
        if (cursorSprite == null)
            return;

        Rect rect = cursorSprite.textureRect;

        cursorTexture = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGBA32, false);
        Color[] pixels = cursorSprite.texture.GetPixels(
            (int)rect.x,
            (int)rect.y,
            (int)rect.width,
            (int)rect.height
        );

        cursorTexture.SetPixels(pixels);
        cursorTexture.Apply();
    }

    public void ApplyCursor()
    {
        if (cursorTexture == null)
            return;

        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }
}