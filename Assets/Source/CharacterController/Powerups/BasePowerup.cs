using UnityEngine;

public abstract class BasePowerup
{
    protected CharacterController2d AppliedController;
    protected PlayerCharacterStateMachine AppliedStateMachine;

    public void Apply(CharacterController2d characterController, PlayerCharacterStateMachine stateMachine)
    {
        AppliedController = characterController;
        AppliedStateMachine = stateMachine;
        ApplyInternal(characterController, stateMachine);
    }

    protected abstract void ApplyInternal(CharacterController2d characterController, PlayerCharacterStateMachine stateMachine);
    public void Unapply()
    {
        UnapplyInternal();
        AppliedController = null;
        AppliedStateMachine = null;
    }

    protected abstract void UnapplyInternal();
}

