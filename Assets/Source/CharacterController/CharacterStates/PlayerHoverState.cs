
using UnityEngine;

public class PlayerHoverState : BaseState<CharacterController2d>
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
        _velocity.y = 0;
        // horizontalMovement
        _velocity.x = StateBehaviour.CalculateHorizontalVelocity(
            _velocity.x,
            Agent.CurrentFrameInput.Direction.x * Agent.PlayerVariables.AirSpeed,
            Agent.PlayerVariables.AirAcceleration,
            Agent.PlayerVariables.AirDeceleration,
            delta);
        _velocity.x = Mathf.Clamp(_velocity.x, -Agent.PlayerVariables.AirSpeed, Agent.PlayerVariables.AirSpeed);

        Agent.MovementComponent.SetVelocity(_velocity);
    }

    private bool IsGrounded()
    {
        return Agent.MovementComponent.IsAgainstGround;
    }
}