using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveMenuController : MonoBehaviour
{
    [Header("Title")]
    public TMP_Text titleText;

    [Header("Slot Buttons")]
    public Button slot1Button;
    public Button slot2Button;
    public Button slot3Button;

    [Header("Slot Texts")]
    public TMP_Text slot1Text;
    public TMP_Text slot2Text;
    public TMP_Text slot3Text;

    [Header("Other Buttons")]
    public Button backButton;

    [Header("Scenes")]
    public string mainMenuSceneName = "UIMenu";

    [Header("Mode")]
    public bool forceSaveMode = false;

    private int selectedSlot = 0;
    private bool saveMode = false;

    private TMP_InputField activeInputField;
    private GameObject activeInputObject;

    private const string SaveNamePlaceholder = "type here the name of this save";
    private const int MaxSaveNameLength = 24;

    private void Start()
    {
        saveMode = forceSaveMode || GameSaveManager.HasPendingSaveDraft();

        if (slot1Button != null) slot1Button.onClick.AddListener(() => OnSlotPressed(1));
        if (slot2Button != null) slot2Button.onClick.AddListener(() => OnSlotPressed(2));
        if (slot3Button != null) slot3Button.onClick.AddListener(() => OnSlotPressed(3));

        if (backButton != null) backButton.onClick.AddListener(GoBack);

        SetupTitle();
        RefreshSlots();
    }

    private void SetupTitle()
    {
        if (titleText == null)
        {
            return;
        }

        titleText.text = saveMode ? "Choose Save Slot" : "My Saved Games";
    }

    private void OnSlotPressed(int slotIndex)
    {
        selectedSlot = slotIndex;

        if (saveMode)
        {
            OpenInputOnSlot(slotIndex);
        }
        else
        {
            LoadSlot(slotIndex);
        }
    }

    private void OpenInputOnSlot(int slotIndex)
    {
        DestroyActiveInput();

        TMP_Text slotText = GetSlotText(slotIndex);
        Button slotButton = GetSlotButton(slotIndex);

        if (slotText == null || slotButton == null)
        {
            return;
        }

        RefreshSlots();

        slotText.gameObject.SetActive(false);

        activeInputObject = new GameObject("RuntimeSaveNameInput");
        activeInputObject.transform.SetParent(slotButton.transform, false);

        RectTransform inputRect = activeInputObject.AddComponent<RectTransform>();
        inputRect.anchorMin = Vector2.zero;
        inputRect.anchorMax = Vector2.one;
        inputRect.offsetMin = Vector2.zero;
        inputRect.offsetMax = Vector2.zero;
        inputRect.localScale = Vector3.one;

        Image inputImage = activeInputObject.AddComponent<Image>();
        inputImage.color = new Color(0f, 0f, 0f, 0f);
        inputImage.raycastTarget = true;

        activeInputField = activeInputObject.AddComponent<TMP_InputField>();
        activeInputField.characterLimit = MaxSaveNameLength;
        activeInputField.lineType = TMP_InputField.LineType.SingleLine;
        activeInputField.targetGraphic = inputImage;

        GameObject textObject = new GameObject("Text");
        textObject.transform.SetParent(activeInputObject.transform, false);

        RectTransform textRect = textObject.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        textRect.localScale = Vector3.one;

        TMP_Text inputText = textObject.AddComponent<TextMeshProUGUI>();
        inputText.text = "";
        inputText.fontSize = slotText.fontSize;
        inputText.alignment = TextAlignmentOptions.Center;
        inputText.color = slotText.color;
        inputText.raycastTarget = false;

        GameObject placeholderObject = new GameObject("Placeholder");
        placeholderObject.transform.SetParent(activeInputObject.transform, false);

        RectTransform placeholderRect = placeholderObject.AddComponent<RectTransform>();
        placeholderRect.anchorMin = Vector2.zero;
        placeholderRect.anchorMax = Vector2.one;
        placeholderRect.offsetMin = Vector2.zero;
        placeholderRect.offsetMax = Vector2.zero;
        placeholderRect.localScale = Vector3.one;

        TMP_Text placeholderText = placeholderObject.AddComponent<TextMeshProUGUI>();
        placeholderText.text = SaveNamePlaceholder;
        placeholderText.fontSize = slotText.fontSize * 0.7f;
        placeholderText.alignment = TextAlignmentOptions.Center;
        placeholderText.color = new Color(0.6f, 0.6f, 0.6f, 1f);
        placeholderText.raycastTarget = false;

        activeInputField.textComponent = inputText;
        activeInputField.placeholder = placeholderText;
        activeInputField.text = "";

        activeInputField.onSubmit.AddListener(SubmitTypedSaveName);
        activeInputField.onEndEdit.AddListener(SubmitTypedSaveName);

        activeInputField.Select();
        activeInputField.ActivateInputField();

        Debug.Log("Input opened for save slot " + slotIndex);
    }

    private void SubmitTypedSaveName(string typedName)
    {
        if (!saveMode)
        {
            return;
        }

        if (selectedSlot < 1 || selectedSlot > SaveSystem.MaxSlots)
        {
            DestroyActiveInput();
            RefreshSlots();
            return;
        }

        if (!GameSaveManager.HasPendingSaveDraft())
        {
            DestroyActiveInput();
            RefreshSlots();
            return;
        }

        EnsureSaveManagerExists();

        string saveName = typedName;

        if (string.IsNullOrWhiteSpace(saveName))
        {
            saveName = "Save " + selectedSlot;
        }

        saveName = saveName.Trim();

        Debug.Log("Saving slot " + selectedSlot + " with name: " + saveName);

        GameSaveManager.Instance.CommitPendingSaveToSlot(selectedSlot, saveName);

        DestroyActiveInput();
    }

    private void LoadSlot(int slotIndex)
    {
        SaveData data = SaveSystem.LoadGame(slotIndex);

        if (data == null)
        {
            Debug.Log("Slot " + slotIndex + " is empty.");
            return;
        }

        EnsureSaveManagerExists();

        GameSaveManager.Instance.LoadGameFromSlot(slotIndex);
    }

    private void GoBack()
    {
        GameSaveManager.ClearPendingSaveDraft();
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void RefreshSlots()
    {
        UpdateSlotText(1, slot1Text);
        UpdateSlotText(2, slot2Text);
        UpdateSlotText(3, slot3Text);
    }

    private void UpdateSlotText(int slotIndex, TMP_Text slotText)
    {
        if (slotText == null)
        {
            return;
        }

        slotText.gameObject.SetActive(true);

        SaveData data = SaveSystem.LoadGame(slotIndex);

        if (data == null || string.IsNullOrWhiteSpace(data.saveName))
        {
            slotText.text = "Save " + slotIndex;
            return;
        }

        slotText.text = data.saveName;
    }

    private TMP_Text GetSlotText(int slotIndex)
    {
        if (slotIndex == 1) return slot1Text;
        if (slotIndex == 2) return slot2Text;
        if (slotIndex == 3) return slot3Text;

        return null;
    }

    private Button GetSlotButton(int slotIndex)
    {
        if (slotIndex == 1) return slot1Button;
        if (slotIndex == 2) return slot2Button;
        if (slotIndex == 3) return slot3Button;

        return null;
    }

    private void DestroyActiveInput()
    {
        if (activeInputField != null)
        {
            activeInputField.onSubmit.RemoveAllListeners();
            activeInputField.onEndEdit.RemoveAllListeners();
            activeInputField = null;
        }

        if (activeInputObject != null)
        {
            Destroy(activeInputObject);
            activeInputObject = null;
        }

        if (slot1Text != null) slot1Text.gameObject.SetActive(true);
        if (slot2Text != null) slot2Text.gameObject.SetActive(true);
        if (slot3Text != null) slot3Text.gameObject.SetActive(true);
    }

    private void EnsureSaveManagerExists()
    {
        if (GameSaveManager.Instance != null)
        {
            return;
        }

        GameObject saveManagerObject = new GameObject("SaveManager");
        saveManagerObject.AddComponent<GameSaveManager>();
    }
}