using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public struct LobbyData : INetworkSerializable, IEquatable<LobbyData>
{
    public int LobbyId { get; set; }
    public int PlayerCount { get; set; }

    public LobbyData(int lobbyId, int playerCount)//, string ip )
    {
        LobbyId = lobbyId;
        PlayerCount = playerCount;
        //hostIp = System.Text.Encoding.Default.GetBytes(ip);

    }

    public bool Equals(LobbyData other)
    {
        throw new NotImplementedException();
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        throw new NotImplementedException();
    }
}

