using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[SelectionBase]
[DisallowMultipleComponent]
public class SimpleHealthbar : MonoBehaviour
{
    [SerializeField] Image fill;
    [SerializeField] Image lagFill;
    [SerializeField] Image bgFill;
    [SerializeField] float lagHangTimeSecconds;
    [SerializeField] float lagDecay;

    SimpleHealth target;

    private void Awake()
    {
        target = GetComponentInParent<SimpleHealth>();
    }

    private void OnEnable()
    {
        target.DamageEvent += _ => UpdateHealthbar();

        if (target is Health)
        {
            ((Health)target).RegenEvent += UpdateHealthbar;
        }

        Color materialMeta = new Color(target.MaxHealth / 2550.0f, 0.1f, 0.0f, 1.0f);
        fill.color = materialMeta;
        lagFill.color = materialMeta;
        bgFill.color = materialMeta;
    }

    private void OnDisable()
    {
        target.DamageEvent -= _ => UpdateHealthbar();

        if (target is Health)
        {
            ((Health)target).RegenEvent -= UpdateHealthbar;
        }
    }

    private void Update()
    {
        if (lagFill)
        {
            float t = Time.time - (target.LastDamageTime + lagHangTimeSecconds);
            if (t > 0.0f)
            {
                lagFill.fillAmount += (fill.fillAmount - lagFill.fillAmount) * Mathf.Min(lagDecay * Time.deltaTime, 1.0f);
            }
        }
    }

    protected virtual void UpdateHealthbar()
    {
        fill.fillAmount = target.NormalizedHealth;
    }
}
