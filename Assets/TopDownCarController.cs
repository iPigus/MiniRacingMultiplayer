using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownCarController : MonoBehaviour
{
    [Header("Car Settings")]
    public float Accerelation = 30f;
    public float TurnFactor = 3.5f;

    float accerelationInput = 0;
    float steeringInput = 0;

    float rotationAngle = 0;

    Rigidbody2D Rigidbody;

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        ApplyEngineForce();

        ApplySteering();
    }
    void ApplyEngineForce()
    {
        Vector2 engineForce = transform.up * accerelationInput * Accerelation;

        Rigidbody.AddForce(engineForce, ForceMode2D.Force);
    }
    void ApplySteering()
    {
        rotationAngle -= steeringInput * TurnFactor;

        Rigidbody.MoveRotation(rotationAngle);
    }

    public void SetInput(Vector2 inputVector)
    {
        steeringInput = inputVector.x;
        accerelationInput = inputVector.y; 
    }
}
