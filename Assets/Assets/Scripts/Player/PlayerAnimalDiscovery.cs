using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerAnimalDiscovery : MonoBehaviour
{
    public float discoveryDistance = 8f;
    public KeyCode legacyKey = KeyCode.E;
    

    void Update()
    {
        bool pressed = false;

#if ENABLE_INPUT_SYSTEM
        var keyboard = Keyboard.current;
        if (keyboard != null)
        {
            pressed = keyboard.eKey.wasPressedThisFrame;
            // sau pentru C:
            // pressed = keyboard.cKey.wasPressedThisFrame;
        }
#elif ENABLE_LEGACY_INPUT_MANAGER
        pressed = Input.GetKeyDown(legacyKey);
#endif

        if (!pressed) return;

        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, discoveryDistance);

        foreach (Collider nearbyCollider in nearbyColliders)
        {
            DiscoverableAnimal animal = nearbyCollider.GetComponentInParent<DiscoverableAnimal>();

            if (animal != null)
            {
                animal.Discover();
                break;
            }
        }
    }
}
