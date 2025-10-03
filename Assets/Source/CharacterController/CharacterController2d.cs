using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(MovementComponent), typeof(Rigidbody2D))]
public class CharacterController2d : MonoBehaviour
{
    public class RuntimeVariables
    {
        public bool CanJump => UsedJumps < MaxJumps;
        public int UsedJumps = 0;
        public int MaxJumps = 1;
        public DateTime TimeLastLeftGround = DateTime.MinValue;
    }
    public struct FrameInput
    {
        public bool JumpPressedThisFrame;
        public DateTime JumpPressedTime;
        public bool JumpHeld;
        public Vector2 Direction;
    }

    public PlayerVariables PlayerVariables;
    //---------

    public MovementComponent MovementComponent;
    private Rigidbody2D _rb;

    public FrameInput CurrentFrameInput = default;


    public PlayerCharacterStateMachine CharacterStateMachine;


    public RuntimeVariables RuntimeVars = new();


    private void Awake()
    {
        MovementComponent = GetComponent<MovementComponent>();
        CharacterStateMachine.SetAgent(this);
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        CharacterStateMachine.SetNextState(new StateChangeRequest(typeof(PlayerGroundedState)));
    }

    private void Update()
    {
        CurrentFrameInput.JumpPressedThisFrame = PlayerVariables.JumpInput.action.WasPressedThisFrame();
        if (CurrentFrameInput.JumpPressedThisFrame)
        {
            CurrentFrameInput.JumpPressedTime = DateTime.Now;
        }
        CurrentFrameInput.JumpHeld = PlayerVariables.JumpInput.action.IsPressed();
        CurrentFrameInput.Direction = PlayerVariables.MoveInput.action.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        MovementComponent.PhysicsMove(_rb);
    }

}
