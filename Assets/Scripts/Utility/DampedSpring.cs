using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Utility/Damped Spring")]
public class DampedSpring : ScriptableObject
{
    public float strength;
    public float dampen;
    public float rotationForceScaling = 90.0f;

    public DampedSpring() { }
    public DampedSpring(float strength, float dampen) 
    {
        this.strength = strength;
        this.dampen = dampen;
    }

    public void Process(ref Vector3 position, ref Vector3 velocity, Vector3 target) => Process(ref position, ref velocity, target, Time.deltaTime);
    public void Process(ref Vector3 position, ref Vector3 velocity, Vector3 target, float deltaTime)
    {
        Process(ref position.x, ref velocity.x, target.x, deltaTime);
        Process(ref position.y, ref velocity.y, target.y, deltaTime);
        Process(ref position.z, ref velocity.z, target.z, deltaTime);
    }

    public void Process(Vector3 position, ref Vector3 velocity, Vector3 target) => Process(ref position, ref velocity, target, Time.deltaTime);
    public void Process(Vector3 position, ref Vector3 velocity, Vector3 target, float deltaTime)
    {
        Process(position.x, ref velocity.x, target.x, deltaTime);
        Process(position.y, ref velocity.y, target.y, deltaTime);
        Process(position.z, ref velocity.z, target.z, deltaTime);
    }

    public void Process(ref float position, ref float velocity, float target) => Process(ref position, ref velocity, target, Time.deltaTime);
    public void Process(ref float position, ref float velocity, float target, float deltaTime)
    {
        Process(position, ref velocity, target, deltaTime);
        position += velocity * deltaTime;
    }

    public void Process(float position, ref float velocity, float target) => Process(position, ref velocity, target, Time.deltaTime);
    public void Process(float position, ref float velocity, float target, float deltaTime)
    {
        velocity += Process(position, velocity, target) * deltaTime;
    }

    public float Process(float position, float velocity, float target)
    {
        float force = 0.0f;

        force += (target - position) * strength;
        force += -velocity * dampen;

        return force;
    }

    public void ProcessAngle(ref float position, ref float velocity, float target) => ProcessAngle(ref position, ref velocity, target, Time.deltaTime);
    public void ProcessAngle(ref float position, ref float velocity, float target, float deltaTime)
    {
        ProcessAngle(position, ref velocity, target, deltaTime);
        position += velocity * deltaTime;
    }

    public void ProcessAngle(float position, ref float velocity, float target) => ProcessAngle(position, ref velocity, target, Time.deltaTime);
    public void ProcessAngle(float position, ref float velocity, float target, float deltaTime)
    {
        velocity += ProcessAngle(position, velocity, target) * deltaTime;
    }

    public float ProcessAngle(float position, float velocity, float target)
    {
        float force = 0.0f;

        force += Mathf.DeltaAngle(position, target) * strength;
        force += -velocity * dampen;

        return force;
    }
}
