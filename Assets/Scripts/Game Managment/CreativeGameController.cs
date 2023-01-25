using UnityEngine;


[SelectionBase]
[DisallowMultipleComponent]
public sealed class CreativeGameController : GameController
{
    protected override void Start()
    {
        base.Start();

        NetworkedPlayerController.SpawnAllPlayers();
    }
}
