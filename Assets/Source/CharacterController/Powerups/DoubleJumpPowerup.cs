
public class DoubleJumpPowerup : BasePowerup
{
    protected override void ApplyInternal(CharacterController2d characterController, PlayerCharacterStateMachine stateMachine)
    {
        characterController.RuntimeVars.MaxJumps = 2;
    }

    protected override void UnapplyInternal()
    {
        AppliedController.RuntimeVars.MaxJumps = 2;
    }
}