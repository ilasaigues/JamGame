using UnityEngine;

public class PlayerDashState : BaseState<CharacterController2d>
{
    float _velocity = 0;
    float _remainingDistance;
    protected override void EnterStateInternal(params StateConfig.IBaseStateConfig[] configs)
    {
        InitFromConfigs(configs);
        _remainingDistance = Agent.PlayerVariables.DashDistance;
        Agent.RuntimeVars.CanDash = false;
        Agent.SetAnimationFlag(CharacterController2d.AnimationParameters.Dashing, true);
        BGMHandler.Instance.PlaySFX(Agent.soundEffects.dash);

    }

    private void InitFromConfigs(params StateConfig.IBaseStateConfig[] configs)
    {
        foreach (var config in configs)
        {
            if (config is StateConfig.StartingVelocityConfig velocityConfig)
            {
                _velocity = Mathf.Sign(velocityConfig.Velocity.x) * Agent.PlayerVariables.DashVelocity;
            }
        }
    }

    protected override void ExitStateInternal()
    {
        Agent.SetAnimationFlag(CharacterController2d.AnimationParameters.Dashing, false);
    }

    protected override void FixedUpdateStateInternal(float delta)
    {
        Agent.MovementComponent.SetVelocity(Vector2.right * _velocity);
        _remainingDistance -= Mathf.Abs(_velocity * delta);
        if (_remainingDistance <= 0)
        {
            Agent.CharacterStateMachine.SetNextState(
                new StateChangeRequest(Agent.MovementComponent.IsAgainstGround ?
                typeof(PlayerGroundedState) : typeof(PlayerAirState),
                new StateConfig.StartingVelocityConfig(Vector2.right * _velocity)));
        }
    }

    protected override void UpdateStateInternal(float delta)
    {
    }
}