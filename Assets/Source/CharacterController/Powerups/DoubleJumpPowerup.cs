
public class DoubleJumpPowerup : BasePowerup
{
    public override CharacterController2d.PowerupType PowerupType => CharacterController2d.PowerupType.DoubleJump;

    protected override void ApplyInternal(CharacterController2d characterController, PlayerCharacterStateMachine stateMachine)
    {
        characterController.RuntimeVars.MaxJumps = 2;
    }

    protected override void UnapplyInternal()
    {
        AppliedController.RuntimeVars.MaxJumps = 1;
    }
}