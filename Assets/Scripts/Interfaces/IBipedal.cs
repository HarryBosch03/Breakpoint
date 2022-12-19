using UnityEngine;

public interface IBipedal
{
    bool IsGrounded { get; }
    Vector2 PlanarVelocity { get; }
    Vector2 LocalPlanarVelocity { get; }
}
