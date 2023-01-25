using UnityEngine;

[System.Serializable]
public sealed class WeaponProcedualAnimator
{
    [SerializeField] SeccondOrderDynamicsV3 positionDriver;
    [SerializeField] SeccondOrderDynamicsV2 rotationDriver;

    [Space]
    [SerializeField] Vector3 rotationOffset;

    [Space]
    [SerializeField] Vector3 idlePosition;
    [SerializeField] Vector2 idleRotation;

    [Space]
    [SerializeField] Vector3 aimPosition;
    [SerializeField] Vector2 aimRotation;

    [Space]
    [SerializeField] Vector3 recoilForceA;
    [SerializeField] Vector3 recoilForceB;

    [Space]
    [SerializeField] Vector2 recoilTorqueA;
    [SerializeField] Vector2 recoilTorqueB;

    public float AimPercent { get; set; }

    public void FixedLoop (Transform cameraRotor, Transform root)
    {
        Vector3 position = GetPosition();
        Vector2 rotation = GetRotation();

        rotationDriver.ProcessOperation = SeccondOrderDynamics.ProcessRotation;

        positionDriver.Process(position, null, Time.deltaTime);

        rotationDriver.Position = new Vector2(SimplifyAngle(rotationDriver.Position.x), rotationDriver.Position.y);
        rotationDriver.Process(rotation, null, Time.deltaTime);

        root.localPosition = positionDriver.Position;
        root.localRotation = Quaternion.Euler(new Vector3(rotationDriver.Position.y, rotationDriver.Position.x, 0.0f)) * Quaternion.Euler(rotationOffset);
    }

    private Vector3 GetPosition()
    {
        return Vector3.Lerp(idlePosition, aimPosition, AimPercent);
    }

    private Vector2 GetRotation()
    {
        return Vector2.Lerp(idleRotation, aimRotation, AimPercent);
    }

    public void Shoot ()
    {
        Vector3 force = Vector3.Lerp(recoilForceA, recoilForceB, UnityEngine.Random.value);
        Vector2 torque = Vector2.Lerp(recoilTorqueA, recoilTorqueB, UnityEngine.Random.value);

        positionDriver.Velocity += force;
        rotationDriver.Velocity += torque;
    }

    public float SimplifyAngle(float v) => v - Mathf.Floor(v / 360.0f) * 360.0f;
}
