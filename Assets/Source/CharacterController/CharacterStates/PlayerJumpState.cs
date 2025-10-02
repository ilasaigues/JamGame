using System;
using UnityEngine;

public class PlayerJumpState : BaseState<CharacterController2d>
{
    Vector2 _velocity;
    bool _jumpHeld = false;

    protected override void EnterStateInternal(params StateConfig.IBaseStateConfig[] configs)
    {
        InitFromConfigs(configs);
        _jumpHeld = true;
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
            ChangeState<PlayerGroundedState>(new StateConfig.StartingVelocityConfig(_velocity));
            return;
        }
        if (ReachedApex())
        {
            ChangeState<PlayerAirState>(new StateConfig.StartingVelocityConfig(_velocity));
        }
    }

    protected override void FixedUpdateStateInternal(float delta)
    {

        // if we stop holding jump, we set this flag to false
        _jumpHeld &= Agent.CurrentFrameInput.JumpHeld;

        // vertical movement
        _velocity.y += GetGravity() * delta;
        if (Mathf.Abs(_velocity.y) > Agent.PlayerVariables.MaxAirVelocity)
        {
            _velocity.y = Agent.PlayerVariables.MaxAirVelocity * Mathf.Sign(_velocity.y);
        }

        // horizontalMovement
        float targetHorizontalVelocity;
        float acceleration;

        if (Agent.CurrentFrameInput.Direction.x != 0) // moving, accelerate to target speed
        {
            targetHorizontalVelocity = Agent.CurrentFrameInput.Direction.x * Agent.PlayerVariables.AirSpeed;
            acceleration = Agent.PlayerVariables.AirAcceleration;
        }
        else // not moving, decelerate to zero
        {
            targetHorizontalVelocity = 0;
            acceleration = Agent.PlayerVariables.GroundDeceleration;
        }

        var diff = targetHorizontalVelocity - _velocity.x;

        if (Mathf.Abs(diff) > acceleration) // if we won't go over the target speed, accelerate
        {
            _velocity.x += Mathf.Sign(diff) * acceleration * delta;
        }
        else // if we would go over the target speed, just set it to the target speed
        {
            _velocity.x = targetHorizontalVelocity;
        }


        Agent.MovementComponent.SetVelocity(MovementComponent.VelocityType.MainMovement, _velocity);
    }

    float GetGravity()
    {
        if (_jumpHeld)
        {
            return Agent.PlayerVariables.JumpGravity;
        }
        else
        {
            return Agent.PlayerVariables.Gravity;
        }
    }

    private bool ReachedApex()
    {
        return _velocity.y <= Agent.PlayerVariables.ApexGravityThreshold;
    }

    private bool IsGrounded()
    {
        return _velocity.y < 0 && Agent.MovementComponent.IsAgainstGround;
    }
}