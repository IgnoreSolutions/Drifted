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
using Drifted;
using Drifted.Interactivity;
using Drifted.UI;
using UnityEngine;

public class BoarClickBehaviour : DriftedSceneInteractable
{
    NavMeshBoar2 behaviour;

    protected override void Awake()
    {
        base.Awake();

        behaviour = GetComponent<NavMeshBoar2>();
    }

    public override void Interact(MonoBehaviour sender)
    {
        base.Interact(sender);

        List<AbstractMenuItem> menuItems = new List<AbstractMenuItem>();

        //if(DriftedConstants.Instance.UI().Skills.Hunting.Level > 3)
        //{
            menuItems.Add(new MenuItem("Domesticate", () => 
            {
                float domesticateChance = Mathf.Clamp(Skills.Hunting.Level * 2, 15, 100);
                float chance = UnityEngine.Random.Range(1, 40);

                if(chance <= domesticateChance)
                {
                    Skills.Hunting.GainExp(30);
                    DriftedConstants.Instance.UI().Console.AddLine("You gained <color=red>30 exp</color> in <b>Hunting</b>!\n...And a new best friend!!");
                    // TODO: this
                }
                else
                {
                    DriftedConstants.Instance.UI().Console.AddLine("You were unable to domesticate the wild boar.");
                }

                return true; 
            }, false));
        //}

        if(menuItems.Count > 0)
        {
            //var popUpMenu = DriftedConstants.Instance.UI().MenuController.MakePopUpMenu(this, menuItems.ToArray());
            //popUpMenu.AutoShowPopup(gameObject);
        }
    }
}