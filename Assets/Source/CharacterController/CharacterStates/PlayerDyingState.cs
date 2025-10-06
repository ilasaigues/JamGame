using UnityEngine;

public class PlayerDyingState : BaseState<CharacterController2d>
{
    protected override void EnterStateInternal(params StateConfig.IBaseStateConfig[] configs)
    {
        Agent.SetAnimationFlag(CharacterController2d.AnimationParameters.Dead, true);
        Agent.SetAnimationFlag(CharacterController2d.AnimationParameters.Stoneskin, false);
        Agent.SetAnimationFlag(CharacterController2d.AnimationParameters.Moving, false);
        Agent.SetAnimationFlag(CharacterController2d.AnimationParameters.Dashing, false);
        Agent.SetAnimationFlag(CharacterController2d.AnimationParameters.Rising, false);
        Agent.SetAnimationFlag(CharacterController2d.AnimationParameters.Hovering, false);

    }

    protected override void ExitStateInternal()
    {
        Agent.SetAnimationFlag(CharacterController2d.AnimationParameters.Dead, false);
    }

    protected override void FixedUpdateStateInternal(float delta)
    {
        Agent.MovementComponent.SetVelocity(Vector2.zero);
    }

    protected override void UpdateStateInternal(float delta)
    {
    }
}