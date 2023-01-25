using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Experimental.Rendering.Universal;
using Cinemachine;

[System.Serializable]
public sealed class PlayerCamera : NestedBehaviour
{
    [SerializeField] float sensitivity = 0.3f;
    [SerializeField] CinemachineVirtualCamera camDriver;
    [SerializeField] RenderObjects[] viewmodelRenderObjects;
    [SerializeField] float baseFOV = 100.0f;

    [Space]
    [SerializeField] float recoilDecay = 15.0f;

    public Vector2 SSRotation { get; set; }
    Vector2 recoilForce;

    public float FOVOverride { get; set; }
    public float FOVOverrideBlend { get; set; }
    public float ViewmodelFOV { get; set; }
    
    private void OnAddRecoil(Vector2 force)
    {
    }

    public void Process(MonoBehaviour caller, Transform camRotor, bool owner)
    {
        if (owner)
        {
            SSRotation += Mouse.current.delta.ReadValue() * sensitivity * camDriver.m_Lens.FieldOfView / baseFOV;
            Cursor.lockState = CursorLockMode.Locked;
            foreach (var ro in viewmodelRenderObjects)
            {
                ro.settings.cameraSettings.cameraFieldOfView = ViewmodelFOV;
            }
        }

        SSRotation = new Vector2(SSRotation.x, Mathf.Clamp(SSRotation.y, -90.0f, 90.0f));

        caller.transform.rotation = Quaternion.Euler(0.0f, SSRotation.x, 0.0f);
        camRotor.rotation = Quaternion.Euler(-SSRotation.y, SSRotation.x, 0.0f);

        camDriver.m_Lens.FieldOfView = Mathf.Lerp(baseFOV, FOVOverride, FOVOverrideBlend);
        
        FOVOverride = baseFOV;

        recoilForce -= recoilForce * Mathf.Min(1.0f, recoilDecay * Time.deltaTime);
        SSRotation += recoilForce * Time.deltaTime;
    }

    public void SetZoom(float zoom, float refFOV = 60.0f)
    {
        FOVOverride = Mathf.Atan(Mathf.Tan(refFOV * Mathf.Deg2Rad * 0.5f) / zoom) * Mathf.Rad2Deg * 2.0f;
    }

    public void AddRecoil (Vector2 force)
    {
        recoilForce += force;
    }
}
