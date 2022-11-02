using Riptide;
using Riptide.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerManager : NetworkManager
{
    public static ServerManager Singleton { get; private set; }
    public Server server { get; private set; }

    public ushort port => 7777;
    public ushort maxClientsConnected => 1;

    private void Awake()
    {
        Singleton = this;

        Application.runInBackground = true;

        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);

        server = new();
        server.Start(port, maxClientsConnected);
        CurrentTick = 0;

        server.ClientDisconnected += PlayerLeft;
    }
    void PlayerLeft(object sender, ServerDisconnectedEventArgs e)
    {
        
    }
    private void FixedUpdate()
    {
        server.Update();

        CurrentTick++;
    }
    private void OnApplicationQuit() => server.Stop();



}
