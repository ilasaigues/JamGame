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