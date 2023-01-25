using UnityEngine;

[System.Serializable]
public sealed class WeaponSway 
{
    [SerializeField] float magnitude;
    [SerializeField] float smoothTime;
    [SerializeField] float maxMagnitude;

    public Transform Root { get; set; }

    Vector2 lastCamRotation;
    Vector2 sway;
    Vector2 velocity;

    public void LateLoop ()
    {
        Quaternion worldCamRotation = Camera.main.transform.rotation;
        Vector2 camRotation = new Vector2(worldCamRotation.eulerAngles.y, worldCamRotation.eulerAngles.x);

        Vector2 delta = camRotation - lastCamRotation;
        lastCamRotation = camRotation;

        Vector2 target = Vector2.ClampMagnitude(delta * magnitude, maxMagnitude);
        sway = Vector2.SmoothDamp(sway, target, ref velocity, smoothTime);
        Root.transform.rotation *= Quaternion.Euler(-sway.y, 0.0f, sway.x);
    }
}
