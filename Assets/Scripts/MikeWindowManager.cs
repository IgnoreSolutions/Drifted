/**
MikeWindowManager.cs

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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Drifted.Input;
using Drifted.Inventory;
using Drifted.Items.ItemDefinitions;
using Drifted.Items.Next;
using MikeSantiago.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Drifted.UI.WindowManager
{
    public struct ItemDropEventArgs
    {
        /// <summary>
        /// The item that was dropped.
        /// </summary>
        /// <value>The held item.</value>
        public Item HeldItem { get; set; }
    }

    /// <summary>
    /// Small helper class for handling what the mouse is holding in-game.
    /// 
    /// This will also eventually set the cursor.
    /// </summary>
    public class MikeMouseManager
    {

    }

    public class MouseDummyContainer
    {
        public GameObject MouseDummyParent;
        public Text MouseDummyStackText;
        public Image MouseDummyCursorImage;
        public Image MouseDummyHoldImage;
    }

    /// <summary>
    /// The game's basic window manager.
    /// </summary>
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class MikeWindowManager : MonoBehaviour
    {
        [HideInInspector]
        public static Font DefaultFont;

        public Canvas MasterCanvas;

        private List<GameObject> WindowList = new List<GameObject>();
        private MikeMouseManager MouseManager;

        private string MouseDummyName = "Mouse Dummy";
        private string MouseDummyHoldSpriteName = "Mouse Dummy Hold Sprite";
        private string MouseDummyStackCountName = "Mouse Dummy Stack Count Name";

        static Color DefaultBgColor = new Color(0.65f, 0.65f, 0.65f, 0.89f); // Slightly transparent black.
        static Color DefaultTextColor = new Color(1.0f, 1.0f, 1.0f, 1.0f); // White


        //public GameObject MouseDummy;
        [HideInInspector]
        public MouseDummyContainer MouseDummy;

        public GameObject BlankAreaClickDetector;


        //private PopUpMenuController MenuController;
        private ActivityConsole ActivityConsoleInst;
        private MikeMouseDummyController CursorHandler;

        [ReadOnly]
        public Rect ScreenRect;

        private void Start()
        {
            DriftedInputManager.SetupInputs();
            ScreenRect = new Rect(0, 0, Screen.width, Screen.height);
        }

        private void Awake()
        {
            WindowList = new List<GameObject>();
            MouseManager = new MikeMouseManager();

            //MasterCanvas = GetComponent<Canvas>();
            //if (MasterCanvas == null) throw new Exception("Couldn't get a hold of the master canvas!");

            /*
            var existingMouseDummy = GetChildByName(MouseDummyName);
            if (UnityEngine.Application.isPlaying)
            {
                DestroyImmediate(existingMouseDummy);
                MouseDummy = CreateMouseDummy();
                var mdCtrl = MouseDummy.MouseDummyParent.GetComponent<MikeMouseDummyController>();
                CursorHandler = mdCtrl;
            }
            */

            //MenuController = transform.gameObject.GetComponent<PopUpMenuController>();
            //ActivityConsoleInst = transform.gameObject.GetComponent<ActivityConsole>();

            //mdCtrl.MouseManager = this.MouseManager; // JUST TO BE FUCKING SURE.
            /*
            if(WindowManagerMaster == null)
            {
                var existing = GameObject.Find("UICanvas/Window Manager");
                if (existing == null) MakeMaster();
                else WindowManagerMaster = existing;

                WindowManagerMaster.transform.position = Vector3.zero;
            }
            */
            if(BlankAreaClickDetector != null) BlankAreaClickDetector.SetActive(false);
        }
        public MikeMouseDummyController GetMouseManager() => CursorHandler;
        public ActivityConsoleController GetConsole()
        {
            //if (ActivityConsoleInst != null) return ActivityConsoleInst;
            return null;
            //return DriftedConstants.Instance.UI().Console;
        }

        [Obsolete]
        public NewPopUpMenuController GetMenuController()
        {
            //if (!MenuController.IsTrueNull()) return MenuController;
            //return null;
            return DriftedConstants.Instance.UI().MenuController;
        }

        public Canvas GetCanvas()
        {
            if (!MasterCanvas.IsTrueNull()) return MasterCanvas;
            return null;
        }

        // Single Instance Controllers
        public MouseDummyContainer CreateMouseDummy()
        {
            MouseDummyContainer container = new MouseDummyContainer();

            GameObject mouseDummy = new GameObject(MouseDummyName);
            mouseDummy.transform.SetParent(MasterCanvas.transform);

            GameObject mouseDummyHoldSprite = new GameObject(MouseDummyHoldSpriteName);
            mouseDummyHoldSprite.transform.SetParent(mouseDummy.transform);
            var mdHoldSprite = mouseDummyHoldSprite.AddComponent<Image>();
            mdHoldSprite.sprite = null;
            mdHoldSprite.color = new Color(0.2f, 0.5f, 0.1f, 0.00f);
            mdHoldSprite.raycastTarget = false;
            mdHoldSprite.GetComponent<RectTransform>().sizeDelta = new Vector2(48f, 48f);


            GameObject mouseDummyStackCount = new GameObject(MouseDummyStackCountName);
            mouseDummyStackCount.transform.SetParent(mouseDummyHoldSprite.transform);

            var mdStackText = mouseDummyStackCount.AddComponent<Text>();
            DefaultFont = Resources.Load<Font>("Fonts/game");
            mdStackText.font = DefaultFont;
            mdStackText.color = DefaultTextColor;
            mdStackText.text = "0";
            mdStackText.raycastTarget = false;
            mouseDummyStackCount.SetActive(false);
            mouseDummyStackCount.GetComponent<RectTransform>().sizeDelta = mdHoldSprite.GetComponent<RectTransform>().sizeDelta;
            mouseDummyStackCount.transform.localPosition = new Vector3(-16f, 0f, 0f);


            Image mouseImage = mouseDummy.AddComponent<Image>();
            mouseImage.sprite = null; // TODO: custom cursor?
            mouseImage.raycastTarget = false;
            mouseImage.color = new Color(0f, 0f, 0f, 0f);
            mouseImage.enabled = false;


            RectTransform mouseDummyTransform = mouseDummy.GetComponent<RectTransform>();
            mouseDummyTransform.sizeDelta = new Vector2(64, 64);

            mouseDummy.AddComponent<WMStayOnTop>();

            container.MouseDummyParent = mouseDummy;
            container.MouseDummyHoldImage = mdHoldSprite;
            container.MouseDummyStackText = mdStackText;
            container.MouseDummyCursorImage = mouseImage;

            var mdCtrl = mouseDummy.AddComponent<MikeMouseDummyController>();
            GameObject virtualCursor = new GameObject("Cursor");
            virtualCursor.transform.SetParent(container.MouseDummyParent.transform);
            Image cursorImg = virtualCursor.AddComponent<Image>();
            cursorImg.raycastTarget = false;
            mdCtrl.Cursor = cursorImg;

            //mdCtrl.InventoryController = GetComponent<InventoryDisplayController>();
            
            mdCtrl.HeldSprite = container.MouseDummyHoldImage;
            mdCtrl.StackCountText = mdStackText;

            return container;
        }


        [ReadOnly] public bool BlankShouldBeActive = false;
        public void SetEnableBlankDetector(bool enabled)
        {
            BlankShouldBeActive = enabled;
            if (BlankAreaClickDetector != null) BlankAreaClickDetector.SetActive(enabled);
        }

        /// <summary>
        /// Creates a new inactive window.
        /// </summary>
        /// <returns>A GameObject which is the newly created window.</returns>
        /// <param name="size">Size.</param>
        /// <param name="title">Title.</param>
        /// <param name="bgColor">Background color.</param>
        public GameObject CreateWindow(Vector2 size, string title, Color? bgColor = null, Color? textColor = null, bool destroyOnClose = false, bool needsCloseButton = true, bool moveable = true, bool hasShadow = true)
        {
            if (MasterCanvas == null) throw new Exception("Master canvas can't be null");

            // TODO: this function is too big.

            var newWindow = new GameObject($"{title}");

            if (hasShadow)
            {
                var dropShadow = newWindow.AddComponent<Shadow>();
                dropShadow.effectColor = new Color(0f, 0f, 0f, .25f);
                dropShadow.effectDistance = new Vector2(7f, -7f);
            }

            var vertLayoutGroup = newWindow.AddComponent<VerticalLayoutGroup>();

            Image newWindowBackground = newWindow.AddComponent<Image>();

            newWindowBackground.color = bgColor ?? DefaultBgColor;

            var rectTransform = newWindow.GetComponent<RectTransform>();
            if (rectTransform == null) throw new Exception("No RectTransform on new window?");

            rectTransform.SetParent(MasterCanvas.transform);
            rectTransform.sizeDelta = size;
            rectTransform.localPosition = Vector3.zero;

            // Adds what makes a window a window.
            var windowControl = newWindow.AddComponent<WindowProperties>();
            windowControl.Moveable = moveable;
            windowControl.WindowTitle = title;
            windowControl.WindowSize = size;
            windowControl.DestroyOnClose = destroyOnClose;
            windowControl.NeedsCloseButton = needsCloseButton;
            windowControl.WindowColor = bgColor ?? DefaultBgColor;
            windowControl.TextColor = textColor ?? DefaultTextColor;
            windowControl.MakeTitlebar(size.x, size.y);

            WindowList.Add(newWindow);
            rectTransform.SetSiblingIndex(2);
            return newWindow;
        }

        public bool AddCustomWindow(GameObject window)
        {
            // Validation
            WindowProperties windowProperties = window.GetComponent<WindowProperties>();
            if (windowProperties == null) { Debug.LogError("Couldn't find window properties"); return false; }

            GameObject titleObject = GameObject.Find($"{window.name}/Window Title");
            if (titleObject == null) { Debug.LogError("Couldn't find title object."); return false; }
            // End

            WindowList.Add(window);

            return true;
        }

        public GameObject GetWindowByName(string name)
        {
            var windowGO = transform.Find(name).gameObject;
            if (windowGO != null) return windowGO;
            return null;
        }

        private GameObject CreateInformationWindow(string windowTitle, string informativeText, Sprite icon = null)
        {
            GameObject newWindow = CreateWindow(new Vector2(200, 200), windowTitle, Color.white, Color.black, true);

            // Building the info text
            var infoText = new GameObject("InfoText");
            infoText.transform.SetParent(newWindow.transform);
            var infoTextTrsnfrm = infoText.GetComponent<RectTransform>();
            var informationText = infoText.AddComponent<Text>();
            informationText.font = DefaultFont;
            informationText.text = informativeText;
            informationText.color = Color.black;

            var layoutProps = infoText.AddComponent<LayoutElement>();
            layoutProps.flexibleWidth = 6.0f;
            //

            // Sprite
            if (icon != null)
            {
                var itemSprite = new GameObject("ItemSprite");
                var itemSpriteTrnsfrm = itemSprite.GetComponent<RectTransform>();
                itemSpriteTrnsfrm.SetParent(newWindow.transform);

                var itemImg = itemSprite.AddComponent<Image>();
                itemImg.sprite = icon;
                itemImg.color = Color.white;

                itemSpriteTrnsfrm.sizeDelta = new Vector2(64f, 64f);
            }
            //

            return newWindow;
        }

        public void DestroyWindow(string name)
        {
            WindowList.Remove(WindowList.Where(x => x.name == name).FirstOrDefault());
            Destroy(GetWindowByName(name));
        }

        private void ClearExisting()
        {
            if (MasterCanvas == null) throw new Exception("Clear existing didn't work.");

            for (int i = 0; i < MasterCanvas.transform.childCount; i++)
            {
                GameObject child = MasterCanvas.transform.GetChild(i).gameObject;
                if (child != null)
                {
                    if (child.name == MouseDummyName)
                        DestroyImmediate(child);
                }
            }
        }

        private GameObject GetChildByName(string childName)
        {
            if (MasterCanvas == null) throw new Exception("Clear existing didn't work.");

            for (int i = 0; i < MasterCanvas.transform.childCount; i++)
            {
                GameObject child = MasterCanvas.transform.GetChild(i).gameObject;
                if (child != null)
                {
                    if (child.name == childName)
                        return child;
                }
            }

            return null;
        }

        internal void SetMouseFollowSprite(Sprite p)
        {
            Image img = MouseDummy.MouseDummyHoldImage;

            if (p == null)
            {
                img.sprite = null; img.color = new Color(0f, 0f, 0f, 0f);
            }
            else
            {
                img.sprite = p; img.color = Color.white;
            }
        }
    }
}