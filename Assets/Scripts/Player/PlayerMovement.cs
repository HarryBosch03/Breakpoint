using System;
using UnityEngine;
using UnityEngine.UIElements;

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
    public Rigidbody DrivingRigidbody { get; private set; }

    public float DistanceToGround { get; private set; }
    public bool IsGrounded => DistanceToGround < 0.0f;
    public GameObject Ground { get; private set; }
    public Rigidbody GroundRigidbody { get; private set; }

    public Func<Vector2> MoveDirection { get; set; }
    public Func<bool> Jump { get; set; }
    public Vector2 PlanarSpeed => new Vector2(DrivingRigidbody.velocity.x, DrivingRigidbody.velocity.z);
    public Vector2 LocalPlanarSpeed
    {
        get
        {
            if (!IsGrounded) return PlanarSpeed;
            if (!GroundRigidbody) return PlanarSpeed;

            return PlanarSpeed - new Vector2(GroundRigidbody.velocity.x, GroundRigidbody.velocity.z);
        }
    }

    public PlayerMovement(GameObject context) : base(context)
    {
        DrivingRigidbody = context.GetComponent<Rigidbody>();
    }

    protected override void OnExecute()
    {
        DistanceToGround = GetDistanceToGround() - springDistance;

        MoveCharacter();

        if (Jump() && !previousJumpState)
        {
            TryJump();
        }
        previousJumpState = Jump();

        ApplySpring();
        ApplyGravity();
    }

    private void ApplySpring()
    {
        if (IsGrounded && Time.time > lastJumpTime + jumpSpringPauseTime)
        {
            float contraction = 1.0f - ((DistanceToGround + springDistance) / springDistance);
            DrivingRigidbody.velocity += Vector3.up * contraction * springForce * Time.deltaTime;
            DrivingRigidbody.velocity -= Vector3.up * DrivingRigidbody.velocity.y * springDamper * Time.deltaTime;
        }
    }

    private void ApplyGravity()
    {
        DrivingRigidbody.useGravity = false;
        DrivingRigidbody.velocity += GetGravity() * Time.deltaTime;
    }

    private void MoveCharacter()
    {
        Vector2 input = MoveDirection();
        Vector3 direction = Context.transform.TransformDirection(input.x, 0.0f, input.y);

        if (IsGrounded)
        {
            Vector3 target = direction * moveSpeed;
            Vector3 current = DrivingRigidbody.velocity;

            Vector3 delta = Vector3.ClampMagnitude(target - current, moveSpeed);
            delta.y = 0.0f;

            Vector3 force = delta / moveSpeed * groundAcceleration;

            DrivingRigidbody.velocity += force * Time.deltaTime;
        }
        else
        {
            DrivingRigidbody.velocity += direction * airMoveAcceleration * Time.deltaTime;
        }
    }

    private void TryJump()
    {
        if (IsGrounded)
        {
            float gravity = Vector3.Dot(Vector3.down, GetGravity());
            float jumpForce = Mathf.Sqrt(2.0f * gravity * jumpHeight);
            DrivingRigidbody.velocity = new Vector3(DrivingRigidbody.velocity.x, jumpForce, DrivingRigidbody.velocity.z);

            lastJumpTime = Time.time;
        }
    }

    private Vector3 GetGravity()
    {
        float scale = upGravity;
        if (!Jump())
        {
            scale = downGravity;
        }
        else if (DrivingRigidbody.velocity.y < 0.0f)
        {
            scale = downGravity;
        }

        return Physics.gravity * scale;
    }

    public float GetDistanceToGround()
    {
        if (Physics.SphereCast(DrivingRigidbody.position + Vector3.up * groundCheckRadius, groundCheckRadius, Vector3.down, out var hit, 1000.0f, groundCheckMask))
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
