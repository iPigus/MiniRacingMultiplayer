using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownCarController : MonoBehaviour
{
    [Header("Car Settings")]
    [Range(0f, 100f)]
    public float Accerelation = 30f;
    [Range(0f, 10f)]
    public float TurnFactor = 3.5f;
    [Range(0f, 1f)]
    public float DriftFactor = .95f;
    [Range(0f, 100f)]
    public float MaxSpeed = 20f;
    [Range(0f, 100f)]
    public float MaxSpeedInReverse = 2f;

    public float accerelationInput { get; private set; } = 0;
    public float steeringInput { get; private set; } = 0;

    float rotationAngle = 0;

    public float velocityVsUp { get; private set; } = 0;

    Rigidbody2D Rigidbody;

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();

        rotationAngle = Rigidbody.rotation;
    }
    private void FixedUpdate()
    {
        ApplyEngineForce();

        KillOrthogonalVelocity();

        ApplySteering();
    }
    void ApplyEngineForce()
    {
        velocityVsUp = Vector2.Dot(transform.up, Rigidbody.velocity);

        if (velocityVsUp > MaxSpeed && accerelationInput > 0) return;

        if (velocityVsUp < -MaxSpeedInReverse && accerelationInput < 0) return;

        if (Rigidbody.velocity.sqrMagnitude > MaxSpeed * MaxSpeed && accerelationInput > 0) return;

        if (accerelationInput == 0)
        {
            Rigidbody.drag = Mathf.Lerp(Rigidbody.drag, 2.0f, Time.fixedDeltaTime * 2f);
        }
        else
        {
            Rigidbody.drag = 0;
        }
        

        Vector2 engineForce = transform.up * accerelationInput * Accerelation;

        Rigidbody.AddForce(engineForce, ForceMode2D.Force);
    }
    void ApplySteering()
    {
        float minSpeedBeforeTurningFactor = Rigidbody.velocity.magnitude / 8;
        minSpeedBeforeTurningFactor = Mathf.Clamp01(minSpeedBeforeTurningFactor); 

        rotationAngle -= steeringInput * TurnFactor * minSpeedBeforeTurningFactor;

        Rigidbody.MoveRotation(rotationAngle);
    }

    public void SetInput(Vector2 inputVector)
    {
        steeringInput = inputVector.y >= 0 ? inputVector.x : -inputVector.x;
        accerelationInput = inputVector.y; 
    }
    void KillOrthogonalVelocity()
    {
        Vector2 forwardVelocity = transform.up * Vector2.Dot(Rigidbody.velocity, transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(Rigidbody.velocity, transform.right);

        Rigidbody.velocity = forwardVelocity + rightVelocity * DriftFactor;
    }
}
