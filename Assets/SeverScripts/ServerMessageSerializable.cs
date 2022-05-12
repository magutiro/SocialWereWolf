using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public struct ServerMessageSerializable : INetworkSerializable
{
    int myInt;
    int myString;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref myInt);
        serializer.SerializeValue(ref myString);
    }
}
