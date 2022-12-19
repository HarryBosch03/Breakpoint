using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[SelectionBase]
[DisallowMultipleComponent]
public sealed class Projectile : MonoBehaviour
{
    [SerializeField] GameObject impactPrefab;

    const float minY = -100.0f;

    new Rigidbody rigidbody;

    public float Speed { get; set; }
    public float Damage { get; set; }

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        rigidbody.velocity = transform.forward * Speed;
    }

    private void FixedUpdate()
    {
        float speed = rigidbody.velocity.magnitude;

        if (Physics.Raycast(rigidbody.position, rigidbody.velocity, out var hit, speed * Time.deltaTime + 0.02f, 0b1))
        {
            DestroyWithStyle(hit.point, hit.normal);
        }

        if (rigidbody.position.y < minY)
        {
            DestroyWithStyle();
        }
    }

    private void DestroyWithStyle() => DestroyWithStyle(transform.position, transform.forward);
    private void DestroyWithStyle(Vector3 impactPoint, Vector3 impactDirection)
    {
        Instantiate(impactPrefab, impactPoint, Quaternion.LookRotation(impactDirection));

        Destroy(gameObject);
    }
}