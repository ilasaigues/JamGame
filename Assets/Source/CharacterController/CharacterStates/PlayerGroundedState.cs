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

        float targetHorizontalVelocity;
        float acceleration;

        if (Agent.CurrentFrameInput.Direction.x != 0) // moving, accelerate to target speed
        {
            targetHorizontalVelocity = Agent.CurrentFrameInput.Direction.x * Agent.PlayerVariables.GroundSpeed;
            acceleration = Agent.PlayerVariables.GroundAcceleration;
        }
        else // not moving, decelerate to zero
        {
            targetHorizontalVelocity = 0;
            acceleration = Agent.PlayerVariables.GroundDeceleration;
        }

        var diff = targetHorizontalVelocity - _horizontalVelocity;

        if (Mathf.Abs(diff) > acceleration * delta) // if we won't go over the target speed, accelerate
        {
            _horizontalVelocity += Mathf.Sign(diff) * acceleration * delta;
        }
        else // if we would go over the target speed, just set it to the target speed
        {
            _horizontalVelocity = targetHorizontalVelocity;
        }
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
