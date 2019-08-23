/**

WindowProperties.cs

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

using System;
using Drifted.UI.WindowManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Drifted.UI
{
    [DisallowMultipleComponent]
    public class WindowProperties : MonoBehaviour, IDragHandler, IPointerClickHandler, IEndDragHandler, IPointerUpHandler
    {
        public Color TextColor = Color.black;
        public Color WindowColor = Color.white;
        public string WindowTitle = "Window Title";
        public Vector2 WindowSize = Vector2.zero;

        //public readonly string WindowTitle = "MWM Window";
        public bool Moveable = true;
        public bool NeedsCloseButton = true;
        public bool DestroyOnClose = false;

        public bool IgnoreLayout = false;

        public bool HiddenByDefault = false;

        private GameObject Titlebar;

        private bool CurrentlyDragging = false;

        void Awake()
        {
            Transform titlebar = transform.Find("Window Title Text");
            if (titlebar) Titlebar = titlebar.gameObject;
            if (WindowSize == Vector2.zero)
            {
                RectTransform thisTransform = GetComponent<RectTransform>();
                if (thisTransform == null) throw new Exception("Needs RectTransform in order to accept WindowProperties!");
                WindowSize = thisTransform.sizeDelta;
            }
        }

        public void MakeTitlebar(float windowWidth, float windowHeight)
        {
            if (Titlebar != null) DestroyImmediate(Titlebar);

            var titleTextGo = new GameObject();
            titleTextGo.name = "Window Title Text";
            var layout = titleTextGo.AddComponent<LayoutElement>();
            layout.minHeight = 20f;

            Text titleText = titleTextGo.AddComponent<Text>();

            titleText.transform.SetParent(titleTextGo.transform);
            titleText.transform.localPosition = Vector3.zero;
            titleText.color = TextColor;

            titleText.font = Resources.Load<Font>("Fonts/game");
            titleText.name = "Window Title";
            titleText.text = WindowTitle;

            titleText.alignment = TextAnchor.MiddleCenter;

            if (NeedsCloseButton)
            {
                var closeButtonGO = new GameObject();
                closeButtonGO.name = "Close Button";

                Text closeButton = closeButtonGO.AddComponent<Text>();
                closeButtonGO.AddComponent<LayoutElement>();
                closeButton.font = Resources.Load<Font>("Fonts/game");
                closeButton.text = "x";
                closeButton.alignment = TextAnchor.MiddleCenter;
                closeButton.resizeTextForBestFit = true;
                closeButton.color = TextColor;

                var closeController = closeButton.transform.gameObject.AddComponent<MikeCloseButtonController>();
                closeController.DestroyOnClose = DestroyOnClose;
                closeController.WindowName = this.name;
                closeController.WindowManager = GameObject.FindWithTag("UIMaster").GetComponent<MikeWindowManager>();
                closeController.CloseButtonText = closeButton;

                closeButtonGO.transform.SetParent(titleTextGo.transform);

                var closeButtonTransform = closeButtonGO.GetComponent<RectTransform>();

                closeButtonTransform.anchorMin = new Vector2(1, 1f);
                closeButtonTransform.anchorMax = new Vector2(1f, 1f);
                closeButtonTransform.pivot = new Vector2(1f, 1f);
                closeButtonTransform.sizeDelta = new Vector2(20f, 20f);
                closeButtonTransform.anchoredPosition = Vector2.zero;



                if (CustomInputManager.IsController) closeButtonGO.SetActive(false); // Disable close button by default when joystick is connected.
            }

            RectTransform titleTextGoRectTrans = titleTextGo.GetComponent<RectTransform>();

            titleTextGoRectTrans.SetParent(transform);
            titleTextGoRectTrans.anchorMin = new Vector2(0f, 1f);
            titleTextGoRectTrans.anchorMax = new Vector2(1f, 1f);
            titleTextGoRectTrans.pivot = new Vector2(.5f, 1);
            titleTextGoRectTrans.offsetMin = Vector2.zero;
            titleTextGoRectTrans.offsetMax = Vector2.zero;
            titleTextGoRectTrans.localScale = new Vector3(1, 1, 1);

            titleTextGoRectTrans.sizeDelta = new Vector2(titleTextGoRectTrans.sizeDelta.x, layout.minHeight);


            if (HiddenByDefault) transform.gameObject.SetActive(false);

            titleTextGo.transform.SetAsFirstSibling();
        }

        public void SetWindowColor(Color newColor)
        {
            WindowColor = newColor;
            var bgImg = GetComponent<Image>();
            if (bgImg != null) bgImg.color = newColor;
        }

        public void SetTextColor(Color newTextColor)
        {
            TextColor = newTextColor;
            var titleTextGo = transform.GetChild(0);
            if (titleTextGo.name == "Window Title") titleTextGo.GetComponent<Text>().color = newTextColor;
            //else throw new Exception("Window Title GameObject not found?!");
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (Moveable)
            {
                transform.position += (Vector3)eventData.delta;
                CurrentlyDragging = true;
                transform.SetAsLastSibling();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            transform.SetAsLastSibling();

            //if(DriftedConstants.Instance.MenusActive) DriftedConstants.Instance.UI().MenuController.CloseAll();
        }

        public void OnEndDrag(PointerEventData eventData) => CurrentlyDragging = false;

        public void OnPointerUp(PointerEventData eventData)
        {
            if(eventData.button == PointerEventData.InputButton.Left) transform.SetAsLastSibling();
        }

        public bool IsCurrentlyDragging => CurrentlyDragging;
    }
}