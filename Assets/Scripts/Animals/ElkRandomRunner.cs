using UnityEngine;

public class ElkRandomRunner : MonoBehaviour
{
    public Vector3 areaCenter;
    public Vector2 areaSize = new Vector2(80f, 80f);

    public Terrain groundTerrain;
    public float groundOffset = 0.05f;

    public float speed = 4f;
    public float turnSpeed = 6f;
    public float targetReachDistance = 1f;
    public float minWaitTime = 0.2f;
    public float maxWaitTime = 1.5f;

    private Vector3 targetPosition;
    private bool isWaiting;
    private float waitTimer;

    private void Start()
    {
        SnapToGround();
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
        direction.y = 0f;

        if (direction.magnitude <= targetReachDistance)
        {
            isWaiting = true;
            waitTimer = Random.Range(minWaitTime, maxWaitTime);
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

        Vector3 nextPosition = transform.position + moveDirection * speed * Time.deltaTime;
        nextPosition = GetGroundPosition(nextPosition);

        transform.position = nextPosition;

        ClampInsideArea();
    }

    private void PickNewTarget()
    {
        float randomX = Random.Range(-areaSize.x / 2f, areaSize.x / 2f);
        float randomZ = Random.Range(-areaSize.y / 2f, areaSize.y / 2f);

        Vector3 position = new Vector3(
            areaCenter.x + randomX,
            areaCenter.y,
            areaCenter.z + randomZ
        );

        targetPosition = GetGroundPosition(position);
    }

    private void ClampInsideArea()
    {
        float minX = areaCenter.x - areaSize.x / 2f;
        float maxX = areaCenter.x + areaSize.x / 2f;
        float minZ = areaCenter.z - areaSize.y / 2f;
        float maxZ = areaCenter.z + areaSize.y / 2f;

        Vector3 position = transform.position;

        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.z = Mathf.Clamp(position.z, minZ, maxZ);

        transform.position = GetGroundPosition(position);
    }

    private void SnapToGround()
    {
        transform.position = GetGroundPosition(transform.position);
    }

    private Vector3 GetGroundPosition(Vector3 position)
    {
        if (groundTerrain == null)
        {
            return position;
        }

        float y = groundTerrain.transform.position.y + groundTerrain.SampleHeight(position);

        return new Vector3(position.x, y + groundOffset, position.z);
    }
}