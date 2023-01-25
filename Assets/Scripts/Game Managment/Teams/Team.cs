using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


[SelectionBase]
[DisallowMultipleComponent]
public sealed class Team
{
    [SerializeField] string teamName;
    [SerializeField] List<Team> friendlyTeams;
    [SerializeField] List<Team> hostileTeams;

    NetworkList<NetworkObjectReference> members;
    public int ID { get; private set; }

    public void RegisterMember(NetworkObjectReference memberReference)
    {
        if (memberReference.TryGet(out NetworkObject member))
        {
            if (members.Contains(member)) return;
            Debug.Log($"Added \"{member.gameObject.name}\" from Team \"{teamName}\"");
            members.Add(member);
        }
    }

    public void DeregisterMember (NetworkObjectReference memberReference)
    {
        if (memberReference.TryGet(out NetworkObject member))
        {
            if (!members.Contains(member)) return;
            Debug.Log($"Removed \"{member.gameObject.name}\" from Team \"{teamName}\"");
            members.Remove(member);
        }
    }
}
