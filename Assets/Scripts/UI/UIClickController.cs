/*
    Copyright (C) 2019 Mike Santiago - All Rights Reserved
    axiom@ignoresolutions.xyz

    Permission to use, copy, modify, and/or distribute this software for any
    purpose with or without fee is hereby granted, provided that the above
    copyright notice and this permission notice appear in all copies.

    THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
    WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
    MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
    ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
    WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
    ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF
    OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
*/

using Drifted;
using Drifted.Input;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//namespace Drifted.UI
//{
    /// <summary>
    /// Behaviour that allows you to define 
    /// </summary>
    [RequireComponent(typeof(Selectable))]
    [RequireComponent(typeof(RectTransform))]
    public class UIClickController : MonoBehaviour, IPointerClickHandler, IDeselectHandler, ISelectHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public UnityEvent OnLeft;
        public UnityEvent OnRight;
        public UnityEvent OnSelect;
        public UnityEvent OnDeselect;

        public UnityEvent OnHighlight;
        public UnityEvent OnDehighlight;

        private Selectable selectable;

        [ReadOnly]
        [SerializeField]
        private bool Selected = false;

        private void Awake()
        {
            selectable = GetComponent<Selectable>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (DriftedInputManager.IsController) return;

            if (eventData.button == PointerEventData.InputButton.Left) PrimaryInteract();
            else if (eventData.button == PointerEventData.InputButton.Right) SecondaryInteract();
        }

        private void Update()
        {
            if(DriftedInputManager.IsController == false) return;

            Selected = (EventSystem.current.currentSelectedGameObject == gameObject);

            if (DriftedInputManager.IsController && Selected)
            {
                if (DriftedInputManager.KeyDown("UISubmit")) PrimaryInteract();
                else if (DriftedInputManager.KeyDown("UISecondaryAction")) SecondaryInteract();
            }
        }

        public void PrimaryInteract()
        {
            OnLeft?.Invoke();
        }

        public void SecondaryInteract()
        {
            OnRight?.Invoke();
        }

        void IDeselectHandler.OnDeselect(BaseEventData eventData)
        {
            OnDeselect?.Invoke();
        }

        void ISelectHandler.OnSelect(BaseEventData eventData)
        {
            OnSelect?.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (CustomInputManager.IsController) return;

            OnHighlight?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (CustomInputManager.IsController) return;
            OnDehighlight?.Invoke();
        }
    }
//}