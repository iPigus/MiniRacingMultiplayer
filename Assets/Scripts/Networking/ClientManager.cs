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
        Debug.Log("Creating ClientManager!");

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

        ClientModeStatus.SetText("Connecting");

        if (client == null) client = new Riptide.Client();

        client.Connect($"{ip}:{port}");
    }
    private void OnApplicationQuit() => client.Disconnect();
    private void OnDestroy() => client.Disconnect();
    void DidConnect(object sender, EventArgs e)
    {
        SceneManager.LoadScene(4);

        SendPlayerInfo();
    }
    public void SendPlayerInfo()
    {
        Message message = Message.Create(MessageSendMode.Reliable, ClientToServerId.playerInfo);

        message.AddInt(PlayerPrefs.GetInt("CarChosen"));

        client.Send(message);
    }
    void FailedToConnect(object sender, EventArgs e)
    {
        ClientModeStatus.SetText("Failed to connect!");
        Debug.Log("Failed to connect!");
    }

    IEnumerator ConnectAfterTime(float time)
    {
        yield return new WaitForSecondsRealtime(time);

        ClientModeStatus.SetText("Client Mode");
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

        GameTimer.Singleton.StartCountdown();
    }
}
