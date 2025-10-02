using UnityEngine;

namespace AstralCore
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Scriptables/Time Context", fileName = "newTimeContext")]
    public class ScriptableTimeContext : ScriptableObject, ITimeContext
    {

        [SerializeField]
        private float _deltaTimeMultiplier;
        [SerializeField]
        private float _fixedDeltaTimeMultiplier;

        private bool _paused;

        public float DeltaTime => _paused ? 0 : _deltaTimeMultiplier * Time.deltaTime;

        public float FixedDeltaTime => _paused ? 0 : _fixedDeltaTimeMultiplier * Time.fixedDeltaTime;

        public System.Action<bool> OnPause = new System.Action<bool>(p => { });

        public void SetDeltaTimeMultiplier(float multiplier)
        {
            _deltaTimeMultiplier = multiplier;
        }

        public void SetFixedDeltaTimeMultiplier(float multiplier)
        {
            _fixedDeltaTimeMultiplier = multiplier;
        }

        public void TogglePause()
        {
            SetPause(_paused);
        }

        public void SetPause(bool pause)
        {
            if (_paused == pause) return;
            else
            {
                _paused = pause;
                OnPause(pause);
            }
        }
    }
}
