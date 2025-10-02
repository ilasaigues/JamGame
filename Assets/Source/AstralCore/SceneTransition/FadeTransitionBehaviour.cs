using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
namespace AstralCore
{
    [CreateAssetMenu(menuName = "Scene Transitions/Fade")]
    public class FadeTransitionBehaviour : BaseSceneTransitionBehaviour
    {
        [SerializeField] private float duration = 0.5f;
        [SerializeField] private Color color = Color.black;

        private CanvasGroup canvasGroup;

        private void EnsureCanvas(Canvas parentCanvas)
        {
            if (canvasGroup != null) return;

            var go = new GameObject("Fader");
            var image = go.AddComponent<UnityEngine.UI.Image>();
            image.color = color;

            canvasGroup = go.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;

            DontDestroyOnLoad(go);
        }

        public override async Task EnterAsync(Canvas parentCanvas)
        {
            EnsureCanvas(parentCanvas);
            await FadeTo(1f);
        }

        public override async Task ExitAsync(Canvas parentCanvas)
        {
            EnsureCanvas(parentCanvas);
            await FadeTo(0f);
        }

        private async Task FadeTo(float target)
        {
            float start = canvasGroup.alpha;
            var fadeTween = DOTween.To(() => canvasGroup.alpha, v => canvasGroup.alpha = v, target, _transitionDuration);
            while (!fadeTween.IsComplete())
            {
                await Task.Delay(25);
            }
        }
    }
}
