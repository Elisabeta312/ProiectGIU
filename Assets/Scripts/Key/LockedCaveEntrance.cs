using UnityEngine;

public class LockedCaveEntrance : MonoBehaviour
{
    [Header("Entrance Id")]
    public string entranceId = "cave_entrance_1";
    [Header("Required Key")]
    public string requiredKeyId = "royal_key_1";

    [Header("Blocked Passage")]
    public Collider blockingCollider;

    [Header("State")]
    public bool isLocked = true;

    [Header("Input")]
    public KeyCode unlockKey = KeyCode.E;

    private bool playerIsNearby;

    private void Start()
    {
        if (CaveEntranceStateRegistry.IsOpened(entranceId))
        {
             isLocked = false;
        }
        ApplyLockState();
    }

    private void Update()
    {
        if (!isLocked)
        {
            return;
        }

        if (!playerIsNearby)
        {
            return;
        }

        if (Input.GetKeyDown(unlockKey))
        {
            TryUnlock();
        }
    }

    private void TryUnlock()
    {
        if (!PlayerKeyInventory.HasKey(requiredKeyId))
        {
            Debug.Log("You need the key: " + requiredKeyId);
            return;
        }

        isLocked = false;

        CaveEntranceStateRegistry.MarkOpened(entranceId);

        ApplyLockState();

        if (QuestFeedbackUI.Instance != null)
{
    QuestFeedbackUI.Instance.HideUnlockInstructionsText();
    QuestFeedbackUI.Instance.ShowCaveOpenScreen();
}

        Debug.Log("Cave entrance unlocked.");
    }

    private void ApplyLockState()
    {
        if (blockingCollider != null)
        {
            blockingCollider.enabled = isLocked;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
{
    playerIsNearby = true;

    if (!isLocked)
    {
        QuestFeedbackUI.Instance.ShowEntryOpenText();
        return;
    }

    if (PlayerKeyInventory.HasKey(requiredKeyId))
    {
        QuestFeedbackUI.Instance.ShowUnlockInstructionsText();
    }
    else
    {
        QuestFeedbackUI.Instance.ShowKeyRequiredText();
    }
}
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
{
    playerIsNearby = false;

    if (QuestFeedbackUI.Instance != null)
    {
        QuestFeedbackUI.Instance.HideUnlockInstructionsText();
    }
}
    }
}