using UnityEngine;

public class RatBehaviour : MonoBehaviour
{
    [Header("Target")]
    public Transform trashTarget;

    [Header("Ground")]
    public Terrain[] groundTerrains;
    public float groundOffset = 0.05f;

    [Header("Circle movement")]
    public float circleRadius = 2.5f;
    public float moveSpeed = 1.5f;
    public float turnSpeed = 8f;
    public bool clockwise = true;
    public float startAngle = 0f;

    [Header("Natural movement")]
    public float radiusVariation = 0.25f;
    public float wobbleSpeed = 2f;

    [Header("Animation")]
    public Animator animator;
    public string movingBoolName = "";
    public string movingTriggerName = "";

    private float currentAngle;
    private float wobbleOffset;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    private void Start()
    {
        currentAngle = startAngle;
        wobbleOffset = Random.Range(0f, 100f);

        SnapToGround();
        PlayMoveAnimation();
    }

    private void Update()
    {
        if (trashTarget == null)
        {
            return;
        }

        MoveAroundTrash();
    }

    private void MoveAroundTrash()
    {
        float direction = clockwise ? -1f : 1f;

        currentAngle += direction * moveSpeed * Time.deltaTime * 60f;

        float angleRad = currentAngle * Mathf.Deg2Rad;

        float naturalRadius = circleRadius + Mathf.Sin(Time.time * wobbleSpeed + wobbleOffset) * radiusVariation;

        Vector3 wantedPosition = trashTarget.position + new Vector3(
            Mathf.Cos(angleRad) * naturalRadius,
            0f,
            Mathf.Sin(angleRad) * naturalRadius
        );

        wantedPosition = GetGroundPosition(wantedPosition);

        Vector3 moveDirection = wantedPosition - transform.position;
        moveDirection.y = 0f;

        if (moveDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection.normalized, Vector3.up);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                turnSpeed * Time.deltaTime
            );
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            wantedPosition,
            moveSpeed * Time.deltaTime
        );
    }

    private void SnapToGround()
    {
        transform.position = GetGroundPosition(transform.position);
    }

    private Vector3 GetGroundPosition(Vector3 position)
    {
        Terrain terrain = GetTerrainAtPosition(position);

        if (terrain == null)
        {
            return position;
        }

        float y = terrain.transform.position.y + terrain.SampleHeight(position);
        return new Vector3(position.x, y + groundOffset, position.z);
    }

    private Terrain GetTerrainAtPosition(Vector3 position)
    {
        if (groundTerrains == null)
        {
            return null;
        }

        for (int i = 0; i < groundTerrains.Length; i++)
        {
            Terrain terrain = groundTerrains[i];

            if (terrain == null || terrain.terrainData == null)
            {
                continue;
            }

            Vector3 terrainPosition = terrain.transform.position;
            Vector3 terrainSize = terrain.terrainData.size;

            bool insideX = position.x >= terrainPosition.x && position.x <= terrainPosition.x + terrainSize.x;
            bool insideZ = position.z >= terrainPosition.z && position.z <= terrainPosition.z + terrainSize.z;

            if (insideX && insideZ)
            {
                return terrain;
            }
        }

        return null;
    }

    private void PlayMoveAnimation()
    {
        if (animator == null)
        {
            return;
        }

        if (!string.IsNullOrEmpty(movingBoolName))
        {
            animator.SetBool(movingBoolName, true);
        }

        if (!string.IsNullOrEmpty(movingTriggerName))
        {
            animator.SetTrigger(movingTriggerName);
        }
    }
}