using Unity.Mathematics;
using UnityEngine;

public class PlayerAirState : BaseState<CharacterController2d>
{
    Vector2 _velocity;
    protected override void EnterStateInternal(params StateConfig.IBaseStateConfig[] configs)
    {
        InitFromConfigs(configs);
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
    }

    protected override void FixedUpdateStateInternal(float delta)
    {

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
}