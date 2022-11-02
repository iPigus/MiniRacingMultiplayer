using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CarController : MonoBehaviour
{
    Rigidbody2D Rigidbody;

    Vector2 velocity
    {
        get => Rigidbody.velocity;
        set => Rigidbody.velocity = value;
    }
    float speed => velocity.magnitude;

    float MaxVelocity = 100;
    float Accerelation = 1;
    float TurnAngle = 1;


    bool[] ForwardBack = new bool[2];
    bool[] LeftRight = new bool[2];
    bool brake = false;

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) ForwardBack[0] = true;
        
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) ForwardBack[1] = true;
        
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) LeftRight[0] = true; 
        
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) LeftRight[1] = true;

        if (Input.GetKey(KeyCode.Space)) brake = true;
    }
    private void FixedUpdate()
    {
        CarPhysics();
        Movement(ForwardBack,LeftRight, brake);

        ForwardBack.ToList().ForEach(x => x = false);
        LeftRight.ToList().ForEach(x => x = false);
        brake = false;
    }

    void Movement(bool[] inputs, bool[] LeftRight, bool isBrake)
    {
        if (isBrake) velocity *= .98f;
        else
        {
            if (inputs[0] && !inputs[1])
            {
                velocity += (Vector2)transform.forward * Time.deltaTime * Accerelation;
            }
            if (!inputs[0] && inputs[1])
            {
                velocity -= (Vector2)transform.forward * Time.deltaTime * Accerelation * .3f;
            }
        }
    }

    void CarPhysics()
    {
        if (speed != 0)
        {
            velocity *= .95f;
        }
    }
}
