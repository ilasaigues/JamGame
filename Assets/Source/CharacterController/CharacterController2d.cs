using System;
using System.Collections;
using AstralCore;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(MovementComponent), typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class CharacterController2d : TimeboundMonoBehaviour, IKillable
{
    public class RuntimeVariables
    {
        public bool CanJump => UsedJumps < MaxJumps;
        public int UsedJumps = 0;
        public int MaxJumps = 1;
        public DateTime TimeLastLeftGround = DateTime.MinValue;
        public bool CanDash = false;
    }
    public struct FrameInput
    {
        public bool JumpPressedThisFrame;
        public DateTime JumpPressedTime;
        public bool JumpHeld;
        public Vector2 Direction;
        public Vector2 LastNonZeroHorizontalDirection;
    }

    public enum PowerupType
    {
        None = 0,
        DoubleJump = 1,
        Dash = 2,
        Hover = 3,
        StoneSkin = 4,
    }

    public static class AnimationParameters
    {
        public static string Stoneskin = "Stoneskin";
        public static string Grounded = "Grounded";
        public static string Moving = "Moving";
        public static string Dashing = "Dashing";
        public static string Dead = "Dead";
        public static string DoubleJumpTrigger = "DoubleJump";
        public static string Rising = "Rising";
        public static string Hovering = "Hovering";
    }

    public PlayerVariables PlayerVariables;
    //---------

    public MovementComponent MovementComponent;
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    public FrameInput CurrentFrameInput = default;


    public PlayerCharacterStateMachine CharacterStateMachine;


    public RuntimeVariables RuntimeVars = new();

    //---------
    private readonly DashPowerup DashPowerup = new();
    private readonly DoubleJumpPowerup DoubleJumpPowerup = new();
    private readonly HoverPowerup HoverPowerup = new();
    private readonly StoneSkinPowerup StoneSkinPowerup = new();
    private BasePowerup CurrentPowerup = null;

    private LevelController LevelController;

    private void Awake()
    {
        MovementComponent = GetComponent<MovementComponent>();
        CharacterStateMachine.SetAgent(this);
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        CharacterStateMachine.SetNextState(new StateChangeRequest(typeof(PlayerGroundedState)));
        LevelController = FindFirstObjectByType<LevelController>();
        if (LevelController == null)
        {
            AstralCore.Logger.LogWarning(LogCategory.Message, "NO LEVEL CONTROLLER FOUND, RESPAWN WON'T WORK");
        }
    }

    private void Update()
    {
        if (_timeContext.Paused) return;
        //TEMP
        if (Keyboard.current[Key.Digit0].wasPressedThisFrame)
        {
            SetCurrentPowerup(PowerupType.None);
        }
        else if (Keyboard.current[Key.Digit1].wasPressedThisFrame)
        {
            SetCurrentPowerup(PowerupType.DoubleJump);
        }
        else if (Keyboard.current[Key.Digit2].wasPressedThisFrame)
        {
            SetCurrentPowerup(PowerupType.Dash);
        }
        else if (Keyboard.current[Key.Digit3].wasPressedThisFrame)
        {
            SetCurrentPowerup(PowerupType.Hover);
        }
        else if (Keyboard.current[Key.Digit4].wasPressedThisFrame)
        {
            SetCurrentPowerup(PowerupType.StoneSkin);
        }
        //TEMP

        CurrentFrameInput.JumpPressedThisFrame = PlayerVariables.JumpInput.action.WasPressedThisFrame();
        if (CurrentFrameInput.JumpPressedThisFrame)
        {
            CurrentFrameInput.JumpPressedTime = DateTime.Now;
        }
        CurrentFrameInput.JumpHeld = PlayerVariables.JumpInput.action.IsPressed();
        CurrentFrameInput.Direction = PlayerVariables.MoveInput.action.ReadValue<Vector2>();
        if (CurrentFrameInput.Direction.x != 0)
        {
            CurrentFrameInput.LastNonZeroHorizontalDirection = CurrentFrameInput.Direction;
        }
        _spriteRenderer.flipX = CurrentFrameInput.LastNonZeroHorizontalDirection.x < 0;
    }

    public bool CanTakePowerup(PowerPickup pickup)
    {
        if (CharacterStateMachine.IsInState<PlayerDyingState>() || (CurrentPowerup != null && CurrentPowerup.PowerupType == pickup.powerupType))
        {
            return false;
        }
        return true;
    }

    public void SetCurrentPowerup(PowerupType powerupType)
    {
        BasePowerup newPowerup = null;
        switch (powerupType)
        {
            case PowerupType.DoubleJump:
                newPowerup = DoubleJumpPowerup;
                break;
            case PowerupType.Dash:
                newPowerup = DashPowerup;
                break;
            case PowerupType.Hover:
                newPowerup = HoverPowerup;
                break;
            case PowerupType.StoneSkin:
                newPowerup = StoneSkinPowerup;
                break;
        }
        if (CurrentPowerup != newPowerup)
        {
            CurrentPowerup?.Unapply();
            CurrentPowerup = newPowerup;
            CurrentPowerup?.Apply(this, CharacterStateMachine);
        }
    }

    private void FixedUpdate()
    {
        MovementComponent.PhysicsMove(_rb);
    }

    public bool CanBeKilledBy(BaseHazard hazard)
    {
        // check for current state to see if hazard can actually kill

        return !CharacterStateMachine.IsInState<PlayerDyingState>();
    }

    public IEnumerator Kill(BaseHazard hazard)
    {
        if (LevelController == null) yield break;
        // disable controls (dying state?)
        // play death animation
        CharacterStateMachine.SetNextState(new StateChangeRequest(typeof(PlayerDyingState)));

        // WAIT UNTIL ANIMATION IS DONE


        // check if this is your first death of that soul type
        // ^ Save file handling required, check to see how many deaths of this type have happened
        // ^ spawn death sprite and play dialogue if it is
        // WAIT UNTIL DIALOGUE IS DONE

        // drop soul
        hazard.SpawnPickup(transform.position);

        // WAIT A BIT
        yield return new WaitForSeconds(0.5f);

        // move to respawn position
        transform.position = LevelController.transform.position;

        // play respawn animation
        SetAnimationFlag(AnimationParameters.Dead, false);
        // WAIT UNTIL ANIMATION IS DONE
        yield return new WaitForSeconds(1f);

        // reset powerup
        SetCurrentPowerup(PowerupType.None);
        // re-enable controls
        CharacterStateMachine.SetNextState(new StateChangeRequest(typeof(PlayerAirState), new StateConfig.StartingVelocityConfig(Vector2.zero)));
    }

    public void SetAnimationFlag(string animFlag, bool active)
    {
        _animator.SetBool(animFlag, active);
    }

    public void SetAnimatorTrigger(string animTrigger)
    {
        _animator.SetTrigger(animTrigger);
    }
}
