using Riptide;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NetworkManager;

public class CarNetwork : MonoBehaviour
{
    public ushort CarId { get; set; }   // 0 - server

    Rigidbody2D Rigidbody;
    TopDownCarController CarController;

    Vector2 lastPosition = new();

    private void Awake()
    {                                               
        Rigidbody = GetComponent<Rigidbody2D>();
        CarController = GetComponent<TopDownCarController>();
    }

    private void FixedUpdate()
    {
        if (lastPosition == Rigidbody.position || !isServer) return;
        
        lastPosition = Rigidbody.position;

        SendCarPosition();

        SendCarData();
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
}
