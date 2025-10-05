using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AstralCore
{
    [RequireComponent(typeof(Canvas))]
    public class SceneTransitionManager : MonoBehaviour
    {
        [SerializeField] private Canvas TransitionCanvasContainer;

        public static SceneTransitionManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                DestroyImmediate(gameObject);
                return;
            }
            Instance = this;
            TransitionCanvasContainer = GetComponent<Canvas>();
            TransitionCanvasContainer.enabled = false;
        }

        public async void TransitionToScene(SceneReference targetSceneReference, BaseSceneTransitionBehaviour transition)
        {
            //Declare the scene transition operation without awaiting it
            var loadOperation = SceneManager.LoadSceneAsync(targetSceneReference.ToString());
            //block scene activation preemptively
            loadOperation.allowSceneActivation = false;
            //enable the transition canvas
            TransitionCanvasContainer.enabled = true;
            //wait for the first half of the transition to happen
            await transition.EnterAsync(TransitionCanvasContainer);
            //allow for the new scene to activate when ready
            loadOperation.allowSceneActivation = true;
            //await for the scene loading to happen
            await loadOperation;
            //wait for the second half of the transition to happen
            await transition.ExitAsync(TransitionCanvasContainer);
            //disable the transition canvas
            TransitionCanvasContainer.enabled = false;
        }
    }
}
