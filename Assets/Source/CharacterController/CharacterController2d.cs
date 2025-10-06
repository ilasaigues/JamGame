using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AstralCore;
using Unity.VisualScripting;
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

    Dictionary<PowerupType, int> DeathCounts = new();

    public int TotalDeaths => DeathCounts.Values.Sum();

    [SerializeField] DialogueAsset FirstDeathDialogue;
    [SerializeField] DialogueAsset FirstLaserDialogue;
    [SerializeField] DialogueAsset FirstSpikesDialogue;
    [SerializeField] DialogueAsset FirstCrusherDialogue;
    [SerializeField] DialogueAsset FirstFireDialogue;

    [SerializeField] Animator DeathPrefab;

    public SFX soundEffects;

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
        BGMHandler.Instance.SetBGM((int)powerupType);
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
        if (CharacterStateMachine.IsInState<PlayerDyingState>())
        {
            return false;
        }
        switch (hazard.PowerupConfig.PowerupType)
        {
            case PowerupType.DoubleJump: return true;
            case PowerupType.Dash: return !CharacterStateMachine.IsInState<PlayerDashState>();
            case PowerupType.Hover: return true;
            case PowerupType.StoneSkin:
                if (CharacterStateMachine.IsInState<PlayerStoneSkinState>())
                {
                    BGMHandler.Instance.PlaySFX(soundEffects.stone);
                    return false;
                }
                else return true;
            default: return true;

        }
    }

    public void PlayWalkSound()
    {
        BGMHandler.Instance.PlaySFX(soundEffects.walk);
    }

    public IEnumerator Kill(BaseHazard hazard)
    {
        // reset powerup
        SetCurrentPowerup(PowerupType.None);

        if (LevelController == null) yield break;
        // disable controls (dying state?)
        // play death animation
        CharacterStateMachine.SetNextState(new StateChangeRequest(typeof(PlayerDyingState)));

        Animator deathInstance = null;
        // WAIT UNTIL ANIMATION IS DONE

        if (TotalDeaths == 0) // first death
        {
            yield return new WaitForSeconds(2);

            // spawn death on hazard respawner location
            deathInstance = Instantiate(DeathPrefab, hazard.powerPickupSpawner.transform.position, Quaternion.identity);
            if (deathInstance.transform.position.x > transform.position.x)
            {
                deathInstance.GetOrAddComponent<SpriteRenderer>().flipX = true;
            }
            deathInstance.SetBool("active", true);


            yield return new WaitForSeconds(2);

            // enqueue first death dialogue
            LevelController.DialoguePlayer.EnqueueDialogue(FirstDeathDialogue);
        }

        if (!DeathCounts.TryGetValue(hazard.PowerupConfig.PowerupType, out var count))
        {
            if (deathInstance == null)
            {
                yield return new WaitForSeconds(1);
                deathInstance = Instantiate(DeathPrefab, hazard.powerPickupSpawner.transform.position, Quaternion.identity);
                if (deathInstance.transform.position.x > transform.position.x)
                {
                    deathInstance.GetOrAddComponent<SpriteRenderer>().flipX = true;
                }
                deathInstance.SetBool("active", true);
            }
            yield return new WaitForSeconds(1);
            DeathCounts[hazard.PowerupConfig.PowerupType] = 0;
            // enqueue first death of type
            switch (hazard.PowerupConfig.PowerupType)
            {
                case PowerupType.DoubleJump:
                    LevelController.DialoguePlayer.EnqueueDialogue(FirstSpikesDialogue);
                    break;
                case PowerupType.Dash:
                    LevelController.DialoguePlayer.EnqueueDialogue(FirstLaserDialogue);
                    break;
                case PowerupType.Hover:
                    LevelController.DialoguePlayer.EnqueueDialogue(FirstFireDialogue);
                    break;
                case PowerupType.StoneSkin:
                    LevelController.DialoguePlayer.EnqueueDialogue(FirstCrusherDialogue);
                    break;
            }
        }

        DeathCounts[hazard.PowerupConfig.PowerupType] = DeathCounts[hazard.PowerupConfig.PowerupType] + 1;

        // WAIT UNTIL DIALOGUE IS DONE
        yield return new WaitUntil(() => !LevelController.DialoguePlayer.IsInDialogue);

        // hide death
        if (deathInstance != null)
        {
            deathInstance.SetBool("active", false);
        }

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
