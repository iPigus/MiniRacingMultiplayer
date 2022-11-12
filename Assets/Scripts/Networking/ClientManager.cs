using Riptide;
using Riptide.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientManager : NetworkManager
{
    public static ClientManager Singleton { get; private set; }

    public Client client { get; private set; }

    ushort port => 7777;
    string ip => PlayerPrefs.GetString("ip");
    public ushort TickDivergenceTolerance { get; private set; } = 1;
    public bool isTryingToConnect { get; private set; } = false;
    public ushort TickPing { get; private set; } = 0;
    public float Ping { get; private set; } = 0;
    public ushort lagCall { get; set; } = 1;

    private void Awake()
    {
        Singleton = this;
        DontDestroyOnLoad(this);
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);

        client = new Riptide.Client();
        CurrentTick = 0;

        client.Connected += DidConnect;
        client.ConnectionFailed += FailedToConnect;
        client.Disconnected += DidDisconnect;
    }
    private void FixedUpdate()
    {
        client.Update();

        CurrentTick++;
    }
    public void Connect()
    {
        Debug.Log("Trying to connect at: " + $"{ip}:{port}"); isTryingToConnect = true;

        if (client == null) client = new Riptide.Client();

        client.Connect($"{ip}:{port}");
    }
    private void OnApplicationQuit()
    {
        client.Disconnect();
    }
    private void OnDestroy()
    {
        client.Disconnect();
    }
    void DidConnect(object sender, EventArgs e)
    {
        SceneManager.LoadScene(4);
    }
    void FailedToConnect(object sender, EventArgs e)
    {
        Debug.Log("Failed to connect!");
    }
    void DidDisconnect(object sender, EventArgs e)
    {
        SceneManager.LoadScene(0);
        Destroy(this.gameObject);
    }

    [MessageHandler((ushort)ServerToClientId.gameStart)]
    public static void RecivingGameStartData(Message message)
    {
        if (SceneManager.GetActiveScene().buildIndex != 4) SceneManager.LoadScene(4);

        
    }
}
