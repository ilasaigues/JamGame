using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    private Stack<UIWindow> windowStack = new Stack<UIWindow>();

    public InputActionReference BackAction;

    [SerializeField] private CanvasGroup darkenerPrefab;
    [SerializeField] private UIWindow BackUIPrefab;
    private CanvasGroup currentDarkener;


    void OnEnable()
    {
        BackAction.action.performed += OnBackPressed;
    }
    void OnDisable()
    {
        BackAction.action.performed -= OnBackPressed;
    }
    private void OnBackPressed(InputAction.CallbackContext context)
    {
        if (windowStack.Count != 0)
        {
            Pop();
        }
        else
        {
            Push(BackUIPrefab);
        }
    }

    public void Push(UIWindow windowPrefab)
    {
        if (windowStack.Count > 0)
            windowStack.Peek().SetInteractable(false);

        var window = Instantiate(windowPrefab, transform);
        window.SetUIManager(this);
        window.OnCloseRequested += Pop;
        windowStack.Push(window);

        UpdateDarkener();
    }

    public void Pop()
    {
        if (windowStack.Count == 0) return;

        var top = windowStack.Pop();
        Destroy(top.gameObject);

        if (windowStack.Count > 0)
            windowStack.Peek().SetInteractable(true);

        UpdateDarkener();
    }

    private void UpdateDarkener()
    {
        if (currentDarkener == null)
            currentDarkener = Instantiate(darkenerPrefab, transform);

        if (windowStack.Count > 1)
        {
            currentDarkener.transform.SetSiblingIndex(windowStack.Count - 2);
        }
        else
        {
            Destroy(currentDarkener.gameObject);
        }
    }
}
