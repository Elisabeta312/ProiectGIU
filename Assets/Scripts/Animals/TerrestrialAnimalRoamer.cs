using System.Collections;
using UnityEngine;

public class TerrestrialAnimalRoamer : MonoBehaviour
{
    [Header("Roaming Area")]
    public Vector3 areaCenter;
    public Vector3 areaSize;

    [Header("Terrain")]
    public Terrain terrain;
    public LayerMask groundMask = ~0;
    public bool useTerrainHeight = true;
    public float yOffset = 0f;
    public float rayStartHeight = 80f;
    public float rayDistance = 200f;

    [Header("Movement")]
    public float moveSpeed = 1.8f;
    public float rotationSpeed = 6f;
    public float destinationTolerance = 1.2f;

    [Header("Waiting")]
    public float waitMinSeconds = 1f;
    public float waitMaxSeconds = 4f;

    [Header("Animation")]
    public bool controlAnimator = true;
    public Animator animator;
    public string movingBoolParameter = "isWalking";
    public string speedFloatParameter = "Speed";

    private Vector3 currentDestination;
    private bool waiting = false;
    private bool isMoving = false;

    public void SetRoamArea(Vector3 center, Vector3 size)
    {
        areaCenter = center;
        areaSize = size;
    }

    private void Awake()
    {
        if (terrain == null)
        {
            terrain = Terrain.activeTerrain;
        }

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    private void Start()
    {
        SnapToGround();
        PickNewDestination();
    }

    private void Update()
    {
        if (waiting)
        {
            SetAnimation(false, 0f);
            return;
        }

        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector3 currentPosition = transform.position;

        Vector3 flatTarget = new Vector3(
            currentDestination.x,
            currentPosition.y,
            currentDestination.z
        );

        Vector3 direction = flatTarget - currentPosition;
        direction.y = 0f;

        if (direction.magnitude <= destinationTolerance)
        {
            SetAnimation(false, 0f);
            StartCoroutine(WaitThenPickNewDestination());
            return;
        }

        Vector3 moveDirection = direction.normalized;

        Vector3 nextPosition = currentPosition + moveDirection * moveSpeed * Time.deltaTime;

        nextPosition.x = Mathf.Clamp(nextPosition.x, areaCenter.x - areaSize.x * 0.5f, areaCenter.x + areaSize.x * 0.5f);
        nextPosition.z = Mathf.Clamp(nextPosition.z, areaCenter.z - areaSize.z * 0.5f, areaCenter.z + areaSize.z * 0.5f);
        nextPosition.y = GetGroundY(nextPosition) + yOffset;

        transform.position = nextPosition;

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        SetAnimation(true, moveSpeed);
    }

    private IEnumerator WaitThenPickNewDestination()
    {
        waiting = true;
        isMoving = false;
        SetAnimation(false, 0f);

        float waitTime = Random.Range(waitMinSeconds, waitMaxSeconds);
        yield return new WaitForSeconds(waitTime);

        PickNewDestination();

        waiting = false;
    }

    private void PickNewDestination()
    {
        currentDestination = GetRandomGroundPointInsideArea();
    }

    private Vector3 GetRandomGroundPointInsideArea()
    {
        Bounds bounds = GetAreaBounds();

        for (int attempt = 0; attempt < 40; attempt++)
        {
            float randomX = Random.Range(bounds.min.x, bounds.max.x);
            float randomZ = Random.Range(bounds.min.z, bounds.max.z);

            Vector3 point = new Vector3(randomX, areaCenter.y, randomZ);
            point.y = GetGroundY(point) + yOffset;

            if (IsPointInsideBoundsXZ(point, bounds))
            {
                return point;
            }
        }

        Vector3 fallback = bounds.center;
        fallback.y = GetGroundY(fallback) + yOffset;
        return fallback;
    }

    private float GetGroundY(Vector3 position)
    {
        if (useTerrainHeight && terrain != null)
        {
            return terrain.SampleHeight(position) + terrain.transform.position.y;
        }

        Vector3 rayOrigin = new Vector3(position.x, position.y + rayStartHeight, position.z);
        Ray ray = new Ray(rayOrigin, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, groundMask, QueryTriggerInteraction.Ignore))
        {
            return hit.point.y;
        }

        return position.y;
    }

    private void SnapToGround()
    {
        Vector3 pos = transform.position;
        pos.y = GetGroundY(pos) + yOffset;
        transform.position = pos;
    }

    private Bounds GetAreaBounds()
    {
        return new Bounds(areaCenter, areaSize);
    }

    private bool IsPointInsideBoundsXZ(Vector3 point, Bounds bounds)
    {
        return point.x >= bounds.min.x &&
               point.x <= bounds.max.x &&
               point.z >= bounds.min.z &&
               point.z <= bounds.max.z;
    }

    private void SetAnimation(bool moving, float speed)
    {
        if (!controlAnimator || animator == null)
        {
            return;
        }

        if (isMoving == moving)
        {
            SetFloatIfExists(speedFloatParameter, speed);
            return;
        }

        isMoving = moving;

        SetBoolIfExists(movingBoolParameter, moving);
        SetFloatIfExists(speedFloatParameter, speed);
    }

    private void SetBoolIfExists(string parameterName, bool value)
    {
        if (string.IsNullOrWhiteSpace(parameterName))
        {
            return;
        }

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.name == parameterName && parameter.type == AnimatorControllerParameterType.Bool)
            {
                animator.SetBool(parameterName, value);
                return;
            }
        }
    }

    private void SetFloatIfExists(string parameterName, float value)
    {
        if (string.IsNullOrWhiteSpace(parameterName))
        {
            return;
        }

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.name == parameterName && parameter.type == AnimatorControllerParameterType.Float)
            {
                animator.SetFloat(parameterName, value);
                return;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(areaCenter, areaSize);
    }
}