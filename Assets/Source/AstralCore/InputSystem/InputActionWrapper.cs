using UnityEngine;
using UnityEngine.InputSystem;
namespace AstralCore
{
    public class InputActionWrapper
    {
        public InputAction BaseAction;

        private float _timeLastStart = float.NegativeInfinity;
        private float _timeLastCancel = float.NegativeInfinity;
        private float _timeLastPerformed = float.NegativeInfinity;

        public float TimeSinceStart => Time.time - _timeLastStart;
        public float TimeSinceCancel => Time.time - _timeLastCancel;
        public float TimeSincePerform => Time.time - _timeLastPerformed;

        public event System.Action<InputAction.CallbackContext> OnPerformed;
        public event System.Action<InputAction.CallbackContext> OnStarted;
        public event System.Action<InputAction.CallbackContext> OnCancelled;

        public bool IsPressed => BaseAction.IsPressed();

        public InputActionWrapper(InputAction baseAction)
        {
            this.BaseAction = baseAction;
            OnStarted += (c) => _timeLastStart = Time.time;
            OnCancelled += (c) => _timeLastCancel = Time.time;
            OnPerformed += (c) => _timeLastPerformed = Time.time;

            BaseAction.performed += (c) => OnPerformed.Invoke(c);
            BaseAction.started += (c) => OnStarted.Invoke(c);
            BaseAction.canceled += (c) => OnCancelled.Invoke(c);
        }
    }
}
