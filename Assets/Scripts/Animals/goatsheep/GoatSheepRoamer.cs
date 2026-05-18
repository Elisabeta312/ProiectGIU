using System.Collections;
using UnityEngine;

public class GoatSheepRoamer : MonoBehaviour
{
    [Header("Roaming Area")]
    public Vector3 areaCenter;
    public Vector3 areaSize;

    [Header("Terrain")]
    public Terrain terrain;
    public bool useTerrainHeight = true;
    public LayerMask groundMask = ~0;
    public float yOffset = 0f;
    public float rayStartHeight = 80f;
    public float rayDistance = 200f;

    [Header("Movement")]
    public float moveSpeed = 1.15f;
    public float rotationSpeed = 5f;
    public float destinationTolerance = 1.2f;

    [Header("Waiting")]
    public float waitMinSeconds = 2f;
    public float waitMaxSeconds = 5f;

    [Header("Animation States")]
    public Animator animator;
    public string idleStateName = "Idle";
    public string walkStateName = "walk_forward";
    public float animationFadeTime = 0.2f;

    [Header("Animation Stability")]
    public float minRealMoveDistance = 0.002f;
    public float walkStartDelay = 0.08f;
    public float idleStartDelay = 0.15f;

    private Vector3 currentDestination;
    private Vector3 lastPosition;

    private bool waiting = false;
    private bool animationWalking = false;

    private float movingTimer = 0f;
    private float idleTimer = 0f;

    private Coroutine waitRoutine;

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

        lastPosition = transform.position;

        ForceIdle();
        PickNewDestination();
    }

    private void Update()
    {
        Vector3 positionBeforeMove = transform.position;

        if (!waiting)
        {
            MoveToDestination();
        }

        UpdateAnimationFromRealMovement(positionBeforeMove, transform.position);

        lastPosition = transform.position;
    }

    private void MoveToDestination()
    {
        Vector3 currentPosition = transform.position;

        Vector3 target = new Vector3(
            currentDestination.x,
            currentPosition.y,
            currentDestination.z
        );

        Vector3 direction = target - currentPosition;
        direction.y = 0f;

        if (direction.magnitude <= destinationTolerance)
        {
            if (waitRoutine == null)
            {
                waitRoutine = StartCoroutine(WaitThenChooseNewDestination());
            }

            return;
        }

        Vector3 moveDirection = direction.normalized;

        Vector3 nextPosition = currentPosition + moveDirection * moveSpeed * Time.deltaTime;

        nextPosition.x = Mathf.Clamp(
            nextPosition.x,
            areaCenter.x - areaSize.x * 0.5f,
            areaCenter.x + areaSize.x * 0.5f
        );

        nextPosition.z = Mathf.Clamp(
            nextPosition.z,
            areaCenter.z - areaSize.z * 0.5f,
            areaCenter.z + areaSize.z * 0.5f
        );

        nextPosition.y = GetGroundY(nextPosition) + yOffset;

        transform.position = nextPosition;

        if (moveDirection.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    private IEnumerator WaitThenChooseNewDestination()
    {
        waiting = true;

        float waitTime = Random.Range(waitMinSeconds, waitMaxSeconds);
        yield return new WaitForSeconds(waitTime);

        PickNewDestination();

        waiting = false;
        waitRoutine = null;
    }

    private void PickNewDestination()
    {
        Bounds bounds = new Bounds(areaCenter, areaSize);

        for (int attempt = 0; attempt < 60; attempt++)
        {
            float x = Random.Range(bounds.min.x, bounds.max.x);
            float z = Random.Range(bounds.min.z, bounds.max.z);

            Vector3 point = new Vector3(x, areaCenter.y, z);
            point.y = GetGroundY(point) + yOffset;

            if (IsInsideAreaXZ(point, bounds))
            {
                currentDestination = point;
                return;
            }
        }

        Vector3 fallback = bounds.center;
        fallback.y = GetGroundY(fallback) + yOffset;
        currentDestination = fallback;
    }

    private void UpdateAnimationFromRealMovement(Vector3 before, Vector3 after)
    {
        if (animator == null)
        {
            return;
        }

        Vector3 delta = after - before;
        delta.y = 0f;

        bool reallyMoved = delta.magnitude >= minRealMoveDistance && !waiting;

        if (reallyMoved)
        {
            movingTimer += Time.deltaTime;
            idleTimer = 0f;

            if (!animationWalking && movingTimer >= walkStartDelay)
            {
                PlayWalk();
            }
        }
        else
        {
            idleTimer += Time.deltaTime;
            movingTimer = 0f;

            if (animationWalking && idleTimer >= idleStartDelay)
            {
                PlayIdle();
            }
        }
    }

    private float GetGroundY(Vector3 position)
    {
        if (useTerrainHeight && terrain != null)
        {
            return terrain.SampleHeight(position) + terrain.transform.position.y;
        }

        Vector3 rayOrigin = new Vector3(
            position.x,
            position.y + rayStartHeight,
            position.z
        );

        Ray ray = new Ray(rayOrigin, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, groundMask, QueryTriggerInteraction.Ignore))
        {
            return hit.point.y;
        }

        return position.y;
    }

    private void SnapToGround()
    {
        Vector3 position = transform.position;
        position.y = GetGroundY(position) + yOffset;
        transform.position = position;
    }

    private bool IsInsideAreaXZ(Vector3 point, Bounds bounds)
    {
        return point.x >= bounds.min.x &&
               point.x <= bounds.max.x &&
               point.z >= bounds.min.z &&
               point.z <= bounds.max.z;
    }

    private void PlayWalk()
    {
        if (animator == null)
        {
            return;
        }

        animationWalking = true;

        if (!string.IsNullOrWhiteSpace(walkStateName))
        {
            animator.CrossFade(walkStateName, animationFadeTime);
        }
    }

    private void PlayIdle()
    {
        if (animator == null)
        {
            return;
        }

        animationWalking = false;

        if (!string.IsNullOrWhiteSpace(idleStateName))
        {
            animator.CrossFade(idleStateName, animationFadeTime);
        }
    }

    private void ForceIdle()
    {
        if (animator == null)
        {
            return;
        }

        animationWalking = false;

        if (!string.IsNullOrWhiteSpace(idleStateName))
        {
            animator.Play(idleStateName, 0, 0f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(areaCenter, areaSize);
    }
}