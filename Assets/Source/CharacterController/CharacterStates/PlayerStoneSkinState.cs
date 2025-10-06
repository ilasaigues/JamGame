using UnityEngine;

public class PlayerStoneSkinState : BaseState<CharacterController2d>
{
    Vector2 _velocity;
    protected override void EnterStateInternal(params StateConfig.IBaseStateConfig[] configs)
    {
        InitFromConfigs(configs);
        Agent.SetAnimationFlag(CharacterController2d.AnimationParameters.Stoneskin, true);
        BGMHandler.Instance.PlaySFX(Agent.soundEffects.stone);
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
        Agent.SetAnimationFlag(CharacterController2d.AnimationParameters.Stoneskin, false);
    }
    protected override void UpdateStateInternal(float delta) { }

    protected override void FixedUpdateStateInternal(float delta)
    {
        // vertical movement
        _velocity.y = StateBehaviour.CalculateVerticalVelocity(
           _velocity.y,
           Agent.PlayerVariables.MaxAirVelocity,
           GetGravity,
           delta);
        // horizontalMovement
        _velocity.x = 0;

        Agent.MovementComponent.SetVelocity(_velocity);
    }

    float GetGravity()
    {
        return Agent.PlayerVariables.StoneSkinGravity;
    }
}