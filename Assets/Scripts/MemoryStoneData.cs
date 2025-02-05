using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public struct MemoryStoneData : INetworkSerializable
{
    public Powers power;
    public Color color;

    public MemoryStoneData(Powers power, Color color)
    {
        this.power = power;
        this.color = color;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref power);
        serializer.SerializeValue(ref color);
    }
}