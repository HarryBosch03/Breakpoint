using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[SelectionBase]
[DisallowMultipleComponent]
public sealed class PlayerAvatar : NetworkBehaviour, IBipedal, IAvatarRoot
{
    [SerializeField] InputActionAsset inputAsset;

    [Space]
    [SerializeField] Transform camRotor;
    [SerializeField] Transform weaponParent;

    [SerializeField] PlayerMovement movement;
    [SerializeField] new PlayerCamera camera;
    [SerializeField] PlayerWeaponManager weaponManager;

    InputActionMap playerMap;
    PlayerState playerState;

    new Rigidbody rigidbody;

    NetworkVariable<Vector2> moveDirection = new NetworkVariable<Vector2>(writePerm: NetworkVariableWritePermission.Owner);
    NetworkVariable<bool> jump = new NetworkVariable<bool>(writePerm: NetworkVariableWritePermission.Owner);
    NetworkVariable<bool> primaryFire = new NetworkVariable<bool>(writePerm: NetworkVariableWritePermission.Owner);
    NetworkVariable<bool> seccondaryFire = new NetworkVariable<bool>(writePerm: NetworkVariableWritePermission.Owner);
    NetworkVariable<bool> reload = new NetworkVariable<bool>(writePerm: NetworkVariableWritePermission.Owner);
    NetworkVariable<int> selectedWeapon = new NetworkVariable<int>(writePerm: NetworkVariableWritePermission.Owner);

    NetworkVariable<Vector2> ssRotation = new NetworkVariable<Vector2>(writePerm: NetworkVariableWritePermission.Owner);
    NetworkVariable<Vector3> position = new NetworkVariable<Vector3>(writePerm: NetworkVariableWritePermission.Owner);
    NetworkVariable<Vector3> velocity = new NetworkVariable<Vector3>(writePerm: NetworkVariableWritePermission.Owner);

    public bool IsGrounded => movement.IsGrounded;

    public Vector2 PlanarVelocity => movement.PlanarSpeed(rigidbody);
    public Vector2 LocalPlanarVelocity => movement.LocalPlanarSpeed(rigidbody);

    private void Awake()
    {
        playerMap = inputAsset.FindActionMap("Player");
        rigidbody = GetComponent<Rigidbody>();

        weaponManager.Initalize(this, weaponParent, camRotor, camera);

        movement.MoveDirection = () => moveDirection.Value;
        movement.Jump = () => jump.Value;
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
                ctx.Execute(movement, b => b.Process(rigidbody));
                break;
        }
    }

    private void Update()
    {
        var ctx = new NestedBehaviourExecutionContext();

        ReadInputMap();

        if (IsOwner)
        {
            ssRotation.Value = camera.SSRotation;
            position.Value = transform.position;
            velocity.Value = rigidbody.velocity;
        }
        else
        {
            camera.SSRotation = ssRotation.Value;
            transform.position = position.Value;
            rigidbody.velocity = velocity.Value;
        }

        switch (playerState)
        {
            case PlayerState.OnFoot:
            default:
                ctx.Execute(camera, b => b.Process(this, camRotor, IsOwner));
                ctx.Execute(weaponManager, b => b.Process(this, primaryFire.Value, seccondaryFire.Value, reload.Value, selectedWeapon.Value));
                break;
        }
    }

    private void ReadInputMap()
    {
        if (!IsOwner) return;

        System.Func<string, bool> getFlag = (s) =>
        {
            var action = playerMap.FindAction(s);
            return action.phase == InputActionPhase.Performed;
        };

        moveDirection.Value = playerMap.FindAction("move").ReadValue<Vector2>();
        jump.Value = getFlag("jump");

        primaryFire.Value = getFlag("PrimaryFire");
        seccondaryFire.Value = getFlag("SeccondaryFire");
        reload.Value = getFlag("Reload");

        if (getFlag("Weapon1")) selectedWeapon.Value = 0;
        if (getFlag("Weapon2")) selectedWeapon.Value = 1;
        if (getFlag("Holster")) selectedWeapon.Value = -1;
    }

    public enum PlayerState
    {
        OnFoot,
    }
}
