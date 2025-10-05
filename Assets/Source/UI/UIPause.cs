using AstralCore;
using UnityEngine;

public class UIPause : MonoBehaviour
{
    [SerializeField] ScriptableTimeContext PauseContext;
    void OnEnable()
    {
        PauseContext.SetPause(true);
    }

    void OnDestroy()
    {
        PauseContext.SetPause(false);
    }
}
