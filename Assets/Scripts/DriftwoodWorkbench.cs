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
using Drifted;
using Drifted.Interactivity;
using Drifted.UI;
using UnityEngine;

namespace Drifted
{

    public class DriftwoodWorkbench : DriftedSceneInteractable
    {
        [SerializeField]
        GameEvent ShowCraftingUIEvent;

        [SerializeField]
        PlayerMovement player;

        [SerializeField]
        CameraFollow mainCam;

        protected override void Awake()
        {
            base.Awake();

            if (menuManager == null) menuManager = (MenuManager)ScriptableObject.CreateInstance(typeof(MenuManager));

            player = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
            mainCam = Camera.main.GetComponent<CameraFollow>();
        }

        private int menuID = -1;
        public override void Interact(MonoBehaviour sender)
        {
            if (menuID == -1)
            {
                BuildMenu();
            }
        }

        public bool CraftRoutine()
        {
            player.EnqueueAction(player.MovePlayerTo(transform));
            player.EnqueueWait(0.3f);
            player.EnqueueAction(() => ShowCraftingUIEvent.Raise());
            return true;
        }

        public void BuildMenu()
        {
            List<AbstractMenuItem> menuItems = new List<AbstractMenuItem>();

            menuItems.Add(new MenuItem("Craft", CraftRoutine, false));
            menuManager.MakePopUpMenu(gameObject, menuItems.ToArray());
        }
    }
}