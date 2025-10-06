using AstralCore;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class LevelExit : MonoBehaviour
{
    public SceneReference sceneReference;
    public BaseSceneTransitionBehaviour transitionBehaviour;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out CharacterController2d charController))
        {
            SceneTransitionManager.Instance.TransitionToScene(sceneReference, transitionBehaviour);
        }
    }
}
