using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Events;

namespace AstralCore
{
    public class MenuNavigator : MonoBehaviour
    {
        [Header("References")]
        public List<Selectable> Selectables = new();
        [SerializeField] protected Selectable _firstSelected;

        [Header("Controls")]
        [SerializeField] protected InputActionReference _navigateReference;

        [Header("Animations")]
        [SerializeField] protected float _selectedAnimationScale = 1.1f;
        [SerializeField] protected float _scaleDuration = 0.25f;
        [SerializeField] protected List<GameObject> _animationExclusions = new();

        [Header("Sounds")]
        [SerializeField] protected UnityEvent SelectSoundEvent;

        protected Dictionary<Selectable, Vector3> _scales = new();

        protected Selectable _lastSelected;

        protected Tween _scaleUpTween;
        protected Tween _scaleDownTween;

        void Awake()
        {
            Selectables = GetComponentsInChildren<Selectable>().ToList();
            foreach (var selectable in Selectables)
            {
                AddListenersToSelectable(selectable);
                _scales.Add(selectable, selectable.transform.localScale);
            }
        }

        protected virtual IEnumerator SelectAfterDelay()
        {
            yield return null;
            EventSystem.current.SetSelectedGameObject(_firstSelected != null ? _firstSelected.gameObject : null ?? Selectables[0].gameObject);
        }

        public virtual void OnEnable()
        {
            _navigateReference.action.performed += OnNavigate;

            //ensure selectables are reset back to original scale
            foreach (var selectable in Selectables)
            {
                selectable.transform.localScale = _scales[selectable];
            }
            StartCoroutine(SelectAfterDelay());

        }

        public virtual void OnDisable()
        {
            _navigateReference.action.performed -= OnNavigate;

            _scaleUpTween.Kill();
            _scaleDownTween.Kill();
        }

        protected virtual void AddListenersToSelectable(Selectable selectable)
        {
            //add event listener
            EventTrigger trigger = selectable.gameObject.GetOrAddComponent<EventTrigger>();

            //add SELECT event
            EventTrigger.Entry SelectEntry = new()
            {
                eventID = EventTriggerType.Select,
            };
            SelectEntry.callback.AddListener(OnSelectChild);
            trigger.triggers.Add(SelectEntry);


            //add DESELECT event
            EventTrigger.Entry DeselectEntry = new()
            {
                eventID = EventTriggerType.Deselect,
            };
            DeselectEntry.callback.AddListener(OnDeselectChild);
            trigger.triggers.Add(DeselectEntry);


            //add ONPOINTERENTER event
            EventTrigger.Entry PointerEnter = new()
            {
                eventID = EventTriggerType.PointerEnter,
            };
            PointerEnter.callback.AddListener(OnPointerEnterChild);
            trigger.triggers.Add(PointerEnter);

            //add ONPOINTEREXIT event
            EventTrigger.Entry PointerExit = new()
            {
                eventID = EventTriggerType.PointerExit,
            };
            PointerExit.callback.AddListener(OnPointerExitChild);
            trigger.triggers.Add(PointerExit);

        }

        public void OnSelectChild(BaseEventData eventData)
        {
            SelectSoundEvent?.Invoke();
            _lastSelected = eventData.selectedObject.GetComponent<Selectable>();

            if (_animationExclusions.Contains(eventData.selectedObject))
                return;

            //Animate
            Vector3 newScale = eventData.selectedObject.transform.localScale * _selectedAnimationScale;
            _scaleUpTween = eventData.selectedObject.transform.DOScale(newScale, _scaleDuration);
        }

        public void OnDeselectChild(BaseEventData eventData)
        {
            if (_animationExclusions.Contains(eventData.selectedObject))
                return;

            //Animate 
            Selectable sel = eventData.selectedObject.GetComponent<Selectable>();
            _scaleDownTween = eventData.selectedObject.transform.DOScale(_scales[sel], _scaleDuration);
        }


        public void OnPointerEnterChild(BaseEventData eventData)
        {
            if (eventData is PointerEventData pointerEventData)
            {
                Selectable sel = pointerEventData.pointerEnter.GetComponentInParent<Selectable>();
                if (sel == null)
                {
                    sel = pointerEventData.pointerEnter.GetComponentInChildren<Selectable>();
                }
                pointerEventData.selectedObject = sel.gameObject;
            }
        }
        public void OnPointerExitChild(BaseEventData eventData)
        {
            if (eventData is PointerEventData pointerEventData)
            {
                pointerEventData.selectedObject = null;
            }
        }

        protected virtual void OnNavigate(InputAction.CallbackContext context)
        {
            if (EventSystem.current.currentSelectedGameObject == null && _lastSelected != null)
            {
                EventSystem.current.SetSelectedGameObject(_lastSelected.gameObject);
            }
        }
    }
}
