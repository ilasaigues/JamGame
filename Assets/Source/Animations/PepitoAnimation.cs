using UnityEngine;

[CreateAssetMenu(fileName = "PepitoAnimation", menuName = "Scriptable Objects/PepitoAnimation")]
public class PepitoAnimation : ScriptableObject
{
    public AnimationClip idle, walk, jump, fall, doubleJump, dash, hover, stoneSkinOn, stoneSkinOff, death, respawn;
}
