using System;
using Unity.Mathematics;
using UnityEngine;

public class PlayerAirState : BaseState<CharacterController2d>
{
    Vector2 _velocity;
    protected override void EnterStateInternal(params StateConfig.IBaseStateConfig[] configs)
    {
        InitFromConfigs(configs);
        Agent.SetAnimationFlag(CharacterController2d.AnimationParameters.Grounded, false);
    }

    private void InitFromConfigs(params StateConfig.IBaseStateConfig[] configs)
    {
        foreach (var config in configs)
        {
            if (config is StateConfig.StartingVelocityConfig velocityConfig)
            {
                _velocity = velocityConfig.Velocity;
            }
        }
    }

    protected override void ExitStateInternal()
    {

    }
    protected override void UpdateStateInternal(float delta)
    {
        if (IsGrounded())
        {
            ChangeState<PlayerGroundedState>();
            return;
        }
        if (CheckCanJump())
        {
            ChangeState<PlayerJumpState>(new StateConfig.StartingVelocityConfig(new Vector2(_velocity.x, Agent.PlayerVariables.JumpSpeed)));
            return;
        }
    }

    protected override void FixedUpdateStateInternal(float delta)
    {
        // vertical movement
        _velocity.y = StateBehaviour.CalculateVerticalVelocity(
           _velocity.y,
           Agent.PlayerVariables.MaxAirVelocity,
           GetGravity,
           delta);
        // horizontalMovement
        _velocity.x = StateBehaviour.CalculateHorizontalVelocity(
            _velocity.x,
            Agent.CurrentFrameInput.Direction.x * Agent.PlayerVariables.AirSpeed,
            Agent.PlayerVariables.AirAcceleration,
            Agent.PlayerVariables.AirDeceleration,
            delta);
        _velocity.x = Mathf.Clamp(_velocity.x, -Agent.PlayerVariables.AirSpeed, Agent.PlayerVariables.AirSpeed);

        Agent.MovementComponent.SetVelocity(_velocity);

        HandleAnimation();
    }

    private void HandleAnimation()
    {
        Agent.SetAnimationFlag(CharacterController2d.AnimationParameters.Rising, _velocity.y > 0);
    }

    float GetGravity() => _velocity switch
    {
        { y: var Y } when Y >= Agent.PlayerVariables.JumpGravityThreshold => Agent.PlayerVariables.JumpGravity,
        { y: var Y } when Y >= Agent.PlayerVariables.ApexGravityThreshold => Agent.PlayerVariables.ApexGravity,
        { y: _ } => Agent.PlayerVariables.Gravity,
    };

    private bool IsGrounded()
    {
        return Agent.MovementComponent.IsAgainstGround;
    }
    private bool CheckCanJump()
    {
        return Agent.RuntimeVars.CanJump &&
            Agent.CurrentFrameInput.JumpPressedThisFrame &&
            (Agent.RuntimeVars.MaxJumps > 1 ||
            (DateTime.Now - Agent.RuntimeVars.TimeLastLeftGround).TotalSeconds <= Agent.PlayerVariables.CoyoteTime);
    }
}