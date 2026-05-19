using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class JournalDiscoveryHighlighter : MonoBehaviour
{
    [Header("Target")]
    public Image journalImage;

    [Header("Glow")]
    public Color normalColor = Color.white;
    public Color highlightColor = new Color(1f, 0.85f, 0.15f, 1f);

    [Header("Timing")]
    public float flashDuration = 1.2f;
    public float flashSpeed = 8f;

    private Coroutine flashRoutine;

    private void Awake()
    {
        if (journalImage == null)
        {
            journalImage = GetComponent<Image>();
        }

        if (journalImage != null)
        {
            normalColor = journalImage.color;
        }
    }

    public void FlashJournal()
    {
        if (journalImage == null)
        {
            return;
        }

        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }

        flashRoutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        float timer = 0f;

        while (timer < flashDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.PingPong(timer * flashSpeed, 1f);
            journalImage.color = Color.Lerp(normalColor, highlightColor, t);

            yield return null;
        }

        journalImage.color = normalColor;
        flashRoutine = null;
    }
}