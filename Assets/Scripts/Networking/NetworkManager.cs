using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NetworkManager : MonoBehaviour
{
    public ushort CurrentTick { get; protected set; }
    public enum ClientToServerId : ushort 
    { 
        playerInfo,
        playerInputs
    }
    public enum ServerToClientId : ushort
    {
        gameStart,
        carPositions,
        carPhysicsData,
        placements,
        carInfos // for spawn
    }  

    public static NetworkManager GetNetworkManager => FindObjectOfType<NetworkManager>();

    public static bool isServer => GetNetworkManager is ServerManager;
    public static bool isClient => GetNetworkManager is ClientManager;
}
