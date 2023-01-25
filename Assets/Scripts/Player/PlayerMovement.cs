using System;
using UnityEngine;

[System.Serializable]
public sealed class PlayerMovement : NestedBehaviour
{
    [SerializeField] float moveSpeed = 10.0f;
    [SerializeField] float groundAcceleration = 80.0f;

    [Space]
    [SerializeField] float airMoveAcceleration = 8.0f;

    [Space]
    [SerializeField] float jumpHeight = 3.5f;
    [SerializeField] float upGravity = 2.0f;
    [SerializeField] float downGravity = 3.0f;
    [SerializeField] float jumpSpringPauseTime = 0.1f;

    [Space]
    [SerializeField] float springDistance = 1.2f;
    [SerializeField] float springForce = 250.0f;
    [SerializeField] float springDamper = 15.0f;
    [SerializeField] float groundCheckRadius = 0.4f;
    [SerializeField] LayerMask groundCheckMask = 0b1;

    bool previousJumpState;
    float lastJumpTime;

    public float MoveSpeed => moveSpeed;

    public float DistanceToGround { get; private set; }
    public bool IsGrounded => DistanceToGround < 0.0f;
    public GameObject Ground { get; private set; }
    public Rigidbody GroundRigidbody { get; private set; }

    public Func<Vector2> MoveDirection { get; set; }
    public Func<bool> Jump { get; set; }
    public Vector2 PlanarSpeed(Rigidbody rigidbody) => new Vector2(rigidbody.velocity.x, rigidbody.velocity.z);
    public Vector2 LocalPlanarSpeed(Rigidbody rigidbody)
    {
        if (!IsGrounded) return PlanarSpeed(rigidbody);
        if (!GroundRigidbody) return PlanarSpeed(rigidbody);

        return PlanarSpeed(rigidbody) - new Vector2(GroundRigidbody.velocity.x, GroundRigidbody.velocity.z);
    }

    public void Process(Rigidbody rigidbody)
    {
        DistanceToGround = GetDistanceToGround(rigidbody) - springDistance;

        MoveCharacter(rigidbody);

        if (Jump() && !previousJumpState)
        {
            TryJump(rigidbody);
        }
        previousJumpState = Jump();

        ApplySpring(rigidbody);
        ApplyGravity(rigidbody);
    }

    private void ApplySpring(Rigidbody rigidbody)
    {
        if (IsGrounded && Time.time > lastJumpTime + jumpSpringPauseTime)
        {
            float contraction = 1.0f - ((DistanceToGround + springDistance) / springDistance);
            rigidbody.velocity += Vector3.up * contraction * springForce * Time.deltaTime;
            rigidbody.velocity -= Vector3.up * rigidbody.velocity.y * springDamper * Time.deltaTime;
        }
    }

    private void ApplyGravity(Rigidbody rigidbody)
    {
        rigidbody.useGravity = false;
        rigidbody.velocity += GetGravity(rigidbody) * Time.deltaTime;
    }

    private void MoveCharacter(Rigidbody rigidbody)
    {
        Vector2 input = MoveDirection();
        Vector3 direction = rigidbody.transform.TransformDirection(input.x, 0.0f, input.y);

        if (IsGrounded)
        {
            Vector3 target = direction * moveSpeed;
            Vector3 current = rigidbody.velocity;

            Vector3 delta = Vector3.ClampMagnitude(target - current, moveSpeed);
            delta.y = 0.0f;

            Vector3 force = delta / moveSpeed * groundAcceleration;

            rigidbody.velocity += force * Time.deltaTime;
        }
        else
        {
            rigidbody.velocity += direction * airMoveAcceleration * Time.deltaTime;
        }
    }

    private void TryJump(Rigidbody rigidbody)
    {
        if (IsGrounded)
        {
            float gravity = Vector3.Dot(Vector3.down, GetGravity(rigidbody));
            float jumpForce = Mathf.Sqrt(2.0f * gravity * jumpHeight);
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, jumpForce, rigidbody.velocity.z);

            lastJumpTime = Time.time;
        }
    }

    private Vector3 GetGravity(Rigidbody rigidbody)
    {
        float scale = upGravity;
        if (!Jump())
        {
            scale = downGravity;
        }
        else if (rigidbody.velocity.y < 0.0f)
        {
            scale = downGravity;
        }

        return Physics.gravity * scale;
    }

    public float GetDistanceToGround(Rigidbody rigidbody)
    {
        if (Physics.SphereCast(rigidbody.position + Vector3.up * groundCheckRadius, groundCheckRadius, Vector3.down, out var hit, 1000.0f, groundCheckMask))
        {
            Ground = hit.transform.gameObject;
            GroundRigidbody = hit.rigidbody;
            return hit.distance;
        }
        else
        {
            Ground = null;
            GroundRigidbody = null;
            return float.PositiveInfinity;
        }
    }
}
