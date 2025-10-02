using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(MovementComponent), typeof(Rigidbody2D))]
public class CharacterController2d : MonoBehaviour
{

    public struct FrameInput
    {
        public bool JumpPressedThisFrame;
        public bool JumpHeld;
        public Vector2 Direction;
    }

    /*
    [x] Aceleración y deceleración  en piso y aire.
    [x] Salto variable.
    [x] Gravedad en salto, ápex y caida.
    [ ] Coyote jump.
    [ ] Buffer time.
    [ ] Correccion para esquinas.
    [ ] Collider variable segun el estado del salto.
    */

    public PlayerVariables PlayerVariables;
    //---------

    public MovementComponent MovementComponent;
    private Rigidbody2D _rb;

    public FrameInput CurrentFrameInput = default;

    public PlayerCharacterStateMachine CharacterStateMachine;

    private void Awake()
    {
        MovementComponent = GetComponent<MovementComponent>();
        CharacterStateMachine.SetAgent(this);
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        CharacterStateMachine.SetNextState(new StateChangeRequest(typeof(PlayerGroundedState)));
    }

    private void Update()
    {
        CurrentFrameInput.JumpPressedThisFrame = PlayerVariables.JumpInput.action.WasPressedThisFrame();
        CurrentFrameInput.JumpHeld = PlayerVariables.JumpInput.action.IsPressed();
        CurrentFrameInput.Direction = PlayerVariables.MoveInput.action.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        MovementComponent.PhysicsMove(_rb);
    }

}
