using UnityEngine;

public class FishSwim3D : MonoBehaviour
{
    [Header("Area")]
    public Transform areaCenter;
    public Vector3 areaSize = new Vector3(20f, 8f, 20f);

    [Header("Movement")]
    public float minSpeed = 1.2f;
    public float maxSpeed = 3.5f;
    public float turnSpeed = 2.5f;
    public float targetReachDistance = 0.6f;

    [Header("Idle Variation")]
    public float minTargetChangeTime = 2f;
    public float maxTargetChangeTime = 6f;

    [Header("Model Rotation Fix")]
    public Vector3 rotationOffset = Vector3.zero;

    private Vector3 currentTarget;
    private float currentSpeed;
    private float nextTargetTimer;

    private void Start()
    {
        currentSpeed = Random.Range(minSpeed, maxSpeed);
        PickNewTarget();
    }

    private void Update()
    {
        if (areaCenter == null)
            return;

        MoveFish();

        nextTargetTimer -= Time.deltaTime;

        if (Vector3.Distance(transform.position, currentTarget) <= targetReachDistance || nextTargetTimer <= 0f)
        {
            PickNewTarget();
        }
    }

    private void MoveFish()
    {
        Vector3 direction = currentTarget - transform.position;

        if (direction.sqrMagnitude < 0.01f)
            return;

        direction.Normalize();

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        targetRotation *= Quaternion.Euler(rotationOffset);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            turnSpeed * Time.deltaTime
        );

        transform.position += direction * currentSpeed * Time.deltaTime;

        KeepInsideArea();
    }

    private void PickNewTarget()
    {
        currentTarget = GetRandomPointInsideArea();
        currentSpeed = Random.Range(minSpeed, maxSpeed);
        nextTargetTimer = Random.Range(minTargetChangeTime, maxTargetChangeTime);
    }

    private Vector3 GetRandomPointInsideArea()
    {
        Vector3 center = areaCenter.position;

        float x = Random.Range(-areaSize.x / 2f, areaSize.x / 2f);
        float y = Random.Range(-areaSize.y / 2f, areaSize.y / 2f);
        float z = Random.Range(-areaSize.z / 2f, areaSize.z / 2f);

        return center + new Vector3(x, y, z);
    }

    private void KeepInsideArea()
    {
        Vector3 center = areaCenter.position;

        float minX = center.x - areaSize.x / 2f;
        float maxX = center.x + areaSize.x / 2f;

        float minY = center.y - areaSize.y / 2f;
        float maxY = center.y + areaSize.y / 2f;

        float minZ = center.z - areaSize.z / 2f;
        float maxZ = center.z + areaSize.z / 2f;

        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        pos.z = Mathf.Clamp(pos.z, minZ, maxZ);

        transform.position = pos;
    }
}