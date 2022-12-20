using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.Experimental.Rendering.Universal;

[System.Serializable]
public sealed class PlayerCamera : NestedBehaviour
{
    [SerializeField] float sensitivity = 0.3f;
    [SerializeField] CinemachineVirtualCamera fpCam;
    [SerializeField] RenderObjects[] viewmodelRenderObjects;
    [SerializeField] float baseFOV = 100.0f;

    Transform camRotor;
    Vector2 ssRotation;

    public static float FOVOverride { get; set; }
    public static float FOVOverrideBlend { get; set; }
    public static float ViewmodelFOV { get; set; }
    
    public PlayerCamera(MonoBehaviour context, Transform camRotor, CinemachineVirtualCamera fpCam, RenderObjects[] viewmodelRenderObjects) : base(context)
    {
        this.camRotor = camRotor;
        this.fpCam = fpCam;
        this.viewmodelRenderObjects = viewmodelRenderObjects;
    }

    protected override void OnExecute()
    {
        ssRotation += Mouse.current.delta.ReadValue() * sensitivity;

        ssRotation.y = Mathf.Clamp(ssRotation.y, -90.0f, 90.0f);

        Context.transform.rotation = Quaternion.Euler(0.0f, ssRotation.x, 0.0f);
        camRotor.rotation = Quaternion.Euler(-ssRotation.y, ssRotation.x, 0.0f);

        Cursor.lockState = CursorLockMode.Locked;

        fpCam.m_Lens.FieldOfView = Mathf.Lerp(baseFOV, FOVOverride, FOVOverrideBlend);
        foreach (var ro in viewmodelRenderObjects)
        {
            ro.settings.cameraSettings.cameraFieldOfView = ViewmodelFOV;
        }

        FOVOverride = baseFOV;
    }

    public static void SetZoom(float zoom, float refFOV = 60.0f)
    {
        FOVOverride = Mathf.Atan(Mathf.Tan(refFOV * Mathf.Deg2Rad * 0.5f) / zoom) * Mathf.Rad2Deg * 2.0f;
    }
}
