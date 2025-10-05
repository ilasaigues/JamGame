using UnityEngine;
using FMODUnity;

public class BGMHandler : MonoBehaviour
{

    StudioEventEmitter bgm;

    void Start()
    {
        bgm = GetComponent<StudioEventEmitter>();
        bgm.Play();
    }

    public void Pause()
    {
        bgm.EventInstance.getPaused(out bool paused);
        bgm.EventInstance.setPaused(!paused);
    }

    public void SetBGM(int newState)
    {
        bgm.EventInstance.setParameterByName("state", newState);
    }
}
