

using System.Collections;
using UnityEngine;

public class QuestFeedbackUI : MonoBehaviour
{
    public static QuestFeedbackUI Instance;

    [Header("Full Screen UI")]
    public GameObject allAnimalsDiscoveredScreenUI;
    public GameObject keyScreenUI;
    public GameObject caveOpenScreenUI;

    [Header("Cave Texts")]
    public GameObject keyRequiredText;
    public GameObject unlockInstructionsText;
    public GameObject entryOpenText;

    [Header("Timing")]
    public float temporaryTextDuration = 3f;

    private GameObject currentScreen;
    private Coroutine temporaryTextRoutine;
    private bool allAnimalsScreenAlreadyShown;

    private void Awake()
    {
        Instance = this;

        HideAllScreens();
        HideCaveTexts();
    }


    public void ShowAllAnimalsDiscoveredScreen()
{
    if (allAnimalsScreenAlreadyShown)
    {
        return;
    }

    allAnimalsScreenAlreadyShown = true;

    ShowScreen(allAnimalsDiscoveredScreenUI);
}

    public void ShowKeyScreen()
    {
        ShowScreen(keyScreenUI);
    }

    public void ShowCaveOpenScreen()
    {
        ShowScreen(caveOpenScreenUI);
    }

    public void ShowKeyRequiredText()
    {
        ShowTemporaryCaveText(keyRequiredText);
    }

    public void ShowUnlockInstructionsText()
    {
        HideCaveTexts();

        if (unlockInstructionsText != null)
        {
            unlockInstructionsText.SetActive(true);
        }
    }

    public void ShowEntryOpenText()
    {
        ShowTemporaryCaveText(entryOpenText);
    }

    public void HideUnlockInstructionsText()
    {
        if (unlockInstructionsText != null)
        {
            unlockInstructionsText.SetActive(false);
        }
    }

    private void ShowScreen(GameObject screen)
    {
        if (screen == null)
        {
            return;
        }

        if (currentScreen != null)
        {
            currentScreen.SetActive(false);
        }

        currentScreen = screen;
        currentScreen.SetActive(true);
    }

    private void ShowTemporaryCaveText(GameObject textObject)
    {
        HideCaveTexts();

        if (temporaryTextRoutine != null)
        {
            StopCoroutine(temporaryTextRoutine);
        }

        temporaryTextRoutine = StartCoroutine(ShowTemporaryTextRoutine(textObject));
    }

    private IEnumerator ShowTemporaryTextRoutine(GameObject textObject)
    {
        if (textObject != null)
        {
            textObject.SetActive(true);
        }

        yield return new WaitForSeconds(temporaryTextDuration);

        if (textObject != null)
        {
            textObject.SetActive(false);
        }

        temporaryTextRoutine = null;
    }

    private void HideAllScreens()
    {
        if (allAnimalsDiscoveredScreenUI != null) allAnimalsDiscoveredScreenUI.SetActive(false);
        if (keyScreenUI != null) keyScreenUI.SetActive(false);
        if (caveOpenScreenUI != null) caveOpenScreenUI.SetActive(false);
    }

    private void HideCaveTexts()
    {
        if (keyRequiredText != null) keyRequiredText.SetActive(false);
        if (unlockInstructionsText != null) unlockInstructionsText.SetActive(false);
        if (entryOpenText != null) entryOpenText.SetActive(false);
    }

    public bool GetAllAnimalsScreenAlreadyShown()
{
    return allAnimalsScreenAlreadyShown;
}

public void SetAllAnimalsScreenAlreadyShown(bool value)
{
    allAnimalsScreenAlreadyShown = value;
}
}