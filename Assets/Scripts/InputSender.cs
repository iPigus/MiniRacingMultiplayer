using Riptide;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static NetworkManager;

public class InputSender : MonoBehaviour
{
    private void Awake()
    {
        if (FindObjectOfType<ServerManager>()) 
            Destroy(gameObject);
    }
    private void Update()
    {
        Vector2 inputVector = new();

        inputVector.x = Input.GetAxis("Horizontal");
        inputVector.y = Input.GetAxis("Vertical");

        SendInputs(inputVector);
    }

    public void SendInputs(Vector2 input)
    {
        Message message = Message.Create(MessageSendMode.Unreliable, ClientToServerId.playerInputs);

        message.AddVector2(input);
    }
}
