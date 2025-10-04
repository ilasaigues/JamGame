using UnityEngine;

namespace AstralCore
{
    public abstract class GenericContextualizer<T> : TimeboundMonoBehaviour where T : Component
    {
        private T Component;
        void Start()
        {
            Component = GetComponent<T>();
        }

        void Update()
        {
            ApplyTimeMultiplier(Component, _timeContext.Paused ? 0 : _timeContext.DeltaTimeMultiplier);
        }

        protected abstract void ApplyTimeMultiplier(T component, float timeMult);
    }
}
