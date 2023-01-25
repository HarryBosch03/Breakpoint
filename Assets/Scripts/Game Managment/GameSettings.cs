using UnityEngine;
using UnityEngine.Rendering.UI;

[System.Serializable]
public sealed class GameSettings
{
    public bool freeRespawn;
    public float respawnDelay;
    public NoAvatarBehaviour noAvatarBehaviour;
    public FriendlyFireBehaviour friendlyFireBehaviour;
    public Transform defaultSpawnPointContainer;

    public Vector3 GetSpawnPoint() => GetSpawnPoint(Random.Range(0, defaultSpawnPointContainer.childCount));
    public Vector3 GetSpawnPoint (int index)
    {
        if (index < 0 || index >= defaultSpawnPointContainer.childCount)
        {
            return default;
        }

        return defaultSpawnPointContainer.GetChild((int)Mathf.Repeat(index, defaultSpawnPointContainer.childCount)).position;
    }

    public enum NoAvatarBehaviour
    {
        None,
        FreeCam,
    }

    public enum FriendlyFireBehaviour
    {
        Never,
        Always,
    }
}
