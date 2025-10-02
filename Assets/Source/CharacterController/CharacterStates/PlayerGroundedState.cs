using UnityEngine;

public class PlayerGroundedState : BaseState<CharacterController2d>
{

    float _horizontalVelocity;

    protected override void EnterStateInternal(params StateConfig.IBaseStateConfig[] configs)
    {
        Agent.MovementComponent.SetVelocity(MovementComponent.VelocityType.Gravity, Vector2.zero);
        _horizontalVelocity = Agent.MovementComponent.GetVelocity(MovementComponent.VelocityType.MainMovement).x;
    }

    protected override void ExitStateInternal()
    {
    }

    protected override void FixedUpdateStateInternal(double delta)
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

        if (Mathf.Abs(diff) > acceleration) // if we won't go over the target speed, accelerate
        {
            _horizontalVelocity += Mathf.Sign(diff) * acceleration;
        }
        else // if we would go over the target speed, just set it to the target speed
        {
            _horizontalVelocity = targetHorizontalVelocity;
        }
        Agent.MovementComponent.SetVelocity(MovementComponent.VelocityType.MainMovement, Vector2.right * _horizontalVelocity);
    }
}
