using UnityEngine;

namespace AstralCore
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Scriptables/Time Context", fileName = "newTimeContext")]
    public class ScriptableTimeContext : ScriptableObject, ITimeContext
    {

        public float DeltaTimeMultiplier;
        public float FixedDeltaTimeMultiplier;

        private bool _paused;

        public float DeltaTime => _paused ? 0 : DeltaTimeMultiplier * Time.deltaTime;

        public float FixedDeltaTime => _paused ? 0 : FixedDeltaTimeMultiplier * Time.fixedDeltaTime;

        public System.Action<bool> OnPause = new System.Action<bool>(p => { });

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
