using System;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.InputSystem;

[SelectionBase]
[DisallowMultipleComponent]
public sealed class PlayerAvatar : MonoBehaviour, IBipedal, IAvatarRoot
{
    [SerializeField] InputActionAsset inputAsset;

    [Space]
    [SerializeField] Transform cameraContainter;
    [SerializeField] Transform weaponContainer;
    [SerializeField] Cinemachine.CinemachineVirtualCamera fpCam;
    [SerializeField] RenderObjects[] viewmodelRenderObjects;

    PlayerMovement movement;
    new PlayerCamera camera;
    PlayerWeaponManager weaponManager;

    InputActionMap playerMap;
    PlayerState playerState;

    Vector2 moveDirection;
    bool jump;
    bool primaryFire;
    bool seccondaryFire;
    bool reload;

    public bool IsGrounded => movement.IsGrounded;

    public Vector2 PlanarVelocity => movement.PlanarSpeed;
    public Vector2 LocalPlanarVelocity => movement.LocalPlanarSpeed;

    private void Awake()
    {
        playerMap = inputAsset.FindActionMap("Player");

        movement = new PlayerMovement(this);
        camera = new PlayerCamera(this, cameraContainter, fpCam, viewmodelRenderObjects);

        movement.MoveDirection = () => moveDirection;
        movement.Jump = () => jump;

        weaponManager = new PlayerWeaponManager(this, weaponContainer, () => primaryFire, () => seccondaryFire, () => reload);
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

        switch (playerState)
        {
            case PlayerState.OnFoot:
            default:
                movement.Execute(ctx);
                break;
        }
    }

    private void Update()
    {
        var ctx = new NestedBehaviourExecutionContext();

        ReadInputMap();

        switch (playerState)
        {
            case PlayerState.OnFoot:
            default:
                camera.Execute(ctx);
                weaponManager.Execute(ctx);
                break;
        }
    }

    private void ReadInputMap()
    {
        System.Func<string, bool> getFlag = (s) =>
        {
            return playerMap.FindAction(s).ReadValue<float>() > 0.5f;
        };

        moveDirection = playerMap.FindAction("move").ReadValue<Vector2>();
        jump = getFlag("jump");

        primaryFire = getFlag("PrimaryFire");
        seccondaryFire = getFlag("SeccondaryFire");
        reload = getFlag("Reload");
    }

    public enum PlayerState
    {
        OnFoot,
    }
}
