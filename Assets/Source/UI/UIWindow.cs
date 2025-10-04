using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CanvasGroup))]
public class UIWindow : MonoBehaviour
{
    public event Action OnCloseRequested;
    protected CanvasGroup _canvasGroup;
    private UIManager _UIManager;
    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetInteractable(bool state)
    {
        _canvasGroup.interactable = state;
        _canvasGroup.blocksRaycasts = state;
    }

    public void Close()
    {
        Close(default);
    }

    public void Close(InputAction.CallbackContext _)
    {
        OnCloseRequested?.Invoke();
    }

    public void SetUIManager(UIManager uiManager)
    {
        _UIManager = uiManager;
    }

    public void OpenOther(UIWindow newWindowPrefab)
    {
        _UIManager.Push(newWindowPrefab);
    }
}
