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

using System.Collections;
using System.Collections.Generic;
using Drifted.Input;
using Drifted.UI;
using UnityEngine;

public class MenuListener : MonoBehaviour
{
    public Vector3 Offset = Vector3.zero;

	public BoolReference menusActive;

    void Awake()
    {
        if(menusActive == null) menusActive = new BoolReference();
    }

    private int menuCount = 0;

    void UpdateMenusActive()
    {
        menusActive.Value = (menuCount > 0);
    }

    public void AddMenu(GameObject newMenu)
    {
        PopUpMenuView menuView = newMenu.GetComponent<PopUpMenuView>();

        menuView.MenuClosed += () => 
        {
            Debug.Log("Menu Closed");
            menuCount--;
            if(menuCount < 0) menuCount = 0;
            UpdateMenusActive();

            if(!menusActive.Value) DriftedInputManager.SetPlayActive();
        };
        newMenu.transform.SetParent(transform);
        newMenu.SetActive(true);
        if(DriftedInputManager.IsController) DriftedInputManager.SetUIActive();
        Debug.Log("Menu Opened");
        menuCount++;
        UpdateMenusActive();
    }
}
