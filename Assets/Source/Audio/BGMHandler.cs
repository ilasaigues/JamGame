using UnityEngine;
using FMODUnity;

public class BGMHandler : MonoBehaviour
{
    public static BGMHandler Instance { get; private set; }

    StudioEventEmitter bgm;
    public SFX sfxbank;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            DestroyImmediate(this);
        }
    }

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

    public void PlaySFX(EventReference sfx)
    {
        RuntimeManager.PlayOneShot(sfx);
    }
}
