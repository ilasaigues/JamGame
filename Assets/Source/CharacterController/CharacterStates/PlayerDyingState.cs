using UnityEngine;

public class PlayerDyingState : BaseState<CharacterController2d>
{
    protected override void EnterStateInternal(params StateConfig.IBaseStateConfig[] configs)
    {
    }

    protected override void ExitStateInternal()
    {
    }

    protected override void FixedUpdateStateInternal(float delta)
    {
        Agent.MovementComponent.SetVelocity(Vector2.zero);
    }

    protected override void UpdateStateInternal(float delta)
    {
    }
}