using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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


    }
    
}
