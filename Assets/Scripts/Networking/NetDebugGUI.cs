using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


[SelectionBase]
[DisallowMultipleComponent]
public sealed class NetDebugGUI : MonoBehaviour
{
    private void OnGUI()
    {
        if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsClient) 
        {
            enabled = false;
            return;
        }

        if (GUI.Button(new Rect(5, 5, 125, 25), "Host"))
        {
            NetworkManager.Singleton.StartHost();
        }

        if (GUI.Button(new Rect(5, 35, 125, 25), "Server"))
        {
            NetworkManager.Singleton.StartServer();
        }

        if (GUI.Button(new Rect(5, 65, 125, 25), "Client"))
        {
            NetworkManager.Singleton.StartClient();
        }
    }
}
