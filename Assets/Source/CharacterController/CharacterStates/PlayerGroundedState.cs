using System;
using AstralCore;
using UnityEngine;

public class PlayerGroundedState : BaseState<CharacterController2d>
{

    float _horizontalVelocity;

    protected override void EnterStateInternal(params StateConfig.IBaseStateConfig[] configs)
    {
        InitFromConfigs(configs);
        _horizontalVelocity = Agent.MovementComponent.CurrentVelocity.x;
        //reset jump
        Agent.RuntimeVars.UsedJumps = 0;
        Agent.RuntimeVars.CanDash = true;
        Agent.SetAnimationFlag(CharacterController2d.AnimationParameters.Rising, false);
        Agent.SetAnimationFlag(CharacterController2d.AnimationParameters.Grounded, true);
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
            Agent.RuntimeVars.TimeLastLeftGround = DateTime.Now;
            ChangeState<PlayerAirState>(new StateConfig.StartingVelocityConfig(Vector2.right * _horizontalVelocity));
            return;
        }
        if (CheckCanJump())
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
        _horizontalVelocity = Mathf.Clamp(_horizontalVelocity, -Agent.PlayerVariables.GroundSpeed, Agent.PlayerVariables.GroundSpeed);

        Agent.MovementComponent.SetVelocity(Vector2.right * _horizontalVelocity);
        HandleAnimation();
    }

    private void HandleAnimation()
    {
        Agent.SetAnimationFlag(CharacterController2d.AnimationParameters.Moving, _horizontalVelocity != 0);
    }

    private bool IsGrounded()
    {
        return Agent.MovementComponent.IsAgainstGround;
    }

    private bool CheckCanJump()
    {
        return Agent.RuntimeVars.CanJump &&
            (Agent.CurrentFrameInput.JumpPressedThisFrame || // Jump was pressed this frame or...
            (DateTime.Now - Agent.CurrentFrameInput.JumpPressedTime).TotalSeconds <= Agent.PlayerVariables.BufferTime); // ... was pressed recently
    }
}
