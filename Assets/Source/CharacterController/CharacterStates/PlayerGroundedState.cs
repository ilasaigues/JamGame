using AstralCore;
using UnityEngine;

public class PlayerGroundedState : BaseState<CharacterController2d>
{

    float _horizontalVelocity;

    protected override void EnterStateInternal(params StateConfig.IBaseStateConfig[] configs)
    {
        InitFromConfigs(configs);
        Agent.MovementComponent.SetVelocity(MovementComponent.VelocityType.Gravity, Vector2.zero);
        _horizontalVelocity = Agent.MovementComponent.GetVelocity(MovementComponent.VelocityType.MainMovement).x;
    }

    private void InitFromConfigs(params StateConfig.IBaseStateConfig[] configs)
    {
        foreach (var config in configs)
        {
            if (config is StateConfig.StartingVelocityConfig velocityConfig)
            {
                _horizontalVelocity = velocityConfig.Velocity.x;
            }
        }
    }

    protected override void ExitStateInternal()
    {
    }

    protected override void UpdateStateInternal(float delta)
    {
        if (!IsGrounded())
        {
            ChangeState<PlayerAirState>(new StateConfig.StartingVelocityConfig(Vector2.right * _horizontalVelocity));
            return;
        }
        if (CheckJump())
        {
            ChangeState<PlayerJumpState>(new StateConfig.StartingVelocityConfig(new Vector2(_horizontalVelocity, Agent.PlayerVariables.JumpSpeed)));
            return;
        }
    }

    protected override void FixedUpdateStateInternal(float delta)
    {

        // horizontalMovement
        _horizontalVelocity = StateBehaviour.CalculateHorizontalVelocity(
            _horizontalVelocity,
            Agent.CurrentFrameInput.Direction.x * Agent.PlayerVariables.GroundSpeed,
            Agent.PlayerVariables.GroundAcceleration,
            Agent.PlayerVariables.GroundDeceleration,
            delta);

        Agent.MovementComponent.SetVelocity(MovementComponent.VelocityType.MainMovement, Vector2.right * _horizontalVelocity);

    }

    private bool IsGrounded()
    {
        return Agent.MovementComponent.IsAgainstGround;
    }

    private bool CheckJump()
    {
        return Agent.CurrentFrameInput.JumpPressedThisFrame;
    }
}
