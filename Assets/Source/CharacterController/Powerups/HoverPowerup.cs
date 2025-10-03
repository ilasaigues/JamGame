using System;
using UnityEngine.InputSystem;

public class HoverPowerup : BasePowerup
{
    protected override void ApplyInternal(CharacterController2d characterController, PlayerCharacterStateMachine stateMachine)
    {
        characterController.PlayerVariables.SpecialInput.action.performed += OnHoverPressed;
        characterController.PlayerVariables.SpecialInput.action.canceled += OnHoverReleased;
    }

    protected override void UnapplyInternal()
    {
        AppliedController.PlayerVariables.SpecialInput.action.performed -= OnHoverPressed;
        AppliedController.PlayerVariables.SpecialInput.action.canceled -= OnHoverReleased;
    }

    private void OnHoverReleased(InputAction.CallbackContext context)
    {
        if (AppliedStateMachine.IsInState<PlayerHoverState>())
        {
            AppliedStateMachine.SetNextState(
                new StateChangeRequest(
                AppliedController.MovementComponent.IsAgainstGround ?
                typeof(PlayerGroundedState) : typeof(PlayerAirState),
                new StateConfig.StartingVelocityConfig(AppliedController.MovementComponent.CurrentVelocity)));
        }
    }

    private void OnHoverPressed(InputAction.CallbackContext context)
    {
        if (!AppliedStateMachine.IsInState<PlayerHoverState>())
        {
            AppliedStateMachine.SetNextState(
                new StateChangeRequest(typeof(PlayerHoverState),
                new StateConfig.StartingVelocityConfig(AppliedController.MovementComponent.CurrentVelocity)));
        }
    }

}