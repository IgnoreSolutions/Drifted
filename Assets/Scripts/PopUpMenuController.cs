/**
PopUpMenuController.cs

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
/*
using System;
using System.Collections;
using System.Collections.Generic;
using Drifted.UI.WindowManager;
using MikeSantiago.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Drifted.UI
{

    public interface IMenuItem
    {
        Action OnClickAction { get; set; }
        string MenuText { get; set; }
        IMenuItem SubItem { get; set; }
    }
    [DisallowMultipleComponent]
    public class MenuItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IMenuItem
    {
        public Action OnClickAction { get; set; }
        public Action OnHighlightAction { get; set; }
        public Action OnDehighlight { get; set; }
        public string MenuText { get; set; }

        public PopUpMenuController MenuController { get; set; }

        public bool HideOnClick { get; set; }

        public IMenuItem SubItem { get; set; } = null;
        private Text thisText;

        public void Start()
        {
            GameObject uiMaster = GameObject.FindWithTag("UIMaster");
            if (uiMaster == null) throw new Exception("Can't get a reference to the UIMaster.");
            MenuController = uiMaster.GetComponent<PopUpMenuController>();
            if (MenuController == null) throw new Exception("This shouldn't be null!!!!");

            thisText = GetComponent<Text>();
        }

        public void HideMenu() => MenuController.HidePopup();

        public void Highlight()
        {
            if(thisText != null) thisText.color = Color.green;
            if(OnHighlightAction != null)OnHighlightAction();
        }
        public void Unhighlight()
        {
            if(thisText != null) thisText.color = Color.black;
            if(OnDehighlight != null)OnDehighlight();
        }

        public void GiveAction(Action action) => OnClickAction = action;

        public void SetText(string text) => MenuText = text;

        public void OnPointerClick(PointerEventData eventData)
        {
            if(SubItem != null)
            {

            }

            if (OnClickAction != null)
            {
                OnClickAction();
                if(HideOnClick)HideMenu();
            }
            else throw new Exception("No on click action assigned!");
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (thisText != null) thisText.color = Color.green;
            else throw new Exception("Couldn't find text component!");
            if(OnHighlightAction != null) OnHighlightAction();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (thisText != null) thisText.color = Color.black;
            else throw new Exception("Couldn't find text component!");
            if(OnDehighlight != null) OnDehighlight();
        }
    }
    */
