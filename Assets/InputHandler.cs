using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }
}
