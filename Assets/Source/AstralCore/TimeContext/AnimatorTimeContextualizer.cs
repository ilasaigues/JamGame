using UnityEngine;

namespace AstralCore
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorTimeContextualizer : GenericContextualizer<Animator>
    {
        protected override void ApplyTimeMultiplier(Animator component, float timeMult)
        {
            component.speed = timeMult;
        }
    }
}
