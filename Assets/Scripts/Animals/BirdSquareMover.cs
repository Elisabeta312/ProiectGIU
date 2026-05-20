using UnityEngine;

public class BirdSquareMover : MonoBehaviour
{
    [Header("Square Constraint")]
    public Transform center;
    public Vector2 squareSize = new Vector2(30f, 30f);

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float turnSmoothSpeed = 8f;
    public float fixedHeight = 3f;

    [Header("Edge Behavior")]
    public float edgePadding = 0.5f;

    private Vector3 moveDirection;

    private void Start()
    {
        PickRandomDirection();
        FixHeight();
    }

    private void Update()
    {
        if (center == null)
        {
            return;
        }

        MoveForward();
        CheckEdges();
        RotateTowardsMoveDirection();
        FixHeight();
    }

    private void MoveForward()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    private void CheckEdges()
    {
        Vector3 localPosition = transform.position - center.position;

        float halfX = squareSize.x * 0.5f;
        float halfZ = squareSize.y * 0.5f;

        bool hitEdge = false;

        if (localPosition.x > halfX)
        {
            localPosition.x = halfX - edgePadding;
            hitEdge = true;
        }
        else if (localPosition.x < -halfX)
        {
            localPosition.x = -halfX + edgePadding;
            hitEdge = true;
        }

        if (localPosition.z > halfZ)
        {
            localPosition.z = halfZ - edgePadding;
            hitEdge = true;
        }
        else if (localPosition.z < -halfZ)
        {
            localPosition.z = -halfZ + edgePadding;
            hitEdge = true;
        }

        if (hitEdge)
        {
            transform.position = center.position + localPosition;
            PickRandomDirectionInsideSquare();
        }
    }

    private void PickRandomDirection()
    {
        float angle = Random.Range(0f, 360f);

        moveDirection = new Vector3(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            0f,
            Mathf.Sin(angle * Mathf.Deg2Rad)
        ).normalized;
    }

    private void PickRandomDirectionInsideSquare()
    {
        Vector3 localPosition = transform.position - center.position;

        Vector3 targetLocalPosition = new Vector3(
            Random.Range(-squareSize.x * 0.5f, squareSize.x * 0.5f),
            0f,
            Random.Range(-squareSize.y * 0.5f, squareSize.y * 0.5f)
        );

        Vector3 direction = targetLocalPosition - new Vector3(localPosition.x, 0f, localPosition.z);

        if (direction.sqrMagnitude < 0.01f)
        {
            PickRandomDirection();
            return;
        }

        moveDirection = direction.normalized;
    }

    private void RotateTowardsMoveDirection()
    {
        if (moveDirection.sqrMagnitude < 0.01f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            turnSmoothSpeed * Time.deltaTime
        );
    }

    private void FixHeight()
    {
        if (center == null)
        {
            return;
        }

        Vector3 position = transform.position;
        position.y = center.position.y + fixedHeight;
        transform.position = position;
    }
}