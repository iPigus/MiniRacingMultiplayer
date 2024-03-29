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
    bool haveSendMessages = false;

    private void PlayerConneted(object sender, ServerConnectedEventArgs e)
    {
        SendStartGame();
        GameTimer.Singleton.StartCountdown();

        messagesToSendAtSpawn.ToList().ForEach(x => server.SendToAll(x));
        haveSendMessages = true;
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
    private void OnDestroy() => server.Stop();

    void SendStartGame()
    {
        Message message = Message.Create(MessageSendMode.Reliable, ServerToClientId.gameStart);

        server.SendToAll(message);
    }
    public static void SendCarInfo(ushort playerId, ushort carId, Vector2 carPos, float carZrotation)
    {
        Message message = Message.Create(MessageSendMode.Reliable, ServerToClientId.carInfos);

        message.AddUShort(playerId);
        message.AddUShort(carId);
        message.AddVector2(carPos);
        message.AddFloat(carZrotation);

        if (!Singleton.haveSendMessages)
            Singleton.messagesToSendAtSpawn.Add(message);
        else
            Singleton.server.SendToAll(message);
    }
    [MessageHandler((ushort)ClientToServerId.playerInfo)]
    public static void RecivingPlayerInfo(ushort fromPlayerId,Message message)
    {
        int carChosen = message.GetInt();

        CarSpawner.Singleton.SpawnClientCar(carChosen);
    }

    [MessageHandler((ushort)ClientToServerId.playerInputs)]
    public static void RecivingPlayerInputs(ushort fromPlayerId, Message message)
    {
        if (CarNetwork.list.TryGetValue(fromPlayerId, out CarNetwork car))
        {
            car.SetCarInput(message.GetVector2());
        }
    }
}
