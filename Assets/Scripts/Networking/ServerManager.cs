using Riptide;
using Riptide.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ServerManager : NetworkManager
{
    public static ServerManager Singleton { get; private set; }
    public Server server { get; private set; }

    public ushort port => 7777;
    public ushort maxClientsConnected => 1;

    private void Awake()
    {
        Debug.Log("Creating ServerManager!");

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
        server.ClientConnected += PlayerConneted;
    }

    List<Message> messagesToSendAtSpawn = new();

    private void PlayerConneted(object sender, ServerConnectedEventArgs e)
    {
        SendStartGame();
        GameTimer.Singleton.StartCountdown();

        messagesToSendAtSpawn.ToList().ForEach(x => server.SendToAll(x));
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

    void SendStartGame()
    {
        Message message = Message.Create(MessageSendMode.Reliable, ServerToClientId.gameStart);

        server.SendToAll(message);
    }
    public static void SetCarInfo(ushort playerId, ushort carId, Vector2 carPos, float carZrotation)
    {
        Message message = Message.Create(MessageSendMode.Reliable, ServerToClientId.carInfos);

        message.AddUShort(playerId);
        message.AddUShort(carId);
        message.AddVector2(carPos);
        message.AddFloat(carZrotation);

        Singleton.messagesToSendAtSpawn.Add(message);
    }
    [MessageHandler((ushort)ClientToServerId.playerInfo)]
    public static void RecivingPlayerInfo(Message message)
    {
        int carChosen = message.GetInt();

        CarSpawner.Singleton.SpawnClientCar(carChosen);
    }
}
