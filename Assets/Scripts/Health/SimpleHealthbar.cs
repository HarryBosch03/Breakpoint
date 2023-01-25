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
        Color materialMeta = new Color(target.MaxHealth / 2550.0f, 0.1f, 0.0f, 1.0f);
        fill.color = materialMeta;
        lagFill.color = materialMeta;
        bgFill.color = materialMeta;
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

        fill.fillAmount = target.NormalizedHealth;
    }
}
