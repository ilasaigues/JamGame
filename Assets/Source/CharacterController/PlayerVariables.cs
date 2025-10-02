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
    public float Gravity;
    [Header("Jump")]
    public float JumpSpeed;
    public float JumpGravity;
    public float JumpMinTime;
    public float JumpMaxTime;
    [Header("Input")]
    public InputActionReference JumpInput;
    public InputActionReference MoveInput;
}