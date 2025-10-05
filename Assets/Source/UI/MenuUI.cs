using AstralCore;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MenuUI : MonoBehaviour
{
    [SerializeField] BaseSceneTransitionBehaviour sceneTransitionBehaviour;
    UIManager uiManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        uiManager = FindFirstObjectByType<UIManager>();
    }


    public void ChangeScene(SceneReference sceneReference)
    {
        SceneTransitionManager.Instance.TransitionToScene(sceneReference, sceneTransitionBehaviour);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#endif
    }

}
