using UnityEngine;

public class PlayerDashState : BaseState<CharacterController2d>
{
    float _velocity = 0;
    Vector2 _startPos;
    protected override void EnterStateInternal(params StateConfig.IBaseStateConfig[] configs)
    {
        InitFromConfigs(configs);
        _startPos = Agent.transform.position;
        Agent.RuntimeVars.CanDash = false;
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
    }

    protected override void FixedUpdateStateInternal(float delta)
    {
        Agent.MovementComponent.SetVelocity(Vector2.right * _velocity);

        if (Vector2.Distance(_startPos, Agent.transform.position) > Agent.PlayerVariables.DashDistance)
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