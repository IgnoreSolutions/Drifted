﻿/**

InvInfoSquare.cs

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
using Drifted.Items.ItemDefinitions;
using Drifted.Items.Next;

namespace Drifted.UI
{
    public class InvInfoSquare : MonoBehaviour
    {
        public NextGen.Inventory.Inventory playerInventory;
        [SerializeField]
        ActivityConsoleManager Console;

        public Image BgImage;

        private Color oldColor;

        void Awake()
        {
            BgImage = GetComponent<Image>();
        }

        public void OnInteract()
        {
            ItemContainer heldItem = playerInventory.itemInMouse;
            if (heldItem == null) Console.AddLine("Uhhhh");
            if(heldItem.GetItem() != null || heldItem.Quantity > 0) Console.AddLine($"<b>{heldItem.GetItem().ItemName}</b>: {heldItem.GetItem().Description}");
        }
    }

}