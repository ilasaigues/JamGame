using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace AstralCore
{
    public abstract class BaseSceneTransitionBehaviour : ScriptableObject
    {
        [SerializeField] protected float _transitionDuration;
        public abstract Task EnterAsync(Canvas target);
        public abstract Task ExitAsync(Canvas target);
    }
}
