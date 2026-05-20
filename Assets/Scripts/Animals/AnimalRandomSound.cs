using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AnimalRandomSound : MonoBehaviour
{
    [Header("Animal Sounds")]
    public AudioClip[] sounds;

    [Header("Random Time Interval")]
    public float minSeconds = 8f;
    public float maxSeconds = 20f;

    [Header("3D Audio Settings")]
    [Range(0f, 1f)]
    public float volume = 0.7f;

    public float minDistance = 2f;
    public float maxDistance = 25f;

    [Range(0f, 360f)]
    public float spread = 90f;

    [Header("Behavior")]
    public bool playOnlyWhenPlayerIsNear = true;
    public Transform player;
    public float activationDistance = 35f;

    [Header("Pitch Variation")]
    public bool randomPitch = true;
    public float minPitch = 0.95f;
    public float maxPitch = 1.05f;

    private AudioSource audioSource;
    private float timer;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 1f;
        audioSource.dopplerLevel = 0f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.volume = volume;
        audioSource.minDistance = minDistance;
        audioSource.maxDistance = maxDistance;
        audioSource.spread = spread;
    }

    private void Start()
    {
        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");

            if (foundPlayer != null)
            {
                player = foundPlayer.transform;
            }
        }

        ResetTimer();
    }

    private void Update()
    {
        if (sounds == null || sounds.Length == 0)
        {
            return;
        }

        if (playOnlyWhenPlayerIsNear && !IsPlayerNear())
        {
            return;
        }

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            PlayRandomSound();
            ResetTimer();
        }
    }

    private bool IsPlayerNear()
    {
        if (player == null)
        {
            return true;
        }

        float distance = Vector3.Distance(transform.position, player.position);
        return distance <= activationDistance;
    }

    private void PlayRandomSound()
    {
        if (audioSource.isPlaying)
        {
            return;
        }

        int index = Random.Range(0, sounds.Length);

        if (randomPitch)
        {
            audioSource.pitch = Random.Range(minPitch, maxPitch);
        }
        else
        {
            audioSource.pitch = 1f;
        }

        audioSource.PlayOneShot(sounds[index], volume);
    }

    private void ResetTimer()
    {
        if (maxSeconds < minSeconds)
        {
            maxSeconds = minSeconds;
        }

        timer = Random.Range(minSeconds, maxSeconds);
    }

    private void OnValidate()
    {
        if (maxSeconds < minSeconds)
        {
            maxSeconds = minSeconds;
        }

        if (maxDistance < minDistance)
        {
            maxDistance = minDistance;
        }

        if (activationDistance < maxDistance)
        {
            activationDistance = maxDistance;
        }

        AudioSource source = GetComponent<AudioSource>();

        if (source != null)
        {
            source.playOnAwake = false;
            source.loop = false;
            source.spatialBlend = 1f;
            source.dopplerLevel = 0f;
            source.rolloffMode = AudioRolloffMode.Linear;
            source.volume = volume;
            source.minDistance = minDistance;
            source.maxDistance = maxDistance;
            source.spread = spread;
        }
    }
}