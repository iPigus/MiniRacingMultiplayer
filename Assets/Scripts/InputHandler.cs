using Riptide;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NetworkManager;

public class InputHandler : MonoBehaviour
{
    TopDownCarController Controller;

    private void Awake()
    {
        Controller = GetComponent<TopDownCarController>();
    }


    void Update()
    {
        Vector2 inputVector = new();

        inputVector.x = Input.GetAxis("Horizontal");
        inputVector.y = Input.GetAxis("Vertical");

        Controller.SetInput(inputVector);

        if (isClient) SendInput(inputVector);
    }
    void SendInput(Vector2 inputVector)
    {
        Message message = Message.Create(MessageSendMode.Unreliable, ClientToServerId.playerInputs);

        message.AddVector2(inputVector);

        ClientManager.Singleton.client.Send(message);
    }
}
