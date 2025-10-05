using UnityEngine.InputSystem;

public class StoneSkinPowerup : BasePowerup
{
    public override CharacterController2d.PowerupType PowerupType => CharacterController2d.PowerupType.StoneSkin;
    protected override void ApplyInternal(CharacterController2d characterController, PlayerCharacterStateMachine stateMachine)
    {
        characterController.PlayerVariables.SpecialInput.action.performed += OnStoneSkinPressed;
        characterController.PlayerVariables.SpecialInput.action.canceled += OnStoneSkinReleased;
    }

    protected override void UnapplyInternal()
    {
        AppliedController.PlayerVariables.SpecialInput.action.performed -= OnStoneSkinPressed;
        AppliedController.PlayerVariables.SpecialInput.action.canceled -= OnStoneSkinReleased;
    }

    private void OnStoneSkinReleased(InputAction.CallbackContext context)
    {
        if (AppliedStateMachine.IsInState<PlayerStoneSkinState>())
        {
            AppliedStateMachine.SetNextState(
                new StateChangeRequest(
                AppliedController.MovementComponent.IsAgainstGround ?
                typeof(PlayerGroundedState) : typeof(PlayerAirState),
                new StateConfig.StartingVelocityConfig(AppliedController.MovementComponent.CurrentVelocity)));
        }
    }

    private void OnStoneSkinPressed(InputAction.CallbackContext context)
    {
        if (!AppliedStateMachine.IsInState<PlayerStoneSkinState>())
        {
            AppliedStateMachine.SetNextState(
                new StateChangeRequest(typeof(PlayerStoneSkinState),
                new StateConfig.StartingVelocityConfig(AppliedController.MovementComponent.CurrentVelocity)));
        }
    }

}