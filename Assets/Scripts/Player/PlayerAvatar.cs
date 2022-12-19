using System;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.InputSystem;

[SelectionBase]
[DisallowMultipleComponent]
public sealed class PlayerAvatar : MonoBehaviour, IBipedal
{
    [SerializeField] InputActionAsset inputAsset;

    [Space]
    [SerializeField] Transform cameraContainter;

    InputActionMap playerMap;
    MovementState movementState;

    PlayerMovement movement;
    new PlayerCamera camera;

    Vector2 moveDirection;
    bool jump;

    public bool IsGrounded => movement.IsGrounded;

    public Vector2 PlanarVelocity => movement.PlanarSpeed;
    public Vector2 LocalPlanarVelocity => movement.LocalPlanarSpeed;

    private void Awake()
    {
        playerMap = inputAsset.FindActionMap("Player");

        movement = new PlayerMovement(gameObject);
        camera = new PlayerCamera(gameObject, cameraContainter);

        movement.MoveDirection = () => moveDirection;
        movement.Jump = () => jump;
    }

    private void OnEnable()
    {
        playerMap.Enable();
    }

    private void OnDisable()
    {
        playerMap.Disable();
    }

    private void FixedUpdate()
    {
        var ctx = new NestedBehaviourExecutionContext();

        switch (movementState)
        {
            case MovementState.OnFoot:
            default:
                movement.Execute(ctx);
                break;
        }
    }

    private void Update()
    {
        var ctx = new NestedBehaviourExecutionContext();

        ReadInputMap();

        switch (movementState)
        {
            case MovementState.OnFoot:
            default:
                camera.Execute(ctx);
                break;
        }
    }

    private void ReadInputMap()
    {
        moveDirection = playerMap.FindAction("move").ReadValue<Vector2>();
        jump = playerMap.FindAction("jump").ReadValue<float>() > 0.5f;
    }

    public enum MovementState
    {
        OnFoot,
    }
}
