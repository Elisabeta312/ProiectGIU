using UnityEngine;

public class WaspSwarmBehaviour : MonoBehaviour
{
    public Transform hivePoint;

    public float swarmRadius = 5f;
    public float minHeight = 0.5f;
    public float maxHeight = 3f;

    public float speed = 3f;
    public float turnSpeed = 8f;
    public float targetReachDistance = 0.35f;

    public float minWaitTime = 0.05f;
    public float maxWaitTime = 0.4f;

    public float returnDistance = 0.25f;

    private Vector3 targetPosition;
    private bool isReturning;
    private bool isWaiting;
    private float waitTimer;

    public void StartSwarming()
    {
        isReturning = false;
        isWaiting = false;
        PickNewSwarmTarget();
    }

    public void ReturnToHiveAndDisappear()
    {
        isReturning = true;
        isWaiting = false;

        if (hivePoint != null)
        {
            targetPosition = hivePoint.position;
        }
    }

    private void Start()
    {
        if (hivePoint != null && targetPosition == Vector3.zero)
        {
            PickNewSwarmTarget();
        }
    }

    private void Update()
    {
        if (hivePoint == null)
        {
            return;
        }

        if (isReturning)
        {
            MoveBackToHive();
            return;
        }

        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0f)
            {
                isWaiting = false;
                PickNewSwarmTarget();
            }

            return;
        }

        MoveToTarget();
    }

    private void MoveBackToHive()
    {
        targetPosition = hivePoint.position;

        Vector3 direction = targetPosition - transform.position;

        if (direction.magnitude <= returnDistance)
        {
            Destroy(gameObject);
            return;
        }

        MoveInDirection(direction.normalized);
    }

    private void MoveToTarget()
    {
        Vector3 direction = targetPosition - transform.position;

        if (direction.magnitude <= targetReachDistance)
        {
            isWaiting = true;
            waitTimer = Random.Range(minWaitTime, maxWaitTime);
            return;
        }

        MoveInDirection(direction.normalized);
    }

    private void MoveInDirection(Vector3 moveDirection)
    {
        if (moveDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                turnSpeed * Time.deltaTime
            );
        }

        transform.position += moveDirection * speed * Time.deltaTime;
    }

    private void PickNewSwarmTarget()
    {
        Vector2 randomCircle = Random.insideUnitCircle * swarmRadius;
        float randomHeight = Random.Range(minHeight, maxHeight);

        targetPosition = hivePoint.position + new Vector3(
            randomCircle.x,
            randomHeight,
            randomCircle.y
        );
    }
}