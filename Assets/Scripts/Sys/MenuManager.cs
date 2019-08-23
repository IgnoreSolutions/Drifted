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

using System;
using System.Collections;
using System.Collections.Generic;
using Drifted.UI;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Menu
{
    public AbstractMenuItem[] MenuItems;
}

[CreateAssetMenu(menuName = "MENUUUUS/Menu Manager")]
public class MenuManager : ScriptableObject
{
    [Header("Template Setup")]
    public GameObject MenuTemplate;

    public GameEvent MenuAddedEvent;
    public BoolVariable MenusActive;
    Dictionary<int, PopUpMenuView> MenuControllers = new Dictionary<int, PopUpMenuView>();

    public Vector2 UIDropShadowOffset = new Vector2(7, -7);
    public Color UIDropShadowColor = Color.gray;
    public Font DefaultFont;

    public void OnEnable()
    {
        MenuControllers = new Dictionary<int, PopUpMenuView>();
        if(MenusActive == null) MenusActive = ScriptableObject.CreateInstance<BoolVariable>();
        if (DefaultFont == null) DefaultFont = Resources.GetBuiltinResource<Font>("Arial.ttf");

        if(MenusActive != null) MenusActive.Value = false;
    }

    public GameObject MakePopUpMenu(GameObject target, params AbstractMenuItem[] items)
    {
        if(MenuTemplate != null)
        {
            // TODO: Special pop up
            return null;
        }
        //GameObject popUpParent = new GameObject("Pop-Up Canvas");
        //popUpParent.transform.SetParent(parent.transform);

        //Canvas theCanvas = popUpParent.AddComponent<Canvas>();
        //theCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

        GameObject newPopup = new GameObject("Pop-Up Empty");

        Image bg = newPopup.AddComponent<Image>();
        bg.color = new Color(.47f, .47f, .47f, .95f);

        VerticalLayoutGroup verticalLayout = newPopup.AddComponent<VerticalLayoutGroup>();
        verticalLayout.childControlWidth = true;
        verticalLayout.childControlHeight = true;
        verticalLayout.childForceExpandWidth = true;
        verticalLayout.childForceExpandHeight = false;

        verticalLayout.spacing = 10;
        verticalLayout.padding = new RectOffset(5, 5, 5, 5);

        PopUpMenuView newPopupDisplay = newPopup.AddComponent<PopUpMenuView>();
        newPopupDisplay.DefaultFont = DefaultFont;
        newPopupDisplay.target = target;
        newPopupDisplay.menuManager = this;

        foreach (var newItem in items)
        {
            //if (parent != null)
            {
                // TODO: why does this throw an exception sometimes..?
                //Debug.Log("Set parent.");
                //Debug.Log(parent);
                //newItem.Parent = parent;
            }
            newPopupDisplay.AddItem(newItem);
        }

        var fitter = newPopup.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

        Shadow dropShadow = newPopup.AddComponent<Shadow>();
        dropShadow.effectDistance = UIDropShadowOffset;
        dropShadow.effectColor = UIDropShadowColor;

        newPopupDisplay.MenuDisplayed += () =>
        {
            newPopup.transform.SetAsLastSibling();
        };
        newPopupDisplay.MenuClosed += () =>
        {
            
            Debug.Log("Removing menu from internal dictionary");
            // TODO: remove from list based on menu ID
            RemoveMenu(newPopupDisplay.ID);
            Destroy(newPopup);
        };

        lock (MenuControllers)
        {
            while ((MenuControllers.ContainsKey(newPopupDisplay.ID))) newPopupDisplay.ID = UnityEngine.Random.Range(0, 100);

            MenuControllers.Add(newPopupDisplay.ID, newPopupDisplay);
        }

        newPopup.SetActive(false);
        MenuAddedEvent.Raise(newPopup);
        // 
        return newPopup;
    }

    public void CloseMenu(int id)
    {
        PopUpMenuView menuView = null;
        MenuControllers.TryGetValue(id, out menuView);
        if (menuView != null) Destroy(menuView.gameObject);

        RemoveMenu(id);
    }

    private void RemoveMenu(int id)
    {
        lock(MenuControllers)
        {
            MenuControllers.Remove(id);
        }
    }
}
