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
using Drifted.Items.Next;
using UnityEngine;
using UnityEngine.UI;
using Drifted;
using MikeSantiago.Extensions;

namespace Drifted.Crafting.UI
{

    /// <summary>
    /// Behaviour associated with any ingredient square in the crafting menu.
    /// </summary>
    public class IngredientSquareController : MonoBehaviour
    {
        [SerializeField]
        ItemContainer representedItem;

        [SerializeField]
        Image sprite;

        [SerializeField]
        Text stackCount;

        [SerializeField]
        Text presentCount;

        public NextGen.Inventory.Inventory inventoryInstance;

        void OnValidate()
        {
            UpdateHeldItem();
        }

        void UpdateHeldItem()
        {
            if (representedItem == null) return;
            if (sprite == null || stackCount == null || presentCount == null) return;

            Item theItem = representedItem.GetItem();
            if (theItem == null) return;

            sprite.sprite = theItem.Icon;
            stackCount.text = $"{representedItem.Quantity}x";

            if (HasInInventory())
            {
                sprite.color = Color.white;
                stackCount.color = Color.white;
                presentCount.enabled = true;
                int index = inventoryInstance.HasItemInInventory(representedItem);
                presentCount.text = $"Have {inventoryInstance.GetItemAt(index).Quantity}x";
                // jeeeeeeeeeeeeeeeeeeeesus.

            }
            else
            {
                sprite.color = Color.gray;
                stackCount.color = Color.red;
                presentCount.enabled = true;

                if(Application.isPlaying) presentCount.enabled = false;
            }
        }


        public void SetHeldItem(ItemContainer item, NextGen.Inventory.Inventory inventory)
        {
            inventoryInstance = inventory;
            representedItem = item;
            UpdateHeldItem();
        }

        public bool HasInInventory()
        {
            if (inventoryInstance == null) return false;
            return (inventoryInstance.HasItemAndCount(representedItem) > -1);
            //return (index > -1);
        }
    }

} 