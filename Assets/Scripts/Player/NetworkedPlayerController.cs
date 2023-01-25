using Unity.Netcode;
using UnityEngine;

[SelectionBase]
[DisallowMultipleComponent]
public sealed class NetworkedPlayerController : NetworkBehaviour
{
    [SerializeField] PlayerAvatar playerPrefab;
    [SerializeField] GameObject respawnScreen;

    PlayerAvatar avatarInstance;

    static event System.Action SpawnAllPlayersEvent;

    private void OnEnable()
    {
        SpawnAllPlayersEvent += SpawnPlayerServerRPC;
    }

    private void OnDisable()
    {
        SpawnAllPlayersEvent -= SpawnPlayerServerRPC;
    }

    public override void OnNetworkSpawn()
    {
        if (GameController.instance.GameSettings.freeRespawn && IsOwner)
        {
            SpawnPlayerServerRPC();
        }
    }

    [ServerRpc]
    public void SpawnPlayerServerRPC()
    {
        if (IsServer)
        {
            var spawnPoint = GameController.instance.GameSettings.GetSpawnPoint();
            RegisterNewAvatarInstance(avatarInstance = Instantiate(playerPrefab, spawnPoint, Quaternion.identity));
        }
    }

    private void OnPlayerDeath(DamageArgs args)
    {
        if (!IsOwner) return;

        respawnScreen.SetActive(true);
    }

    private void RegisterNewAvatarInstance(Component newInstance) => RegisterNewAvatarInstance(newInstance.GetComponent<NetworkObject>());
    private void RegisterNewAvatarInstance(NetworkObject newInstance)
    {
        if (!newInstance) return;

        newInstance.SpawnWithOwnership(OwnerClientId);
        SetPlayerInstanceClientRPC(newInstance.gameObject);

        if (avatarInstance.TryGetComponent(out SimpleHealth health))
        {
            health.DeathEvent += OnPlayerDeath;
        }
    }

    [ClientRpc]
    private void SetPlayerInstanceClientRPC(NetworkObjectReference playerInstanceReference)
    {
        if (playerInstanceReference.TryGet(out var instance))
        {
            avatarInstance = instance.GetComponent<PlayerAvatar>();
        }
        else
        {
            avatarInstance = null;
            CheckNoAvatarBehaviour();
        }

        if (IsOwner) respawnScreen.SetActive(false);


    }

    private void CheckNoAvatarBehaviour()
    {
        if (!IsOwner) return;

        switch (GameController.instance.GameSettings.noAvatarBehaviour)
        {
            default:
            case GameSettings.NoAvatarBehaviour.None:
                break;
            case GameSettings.NoAvatarBehaviour.FreeCam:
                break;
        }
    }

    public static void SpawnAllPlayers()
    {
        SpawnAllPlayersEvent?.Invoke();
    }
}
