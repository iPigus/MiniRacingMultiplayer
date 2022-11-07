using Riptide;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NetworkManager;

public class CarNetwork : MonoBehaviour
{
    public static Dictionary<ushort, CarNetwork> list = new();

    public ushort CarId { get; set; }   // 0 - server

    Rigidbody2D Rigidbody;
    TopDownCarController CarController;

    Vector2 lastPosition = new();

    private void Awake()
    {                                               
        Rigidbody = GetComponent<Rigidbody2D>();
        CarController = GetComponent<TopDownCarController>();

        CarId = (ushort)list.Count;

        list.Add(CarId,this);
    }

    private void FixedUpdate()
    {
        if (lastPosition == Rigidbody.position || !isServer) return;
        
        lastPosition = Rigidbody.position;

        if (isServer)
        {
            SendCarPosition();
            SendCarData();
        }
    }
    void SendCarPosition()
    {
        Message message = Message.Create(MessageSendMode.Unreliable, ServerToClientId.carPositions);

        message.AddUShort(CarId);

        message.AddVector2(Rigidbody.position);
        message.AddFloat(Rigidbody.rotation);

        ServerManager.Singleton.server.SendToAll(message);
    }

    void SendCarData()
    {
        Message message = Message.Create(MessageSendMode.Unreliable, ServerToClientId.carPhysicsData);

        message.AddUShort(CarId);

        message.AddVector2(Rigidbody.velocity);
        message.AddFloat(CarController.accerelationInput);
        message.AddFloat(CarController.steeringInput);
        message.AddFloat(CarController.velocityVsUp);

        ServerManager.Singleton.server.SendToAll(message);
    }

    [MessageHandler((ushort)ServerToClientId.carPositions)]
    public static void SetCarPosition(Message message)
    {
        if (list.TryGetValue(message.GetUShort(), out CarNetwork car))
        {
            car.Rigidbody.position = message.GetVector2();
            car.Rigidbody.rotation = message.GetFloat();
        }
    }

    [MessageHandler((ushort)ServerToClientId.carPhysicsData)]
    public static void SetCarPhysicsData(Message message)
    {
        if (list.TryGetValue(message.GetUShort(), out CarNetwork car))
        {
            car.Rigidbody.velocity = message.GetVector2();
            car.CarController.accerelationInput = message.GetFloat();
            car.CarController.steeringInput = message.GetFloat();
            car.CarController.velocityVsUp = message.GetFloat();
        }
    }
}
