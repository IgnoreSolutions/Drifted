/**

InvTrashSquare.cs

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

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Drifted.UI.WindowManager;
using Drifted.Inventory;
using System;
using Drifted.Items.ItemDefinitions;
using Drifted.Items.Next;

namespace Drifted.UI
{
    public class InvTrashSquare : MonoBehaviour 
    {
        [SerializeField]
        NextGen.Inventory.Inventory playerInventory;

        [SerializeField]
        ActivityConsoleManager Console;

        public Image BgImage;

        private Color oldColor;
        private Color Transparent = new Color(0f, 0f, 0f, 0f);

        private ItemContainer HeldItem { get; set; }

        void Awake()
        {
            oldColor = BgImage.color;
        }

        public void SimulateLeftClick()
        {
            if (playerInventory == null) return;

            if (playerInventory.itemInMouse != null) // If the mouse is holding something.
            {
                ItemContainer itemFromMouse = playerInventory.itemInMouse;
                playerInventory.SetItemInMouse(null);
                Console.AddLine($"Trashed {itemFromMouse.Quantity}x {itemFromMouse.GetItem().ItemName}");
                HeldItem = itemFromMouse;
                UpdateTrashIcon();
            }
            else if (playerInventory.itemInMouse == null && HeldItem != null) // Item in trash square but mouse not holding anything
            {
                playerInventory.SetItemInMouse(HeldItem);
                HeldItem = null;
                UpdateTrashIcon();
            }
        }

        private void UpdateTrashIcon()
        {
            if (HeldItem == null)
            {
                BgImage.sprite = null;
                BgImage.color = Transparent;
            }
            else
            {
                BgImage.sprite = HeldItem.GetItem().Icon;
                BgImage.color = Color.white;
            }
        }
    }
}