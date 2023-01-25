using UnityEngine;


[SelectionBase]
[DisallowMultipleComponent]
public sealed class Signal : MonoBehaviour
{
    public event System.Action OnRaise;

    public void Raise ()
    {
        OnRaise?.Invoke();
    }
}
