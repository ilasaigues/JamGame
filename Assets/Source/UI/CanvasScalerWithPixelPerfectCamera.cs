using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Canvas))]
[ExecuteAlways]
[DisallowMultipleComponent]
public class CanvasScalerWithPixelPerfectCamera : MonoBehaviour
{
    private Canvas canvas;
    private PixelPerfectCamera pixelPerfectCamera;

    private void OnEnable()
    {
        canvas = GetComponent<Canvas>();
        pixelPerfectCamera = Camera.main.GetComponent<PixelPerfectCamera>();

        HandleWillRenderCanvases();

        Canvas.preWillRenderCanvases += HandleWillRenderCanvases;
    }

    private void OnDisable()
    {
        Canvas.preWillRenderCanvases -= HandleWillRenderCanvases;
    }

    private void HandleWillRenderCanvases()
    {
        // Guard (just in case)
        if (pixelPerfectCamera == null) return;

        try
        {
            // Update the canvas' scale factor
            canvas.scaleFactor = pixelPerfectCamera.pixelRatio;
        }
        // Because in PixelPerfectCamera.cs, line 249, m_Internal could be null
        // and they don't have a guard for that
        catch (System.NullReferenceException)
        {
            // Log
            //Debug.Log("Pixel perfect camera's pixel ratio not (yet) available.");
        }
    }
}
