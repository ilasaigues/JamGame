
using System;
using UnityEngine.InputSystem;

public class DashPowerup : BasePowerup
{
    protected override void ApplyInternal(CharacterController2d characterController, PlayerCharacterStateMachine stateMachine)
    {
        characterController.PlayerVariables.SpecialInput.action.performed += OnDashPressed;
    }

    protected override void UnapplyInternal()
    {
        AppliedController.PlayerVariables.SpecialInput.action.performed -= OnDashPressed;
    }

    private void OnDashPressed(InputAction.CallbackContext context)
    {
        if (!AppliedStateMachine.IsInState<PlayerDashState>() && AppliedController.RuntimeVars.CanDash)
        {
            AppliedStateMachine.SetNextState(
                new StateChangeRequest(typeof(PlayerDashState),
                 new StateConfig.StartingVelocityConfig(AppliedController.CurrentFrameInput.LastNonZeroHorizontalDirection)));
        }
    }

}