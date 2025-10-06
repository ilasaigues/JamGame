using UnityEngine;
using FMOD;
using FMODUnity;
using FMOD.Studio;

[CreateAssetMenu(fileName = "SFX", menuName = "Scriptable Objects/SFX")]
public class SFX : ScriptableObject
{
    public EventReference jump, walk, dash, hover, stone, heavy, fire, death, powerup;
}
