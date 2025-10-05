using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu]
public class DialogueAsset : ScriptableObject
{

    public Sprite ActorSprite;
    public string ActorName;
    [TextArea]
    public string Text;
    public DialogueAsset NextDialogue;

}
