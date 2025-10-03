using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu]
public class PlayerVariables : ScriptableObject
{
    [Header("Ground")]
    public float GroundSpeed;
    public float GroundAcceleration;
    public float GroundDeceleration;
    [Header("Air")]
    public float AirSpeed;
    public float AirAcceleration;
    public float AirDeceleration;
    public float MaxAirVelocity;
    [Header("Jump")]
    public float JumpSpeed;
    public float BufferTime;
    public float CoyoteTime;
    [Header("Dash")]
    public float DashVelocity;
    public float DashDistance;

    [Header("Gravities")]
    public float JumpGravity;
    public float JumpGravityThreshold;
    public float ApexGravity;
    public float ApexGravityThreshold;
    public float Gravity;
    public float StoneSkinGravity;
    [Header("Input")]
    public InputActionReference JumpInput;
    public InputActionReference MoveInput;
    public InputActionReference SpecialInput;
}