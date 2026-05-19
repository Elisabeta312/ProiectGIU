using UnityEngine;

public class AnimalAnimatorAutoDisable : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Distances")]
    public float activeDistance = 35f;

    [Header("Check")]
    public float checkInterval = 0.5f;

    private Animator[] animators;
    private float timer;

    private void Awake()
    {
        animators = GetComponentsInChildren<Animator>(true);

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer < checkInterval)
        {
            return;
        }

        timer = 0f;

        if (player == null)
        {
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);
        bool shouldAnimate = distance <= activeDistance;

        foreach (Animator animator in animators)
        {
            if (animator == null)
            {
                continue;
            }

            if (animator.enabled != shouldAnimate)
            {
                animator.enabled = shouldAnimate;
            }
        }
    }
}