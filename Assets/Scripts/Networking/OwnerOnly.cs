using Unity.Netcode;
using UnityEngine;

[SelectionBase]
[DisallowMultipleComponent]
public sealed class OwnerOnly : NetworkBehaviour
{
    [SerializeField] bool invert;

    public override void OnNetworkSpawn()
    {
        gameObject.SetActive(NetworkObject.IsOwner == !invert);
    }
}
