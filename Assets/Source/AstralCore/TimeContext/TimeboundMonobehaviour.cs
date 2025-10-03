using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace AstralCore
{

    /// <summary>
    /// This class should be used for any and all monobehaviours that depend on a variable TimeContext to adjust their speed or pause/unpause them
    /// </summary>
    public abstract class TimeboundMonoBehaviour : MonoBehaviour
    {
        [SerializeField] protected ScriptableTimeContext _timeContext;

        protected float DeltaTime => _timeContext ? _timeContext.DeltaTime : Time.deltaTime;
        protected float FixedDeltaTime => _timeContext ? _timeContext.FixedDeltaTime : Time.fixedDeltaTime;

        private void Start()
        {
            if (_timeContext == null)
            {
                _timeContext = Resources.Load<ScriptableTimeContext>("DefaultTimeContext");
#if UNITY_EDITOR
                if (_timeContext == null)
                {
                    _timeContext = ScriptableObject.CreateInstance<ScriptableTimeContext>();
                    _timeContext.DeltaTimeMultiplier = _timeContext.FixedDeltaTimeMultiplier = 1;
                    AssetDatabase.CreateAsset(_timeContext, "Assets/Resources/DefaultTimeContext.asset");
                }
#endif
            }
            if (_timeContext != null)
            {
                _timeContext.OnPause += OnPause;
            }
        }

        public void SetTimeContext(ScriptableTimeContext newTimeContext)
        {
            UnsubscribeFromTimeContext();
            _timeContext = newTimeContext;
            _timeContext.OnPause += OnPause;
        }

        private void UnsubscribeFromTimeContext()
        {
            if (_timeContext != null)
            {
                _timeContext.OnPause -= OnPause;
            }
        }

        void OnDestroy()
        {
            UnsubscribeFromTimeContext();
        }

        protected virtual void OnPause(bool pause)
        {
            // no op
        }

    }
}
