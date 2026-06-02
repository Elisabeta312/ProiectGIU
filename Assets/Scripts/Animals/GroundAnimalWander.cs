using UnityEngine;

[RequireComponent(typeof(Animator))]
public class GroundAnimalWander : MonoBehaviour
{
    [Header("Movement Area")]
    public BoxCollider roamingArea;
    public LayerMask groundMask = ~0;
    public bool useTerrainHeight = true;
    public float groundRayStartHeight = 50f;
    public float groundRayDistance = 120f;
    public float groundOffset = 0.02f;

    [Header("Movement")]
    public float moveSpeed = 1.2f;
    public float rotationSpeed = 6f;
    public float destinationTolerance = 0.4f;
    public float minWalkTime = 2f;
    public float maxWalkTime = 5f;
    public float minIdleTime = 1f;
    public float maxIdleTime = 4f;

    [Header("Animation")]
    public bool controlAnimator = true;
    public string movingBoolParameter = "isWalking";
    public string speedFloatParameter = "Speed";
    public float walkingAnimatorSpeed = 1f;
    public float idleAnimatorSpeed = 0f;

    [Header("Safety")]
    public bool avoidSteepSlopes = true;
    public float maxSlopeAngle = 45f;
    public int maxPointAttempts = 20;

    private Animator animator;
    private Vector3 destination;
    private float stateTimer;
    private bool isWalking;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (roamingArea == null)
        {
            roamingArea = GetComponentInParent<BoxCollider>();
        }
    }

    private void Start()
    {
        ChooseIdleState();
        PickNewDestination();
        SnapToGround();
    }

    private void Update()
    {
        stateTimer -= Time.deltaTime;

        if (isWalking)
        {
            MoveToDestination();

            if (Vector3.Distance(GetFlatPosition(transform.position), GetFlatPosition(destination)) <= destinationTolerance)
            {
                ChooseIdleState();
            }

            if (stateTimer <= 0f)
            {
                ChooseIdleState();
            }
        }
        else
        {
            if (stateTimer <= 0f)
            {
                PickNewDestination();
                ChooseWalkState();
            }
        }

        UpdateAnimator();
    }

    private void MoveToDestination()
    {
        Vector3 flatCurrent = GetFlatPosition(transform.position);
        Vector3 flatDestination = GetFlatPosition(destination);
        Vector3 direction = flatDestination - flatCurrent;

        if (direction.sqrMagnitude < 0.001f)
        {
            ChooseIdleState();
            return;
        }

        direction.Normalize();

        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        Vector3 nextPosition = transform.position + transform.forward * moveSpeed * Time.deltaTime;

        if (useTerrainHeight)
        {
            if (TryGetGroundPoint(nextPosition, out Vector3 groundedPosition, out _))
            {
                nextPosition = groundedPosition + Vector3.up * groundOffset;
            }
        }

        if (IsInsideArea(nextPosition))
        {
            transform.position = nextPosition;
        }
        else
        {
            PickNewDestination();
        }
    }

    private void PickNewDestination()
    {
        if (roamingArea == null)
        {
            destination = transform.position + Random.insideUnitSphere * 4f;
            destination.y = transform.position.y;
            return;
        }

        for (int i = 0; i < maxPointAttempts; i++)
        {
            Bounds bounds = roamingArea.bounds;

            Vector3 randomPoint = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                bounds.center.y,
                Random.Range(bounds.min.z, bounds.max.z)
            );

            if (useTerrainHeight)
            {
                if (TryGetGroundPoint(randomPoint, out Vector3 groundPoint, out RaycastHit hit))
                {
                    if (avoidSteepSlopes)
                    {
                        float slope = Vector3.Angle(hit.normal, Vector3.up);
                        if (slope > maxSlopeAngle)
                        {
                            continue;
                        }
                    }

                    destination = groundPoint + Vector3.up * groundOffset;
                    return;
                }
            }
            else
            {
                destination = randomPoint;
                return;
            }
        }

        destination = transform.position;
    }

    private bool TryGetGroundPoint(Vector3 point, out Vector3 groundPoint, out RaycastHit hit)
    {
        Vector3 rayStart = new Vector3(point.x, point.y + groundRayStartHeight, point.z);

        if (Physics.Raycast(rayStart, Vector3.down, out hit, groundRayDistance, groundMask, QueryTriggerInteraction.Ignore))
        {
            groundPoint = hit.point;
            return true;
        }

        groundPoint = point;
        return false;
    }

    private void SnapToGround()
    {
        if (!useTerrainHeight)
        {
            return;
        }

        if (TryGetGroundPoint(transform.position, out Vector3 groundPoint, out _))
        {
            transform.position = groundPoint + Vector3.up * groundOffset;
        }
    }

    private bool IsInsideArea(Vector3 point)
    {
        if (roamingArea == null)
        {
            return true;
        }

        Bounds bounds = roamingArea.bounds;

        return point.x >= bounds.min.x &&
               point.x <= bounds.max.x &&
               point.z >= bounds.min.z &&
               point.z <= bounds.max.z;
    }

    private Vector3 GetFlatPosition(Vector3 position)
    {
        return new Vector3(position.x, 0f, position.z);
    }

    private void ChooseWalkState()
    {
        isWalking = true;
        stateTimer = Random.Range(minWalkTime, maxWalkTime);
    }

    private void ChooseIdleState()
    {
        isWalking = false;
        stateTimer = Random.Range(minIdleTime, maxIdleTime);
    }

    private void UpdateAnimator()
    {
        if (!controlAnimator || animator == null)
        {
            return;
        }

        if (HasAnimatorParameter(movingBoolParameter, AnimatorControllerParameterType.Bool))
        {
            animator.SetBool(movingBoolParameter, isWalking);
        }

        if (HasAnimatorParameter(speedFloatParameter, AnimatorControllerParameterType.Float))
        {
            animator.SetFloat(speedFloatParameter, isWalking ? walkingAnimatorSpeed : idleAnimatorSpeed);
        }
    }

    private bool HasAnimatorParameter(string parameterName, AnimatorControllerParameterType type)
    {
        if (string.IsNullOrWhiteSpace(parameterName))
        {
            return false;
        }

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.name == parameterName && parameter.type == type)
            {
                return true;
            }
        }

        return false;
    }
}