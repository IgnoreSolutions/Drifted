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
using Drifted.Items.Next;
using UnityEngine;

namespace Drifted.Inventory
{
    /// <summary>
    /// Behaviour for dropping edible items onto Marty's image in the inventory. 
    /// </summary>
    public class InventoryMartyController : MonoBehaviour
    {
        public void HandleItemDrop()
        {
            if (DriftedConstants.Instance.Player().Movement.IsEating) return;

            ItemContainer mouseItem = DriftedConstants.Instance.UI().CursorHandler.GetHeldItem();

            if (mouseItem == null || mouseItem.GetItem() == null) return;

            if (mouseItem.Quantity > 0)
            {
                Item actualItem = mouseItem.GetItem();
                // Edible at all.
                bool ate = false;
                if (actualItem is EdibleItem)
                {
                    (actualItem as EdibleItem).Eat();
                    ate = true;
                }
                if (actualItem is PlantableFood)
                {
                    (actualItem as PlantableFood).Eat();
                    ate = true;
                }
                if (ate)
                {
                    DriftedConstants.Instance.UI().CursorHandler.GetHeldItem().Quantity--;
                    DriftedConstants.Instance.UI().CursorHandler.Refresh();
                }
            }
        }
    }

}

