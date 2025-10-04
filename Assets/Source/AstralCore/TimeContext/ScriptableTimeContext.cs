using UnityEngine;

namespace AstralCore
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Scriptables/Time Context", fileName = "newTimeContext")]
    public class ScriptableTimeContext : ScriptableObject, ITimeContext
    {

        public float DeltaTimeMultiplier;
        public float FixedDeltaTimeMultiplier;

        public bool Paused { get; private set; }

        public float DeltaTime => Paused ? 0 : DeltaTimeMultiplier * Time.deltaTime;

        public float FixedDeltaTime => Paused ? 0 : FixedDeltaTimeMultiplier * Time.fixedDeltaTime;

        public System.Action<bool> OnPause = new System.Action<bool>(p => { });

        public void TogglePause()
        {
            SetPause(Paused);
        }

        public void SetPause(bool pause)
        {
            if (Paused == pause) return;
            else
            {
                Paused = pause;
                OnPause(pause);
            }
        }
    }
}
