using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialoguePlayer : MonoBehaviour
{
    public Image ActorPortrait;
    public TextMeshProUGUI ActorNameBox;
    public TextMeshProUGUI TextBox;

    public InputActionReference[] ContinueInputs;

    Queue<DialogueAsset> _dialogueQueue = new();

    Coroutine DialogueCoroutine;

    public bool IsInDialogue { get; private set; }

    private bool _continue;

    private bool ContinuePressed
    {
        get
        {
            if (_continue)
            {
                _continue = false;
                return true;
            }
            return false;
        }
    }

    private void Start()
    {
        foreach (var inputRef in ContinueInputs)
        {
            inputRef.action.performed += PressContinue;
        }
    }

    private void PressContinue(InputAction.CallbackContext _)
    {
        _continue = true;
    }

    public void EnqueueDialogue(DialogueAsset dialogueData)
    {
        gameObject.SetActive(true);
        _dialogueQueue.Enqueue(dialogueData);
        IsInDialogue = true;
        DialogueCoroutine ??= StartCoroutine(RunThroughDialogue());
    }

    WaitForSeconds waitForSeconds = new WaitForSeconds(0.05f);
    public IEnumerator RunThroughDialogue()
    {
        if (_dialogueQueue.Count == 0)
        {
            IsInDialogue = false;
            yield return null;
        }
        var currentDialogue = _dialogueQueue.Dequeue();
        while (currentDialogue != null)
        {
            ActorPortrait.sprite = currentDialogue.ActorSprite;
            ActorNameBox.text = currentDialogue.ActorName;
            TextBox.text = currentDialogue.Text;
            TextBox.maxVisibleCharacters = 0;
            var targetCharacters = currentDialogue.Text.Length;
            while (TextBox.maxVisibleCharacters < targetCharacters)
            {
                TextBox.maxVisibleCharacters++;
                if (ContinuePressed)
                {
                    TextBox.maxVisibleCharacters = targetCharacters;
                }
                yield return waitForSeconds;
            }
            yield return new WaitUntil(() => ContinuePressed);
            if (currentDialogue.NextDialogue != null)
            {
                currentDialogue = currentDialogue.NextDialogue;
            }
            else if (_dialogueQueue.Count > 0)
            {
                currentDialogue = _dialogueQueue.Dequeue();
            }
            else
            {
                currentDialogue = null;
            }
            yield return waitForSeconds;
        }
        DialogueCoroutine = null;
        IsInDialogue = false;
        gameObject.SetActive(false);
    }


}
