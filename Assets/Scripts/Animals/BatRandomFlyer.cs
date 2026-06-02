using UnityEngine;

public class BatRandomFlyer : MonoBehaviour
{
    [Header("Area")]
    public Vector3 areaCenter;
    public Vector2 areaSize = new Vector2(20f, 20f);
    public float minAltitude = 3f;
    public float maxAltitude = 10f;

    [Header("Movement")]
    public float speed = 3f;
    public float turnSpeed = 4f;
    public float targetReachDistance = 0.7f;

    [Header("Waiting")]
    public float minWaitTime = 0.2f;
    public float maxWaitTime = 1.2f;

    private Vector3 targetPosition;
    private float waitTimer;
    private bool isWaiting;

    private void Start()
    {
        PickNewTarget();
    }

    private void Update()
    {
        ClampInsideArea();

        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0f)
            {
                isWaiting = false;
                PickNewTarget();
            }

            return;
        }

        MoveToTarget();
    }

    private void MoveToTarget()
    {
        Vector3 direction = targetPosition - transform.position;

        if (direction.magnitude <= targetReachDistance)
        {
            StartWaiting();
            return;
        }

        Vector3 moveDirection = direction.normalized;

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

        ClampInsideArea();
    }

    private void PickNewTarget()
    {
        float randomX = Random.Range(-areaSize.x / 2f, areaSize.x / 2f);
        float randomZ = Random.Range(-areaSize.y / 2f, areaSize.y / 2f);
        float randomY = Random.Range(minAltitude, maxAltitude);

        targetPosition = new Vector3(
            areaCenter.x + randomX,
            areaCenter.y + randomY,
            areaCenter.z + randomZ
        );
    }

    private void StartWaiting()
    {
        isWaiting = true;
        waitTimer = Random.Range(minWaitTime, maxWaitTime);
    }

    private void ClampInsideArea()
    {
        float minX = areaCenter.x - areaSize.x / 2f;
        float maxX = areaCenter.x + areaSize.x / 2f;

        float minZ = areaCenter.z - areaSize.y / 2f;
        float maxZ = areaCenter.z + areaSize.y / 2f;

        float minY = areaCenter.y + minAltitude;
        float maxY = areaCenter.y + maxAltitude;

        Vector3 position = transform.position;

        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);
        position.z = Mathf.Clamp(position.z, minZ, maxZ);

        transform.position = position;
    }
}