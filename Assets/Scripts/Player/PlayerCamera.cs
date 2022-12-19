using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.Experimental.Rendering.Universal;

public sealed class PlayerCamera : NestedBehaviour
{
    [SerializeField] float sensitivity = 0.3f;
    [SerializeField] CinemachineVirtualCamera fpCam;
    [SerializeField] RenderObjects[] viewmodelRenderObjects;
    [SerializeField] float baseFOV;

    Transform camRotor;
    Vector2 ssRotation;

    public static float ViewmodelFOV { get; set; }
    public static float Zoom { get; set; }

    public PlayerCamera(GameObject context, Transform camRotor) : base(context)
    {
        this.camRotor = camRotor;
    }

    protected override void OnExecute()
    {
        ssRotation += Mouse.current.delta.ReadValue() * sensitivity;

        ssRotation.y = Mathf.Clamp(ssRotation.y, -90.0f, 90.0f);

        Context.transform.rotation = Quaternion.Euler(0.0f, ssRotation.x, 0.0f);
        camRotor.rotation = Quaternion.Euler(-ssRotation.y, ssRotation.x, 0.0f);

        Cursor.lockState = CursorLockMode.Locked;

        fpCam.m_Lens.FieldOfView = Mathf.Atan(Mathf.Tan(baseFOV * Mathf.Deg2Rad * 0.5f) / Zoom) * Mathf.Rad2Deg * 2.0f;
        foreach (var ro in viewmodelRenderObjects)
        {
            ro.settings.cameraSettings.cameraFieldOfView = ViewmodelFOV;
        }

        Zoom = 1.0f;
    }
}
