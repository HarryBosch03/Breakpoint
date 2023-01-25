using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.InputSystem;

[SelectionBase]
[DisallowMultipleComponent]
public sealed class FreeCam : MonoBehaviour
{
    [SerializeField] float maxSpeed;
    [SerializeField] float acceleration;

    [Space]
    [SerializeField] float sensitivity;

    [SerializeField] InputActionAsset inputAsset;

    Vector3 velocity;
    Vector2 ssRotation;

    private void OnEnable()
    {
        inputAsset.Enable();
    }

    private void OnDisable()
    {
        inputAsset.Disable();
    }

    private void Update()
    {
        Vector2 planarInput = inputAsset.FindAction("move").ReadValue<Vector2>();
        float verticalInput = inputAsset.FindAction("verticalMove").ReadValue<float>();

        float sin = Mathf.Sin(ssRotation.x * Mathf.Deg2Rad);
        float cos = Mathf.Cos(ssRotation.x * Mathf.Deg2Rad);
        Vector3 target = new Vector3(planarInput.x * cos + planarInput.y * -sin, verticalInput, planarInput.y * cos + planarInput.x * -sin) * maxSpeed;
        Vector3 force = (target - velocity) * acceleration;

        velocity += force * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;

        if (Mouse.current != null) ssRotation += Mouse.current.delta.ReadValue() * sensitivity;
        transform.rotation = Quaternion.Euler(ssRotation.y, ssRotation.x, 0.0f);
    }
}
