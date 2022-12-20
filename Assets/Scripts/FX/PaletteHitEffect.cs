using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
[DisallowMultipleComponent]
[RequireComponent(typeof(Damagable))]
public sealed class PaletteHitEffect : MonoBehaviour
{
    [SerializeField] ParticleSystem[] fxPrefabs;
    [SerializeField] Vector2Int paletteIndex;

    Damagable damagable;

    private void Awake()
    {
        damagable = GetComponent<Damagable>();
    }

    private void OnEnable()
    {
        damagable.DamageEvent += OnDamage;
    }

    private void OnDisable()
    {
        damagable.DamageEvent -= OnDamage;
    }

    private void OnDamage(DamageArgs args)
    {
        foreach (var fxPrefab in fxPrefabs)
        {
            var instance = Instantiate(fxPrefab, args.point, Quaternion.LookRotation(args.normal));

            var main = instance.main;
            var startColor = main.startColor;
            startColor.color = new Color(paletteIndex.x / 16.0f, paletteIndex.y / 16.0f, 0.0f, 1.0f);
            main.startColor = startColor;
        }
    }
}
