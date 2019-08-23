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

using Drifted.Items.ItemDefinitions;
using Drifted.Items.Next;
using Drifted.UI.WindowManager;
using System;
using UnityEngine;

namespace Drifted.Inventory
{
    public class PlayerInventory : MonoBehaviour
    {
        public NextGen.Inventory.Inventory InventoryInstance;
        //public InventoryHandler Inventory;

        //public ItemContainer LeftHandEquip;
        //public ItemContainer HeadEquip;
        //public ItemContainer BodyEquip;
        //public ItemContainer PantsEquip;

        public void Awake()
        {
            //if (DriftedConstants.Instance == null) Debug.Log("Instance is null?");
            //DriftedConstants.Instance.OnSceneLoaded += () => Inventory.WindowManager = DriftedConstants.Instance.UI().WindowManager;

            //Inventory = new InventoryHandler();

            //LeftHandEquip = new ItemContainer();
            //HeadEquip = new ItemContainer();
            //BodyEquip = new ItemContainer();
            //PantsEquip = new ItemContainer();
        }

        public void SetInventory(InventoryHandler newInv)
        {
            //Inventory = newInv;
            //newInv.ForceRefresh();
        }

        //public void ResizeInventory(int newSize) => Inventory.ResizeInventory(newSize);
    }
}
