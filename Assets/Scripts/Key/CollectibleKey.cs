using UnityEngine;


public class CollectibleKey : MonoBehaviour
{
    [Header("Key")]
    public string keyId = "royal_key_1";

    [Header("Unlock Condition")]
    public bool requiresAllAnimalsDiscovered = true;

    private Collider keyCollider;
    private Renderer[] keyRenderers;
    private bool unlocked;

    private void Awake()
    {
        keyCollider = GetComponent<Collider>();
        keyRenderers = GetComponentsInChildren<Renderer>(true);
    }

    private void Start()
    {
        RefreshState();
    }

    private void Update()
    {
        if (!unlocked)
        {
            RefreshState();
        }
    }

    private void RefreshState()
    {
        if (PlayerKeyInventory.HasKey(keyId))
        {
            gameObject.SetActive(false);
            return;
        }

        unlocked = !requiresAllAnimalsDiscovered || AnimalCollectionGoal.AreAllAnimalsDiscovered();

        SetKeyVisible(unlocked);
    }

    private void SetKeyVisible(bool visible)
    {
        foreach (Renderer keyRenderer in keyRenderers)
        {
            if (keyRenderer != null)
            {
                keyRenderer.enabled = visible;
            }
        }

        if (keyCollider != null)
        {
            keyCollider.enabled = visible;
            keyCollider.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!unlocked)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        PlayerKeyInventory.AddKey(keyId);
        if (QuestFeedbackUI.Instance != null)
{
    QuestFeedbackUI.Instance.ShowKeyScreen();
}

        Debug.Log("Key acquired: " + keyId);

        gameObject.SetActive(false);
    }
}
