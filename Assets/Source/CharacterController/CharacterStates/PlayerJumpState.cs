using UnityEngine;

public class PlayerJumpState : BaseState<CharacterController2d>
{
    Vector2 _velocity;
    bool _jumpHeld = false;

    protected override void EnterStateInternal(params StateConfig.IBaseStateConfig[] configs)
    {
        InitFromConfigs(configs);
        _jumpHeld = true;
        Agent.SetAnimationFlag(CharacterController2d.AnimationParameters.Grounded, false);
        Agent.SetAnimationFlag(CharacterController2d.AnimationParameters.Rising, true);
        if (Agent.RuntimeVars.UsedJumps > 0)
        {
            Agent.SetAnimatorTrigger(CharacterController2d.AnimationParameters.DoubleJumpTrigger);
        }

        Agent.SetAnimationFlag(CharacterController2d.AnimationParameters.Rising, Agent.RuntimeVars.UsedJumps == 0);
        Agent.RuntimeVars.UsedJumps++;

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
            return;
        }
        if (CheckCanJump())
        {
            ChangeState<PlayerJumpState>(new StateConfig.StartingVelocityConfig(new Vector2(_velocity.x, Agent.PlayerVariables.JumpSpeed)));
        }
    }

    protected override void FixedUpdateStateInternal(float delta)
    {

        // if we stop holding jump, we set this flag to false
        _jumpHeld &= Agent.CurrentFrameInput.JumpHeld;

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


        Agent.MovementComponent.SetVelocity(_velocity);
    }

    float GetGravity()
    {
        if (_jumpHeld)
        {
            return Agent.PlayerVariables.JumpGravity;
        }
        else
        {
            return Agent.PlayerVariables.Gravity * 2;
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

    private bool CheckCanJump()
    {
        return Agent.RuntimeVars.CanJump && Agent.CurrentFrameInput.JumpPressedThisFrame;
    }
}