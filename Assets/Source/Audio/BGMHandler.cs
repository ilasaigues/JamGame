using UnityEngine;
using FMODUnity;

public class BGMHandler : MonoBehaviour
{

    StudioEventEmitter bgm;
    public SFX sfxbank;

    void Start()
    {
        bgm = GetComponent<StudioEventEmitter>();
        bgm.Play();
        RuntimeManager.PlayOneShot(sfxbank.powerup);
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

    public void playSFX(EventReference sfx)
    {
        RuntimeManager.PlayOneShot(sfxbank.powerup);
    }
}