/*

[DisallowMultipleComponent]
public class PopUpMenuController : MonoBehaviour
{
    [HideInInspector]
    public bool IsPopUpActivated = false;

    [HideInInspector]
    public bool IsSubMenu = false;

    /// <summary>
    /// The mouse pop up offset from the mouse position to where the pop-up will appear.
    /// </summary>
    public Vector3 MousePopUpOffset = new Vector3(0, -35, 0);

    /// <summary>
    /// The GameObject that represents the pop-up and everything inside of it.
    /// </summary>
    [ReadOnly]
    private GameObject PopUpMaster;


    [HideInInspector]
    public MikeWindowManager WM { get; internal set; }

    private Font popUpFont;
    private int PopUpMenuItemCount = 0;
    private Vector2 MenuItemBaseSize = new Vector2(120f, 40f);

    public CanvasBlankArea BlankArea;

    public bool MenuCompleted = false;

    private int CurrentIndex = 0;

    private EzTimer menuDelayTimer;
    private bool allowedToShow = true;

    public event MenusClearedDelegate MenusCleared;
    public delegate void MenusClearedDelegate();

    public event MenusHiddenDelegate MenusHidden;
    public delegate void MenusHiddenDelegate();

    // Start is called before the first frame update
    void Start()
    {
        if (PopUpMaster.IsTrueNull()) throw new Exception("Pop Up master should've been created in Awake and it wasn't.");

        if (BlankArea != null)
        {
            BlankArea.BlankClicked += (eventData) =>
            {
                if (IsPopUpActivated) HidePopup();
            };
        }
    }

    public int GetItemCount() => PopUpMenuItemCount;

    private void Awake()
    {
        WM = GetComponent<MikeWindowManager>();

        PopUpMaster = new GameObject("Pop-Up");
        PopUpMaster.GetComponent<Transform>().SetParent(transform);

        var popUpShadow = PopUpMaster.AddComponent<Shadow>();
        popUpShadow.effectColor = new Color(0f, 0f, 0f, .25f);
        popUpShadow.effectDistance = new Vector2(7f, -7f);

        popUpFont = Resources.Load<Font>("Fonts/game");

        var popUpPanelBgImg = PopUpMaster.AddComponent<Image>();
        popUpPanelBgImg.color = new Color(.67f, .67f, .67f, .90f);

        RectTransform rt = PopUpMaster.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(0, 0);

        var verticalLayoutGroup = PopUpMaster.AddComponent<VerticalLayoutGroup>();

        PopUpMaster.AddComponent<WMStayOnTop>();
        PopUpMaster.AddComponent<WMStayOnScreen>();

        PopUpMaster.SetActive(false);

        // Timer
        keyDelayTimer = new EzTimer(.25f, () => allowedToPress = true, false);
        menuDelayTimer = new EzTimer(2.0f, () => allowedToShow = true, false);
    }

    // Update is called once per frame

    private EzTimer keyDelayTimer;
    private bool allowedToPress = true;

    void Update()
    {
        if (keyDelayTimer != null && keyDelayTimer.enabled) keyDelayTimer.Tick(Time.deltaTime);
        if (menuDelayTimer != null && menuDelayTimer.enabled) menuDelayTimer.Tick(Time.deltaTime);

        // Controller support
        if(PopUpMenuItemCount > 0 && allowedToPress)
        {
            if (CurrentIndex > (PopUpMenuItemCount - 1)) CurrentIndex = 0;

            if (CustomInputManager.GetCurrentMapping().IsController)
            {
                MenuItem previousItem = PopUpMaster.transform.GetChild(CurrentIndex).GetComponent<MenuItem>();
                if (previousItem != null) previousItem.Unhighlight();

                if(CustomInputManager.KeyDown(CustomInputManager.GetCurrentMapping().DpadUp) && allowedToPress)
                {
                    allowedToPress = false;
                    keyDelayTimer.Start();
                    CurrentIndex--;
                    if (CurrentIndex < 0) CurrentIndex = (PopUpMenuItemCount - 1);
                }
                else if(CustomInputManager.KeyDown(CustomInputManager.GetCurrentMapping().DpadDown) && allowedToPress)
                {
                    allowedToPress = false;
                    keyDelayTimer.Start();
                    CurrentIndex++;
                    if (CurrentIndex > (PopUpMenuItemCount - 1)) CurrentIndex = 0;
                }
                MenuItem item = PopUpMaster.transform.GetChild(CurrentIndex).GetComponent<MenuItem>();
                if (item != null) item.Highlight();


                if (CustomInputManager.KeyDown(CustomInputManager.GetCurrentMapping().OK) && allowedToPress) { item.OnClickAction(); if (item.HideOnClick) { HidePopup(); } }
            }
        }


        //WORK
        //InventoryWindow.GetComponent<Transform>().SetSiblingIndex(0);
        PopUpMaster.transform.SetSiblingIndex(3);
    }

    public void ShowPopupAtMouse()
    {
        if(CustomInputManager.IsController)
        {
            ShowPopup(Screen.width / 2, Screen.height / 2);
            return;
        }

        RectTransform rt = PopUpMaster.GetComponent<RectTransform>();
        Vector2 popUpLocation = new Vector2(Input.mousePosition.x + (rt.sizeDelta.x / 2) + 16f, Input.mousePosition.y - (rt.sizeDelta.y / 2) - 16f);
        ShowPopup((Vector3)popUpLocation);
    }

    public void ShowPopup(Vector2 pos)
    {
        ShowPopup(pos.x, pos.y);
    }

    public void ShowPopup(float x, float y)
    {
        // Move pop up to the very top
        if (MenuCompleted && allowedToShow)
        {
            allowedToShow = false;
            menuDelayTimer.Start();
            PopUpMaster.SetActive(true);

            //Debug.Log("SHOULD be active now.");
            PopUpMaster.transform.SetAsLastSibling();
            PopUpMaster.transform.position = new Vector3(x, y, 3);

            IsPopUpActivated = true;

            if (BlankArea != null)
            {
                BlankArea.transform.gameObject.SetActive(true);
                BlankArea.transform.SetAsFirstSibling();
            }
        }
        else Debug.LogWarning("Menu not complete before calling show.");
    }

    public void HidePopup()
    {
        //Debug.Log("Hide");
        ClearMenus();
        IsSubMenu = false;
        IsPopUpActivated = false;
        if (BlankArea != null)
        {
            BlankArea.transform.gameObject.SetActive(false);
        }
        PopUpMaster.SetActive(false);

        if (MenusHidden != null) MenusHidden();
    }

    public void ClearMenus()
    {
        var items = PopUpMaster.GetComponentsInChildren<Transform>();
        foreach (var itemToDestroy in items)
        {
            if (itemToDestroy.gameObject.GetComponent<MenuItem>() != null) //.name == "Menu Item"
            {
                Destroy(itemToDestroy.gameObject);
            }
        }
        PopUpMenuItemCount = 0;

        if (MenusCleared != null) MenusCleared();
    }

    public void DoneBuildingMenu()
    {
        MenuCompleted = true;
    }

    public MenuItem AddItemToMenu(Action onClickAction, string menuText, bool hideOnClick = true)
    {
        if (PopUpMaster.IsTrueNull()) throw new Exception("PopUpMaster is null. Stop the game and fix it. Make sure PopUpMenuController is working right.");
        if (MenuCompleted)
        {
            ClearMenus();
            MenuCompleted = false;
        }
        var newMenuGameObject = new GameObject($"{menuText} Menu");

        newMenuGameObject.AddComponent<LayoutElement>();

        Text newText = newMenuGameObject.AddComponent<Text>();
        newText.font = popUpFont;
        newText.text = menuText;
        newText.color = Color.black;

        MenuItem newMenuItem = newMenuGameObject.AddComponent<MenuItem>();
        newMenuItem.GiveAction(onClickAction);
        newMenuItem.HideOnClick = hideOnClick;

        newMenuGameObject.transform.localPosition = Vector3.zero;
        // this throws
        //if (PopUpMaster.transform.IsTrueNull()) Debug.LogError("Pop up master transform null");
        newMenuGameObject.transform.SetParent(PopUpMaster.transform);
        newMenuGameObject.transform.localPosition = new Vector3(0, -(MenuItemBaseSize.y * PopUpMenuItemCount), 0);
        newMenuGameObject.GetComponent<RectTransform>().sizeDelta = MenuItemBaseSize;
        //newMenuGameObject.transform.position = Vector3.zero;

        PopUpMenuItemCount++;

        newMenuGameObject.SetActive(true);

        return newMenuItem;
    }
}
}
*/