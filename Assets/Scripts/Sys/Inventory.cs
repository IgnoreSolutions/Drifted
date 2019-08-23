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
using Drifted.Items.Next;
using UnityEngine;

namespace Drifted.NextGen.Inventory
{
    [CreateAssetMenu(menuName = "Drifted/Inventory")]
    public class Inventory : ScriptableObject
    {
        public ItemDB MasterDatabase;

        [SerializeField]
        ItemContainer[] Items;

        public ItemContainer itemInMouse;

        public GameEvent ItemToMouseEvent;

        public int InventorySize() => Items.Length;

        public event InventoryUpdated InventoryChanged;
        public delegate void InventoryUpdated();

        public void MoveItemToMouse(int index)
        {
            if (index > Items.Length) return;
            if (index < 0) return;
            itemInMouse = Items[index];
            Items[index] = null;

            if(ItemToMouseEvent != null) ItemToMouseEvent.Raise();
        }

        public void SetItemInMouse(ItemContainer item)
        {
            itemInMouse = item;
            if (ItemToMouseEvent != null) ItemToMouseEvent.Raise();
        }

        public void MoveItemFromMouseTo(int indexTo)
        {
            if (indexTo > Items.Length) return;
            if (indexTo < 0) return;
            if (itemInMouse == null) return;

            Items[indexTo] = itemInMouse;
            itemInMouse = null;
            if (ItemToMouseEvent != null) ItemToMouseEvent.Raise();
        }

        public void NullItemAt(int index)
        {
            Items[index].SetItem(null);
            Items[index].Quantity = 0;
        }

        public void SetItemAt(ItemContainer container, int index)
        {
            Items[index] = container;
            InventoryChanged?.Invoke();
        }

        public void SetItemAt(Item item, int index)
        {
            Items[index].SetItem(item);
            InventoryChanged?.Invoke();
        }

        public ItemContainer GetItemAt(int index = 0)
        {
            if (index > 0 && index > Items.Length - 1) return null;
            return Items[index];
        }

        public void ResizeInventory(int newSize)
        {
            ItemContainer[] _emptyNewSize = new ItemContainer[newSize];
            if (newSize < Items.Length)
            {
                for (int i = 0; i < newSize; i++) _emptyNewSize[i] = Items[i];
            }
            else
            {
                for (int i = 0; i < Items.Length; i++) _emptyNewSize[i] = Items[i];
            }

            Items = _emptyNewSize;
        }

        public int HasItemInInventory(Item item)
        {
            for (int i = 0; i < Items.Length; i++)
            {
                var invItem = Items[i];
                if (item == invItem.GetItem()) return i;
            }
            return -1;
        }

        public int HasItemInInventory(ItemContainer item)
        {
            return HasItemInInventory(item.GetItem());
        }

        public int HasItemAndCount(ItemContainer item)
        {
            for (int i = 0; i < Items.Length; i++)
            {
                int index = HasItemInInventory(item);

                if (index != -1)
                {
                    ItemContainer fromInventory = Items[index];
                    if (fromInventory.Quantity >= item.Quantity) return index;
                }
            }
            return -1;
        }

        public int HasItemInInventory(int id)
        {
            if (Items == null) throw new Exception("Items was null");

            for (int i = 0; i < Items.Length; i++)
            {
                var item = Items[i];
                // TODO:
                //if (item != null && item.name == id) return i;
                if (item != null && item.GetItem().ItemName == Items[i].GetItem().ItemName) return i;
            }

            return -1;
        }

        public int HasItemInInventory(int id, int quantity)
        {
            int item = HasItemInInventory(id);
            if (item > -1)
            {
                if (Items[item].Quantity >= quantity) return item;
            }

            return -1;
        }

        public int HasItemInInventory(ItemContainer item, bool matchQuantity)
        {
            int index = HasItemInInventory(item);
            if (index == -1) return -1;
            if (matchQuantity)
            {
                if (Items[index].Quantity >= item.Quantity) return index;
            }
            else return index;

            return -1;
        }

        public int GetNextFreeInventorySpace()
        {
            for (int i = 0; i < Items.Length; i++) if (Items[i].GetItem() == null) return i;
            return -1;
        }

        public bool MoveItemInInventory(int oldSlot, int newSlot)
        {
            if (newSlot > Items.Length || oldSlot > Items.Length || newSlot < 0 || oldSlot < 0) return false;

            ItemContainer itemInOldSlot = Items[oldSlot];
            ItemContainer itemInNewSlot = Items[newSlot];

            if (itemInOldSlot != null && itemInNewSlot == null) // Direct move
            {
                Items[newSlot] = Items[oldSlot].Copy(); // TODO: necessary?
                Items[oldSlot] = null;
                InventoryChanged?.Invoke(); ;
                return true;
            }
            else if (itemInOldSlot != null && itemInNewSlot != null) // swap item in mouse for item in inventory
            {
                //var itemInHand = WindowManager.GetMouseManager().GetHeldItem();

                Items[newSlot] = Items[oldSlot];
                var itemToPutInHand = Items[newSlot];
                //Items[newSlot] = itemInHand;
                //WindowManager.GetMouseManager().SetHeldItem(itemToPutInHand);
                InventoryChanged?.Invoke();
                return true;
            }
            else if (itemInOldSlot == null) Debug.LogWarning("Item in old slot was null");

            return false;
        }

        private int AddItemToInventory(ItemContainer item)
        {
            if (item == null) return -1;
            if (item.GetItem() == null) return -1;
            if (item.Quantity == 0) return -1;

            int index = HasItemInInventory(item);
            bool exists = (index != -1);
            if (exists && item.GetItem().Stackable)
            {
                Items[index].Quantity += item.Quantity;
                InventoryChanged?.Invoke();
                return index;
            }

            index = GetNextFreeInventorySpace();
            if (index != -1)
            {
                Items[index] = item.Copy();
                InventoryChanged?.Invoke();
                return index;
            }

            return -1;
        }

        public int AddItem(ItemContainer item)
        {
            if (Items != null)
            {
                return AddItemToInventory(item);
            }
            return -1;
        }

        public bool RemoveCountFromIndex(int index, int quantity)
        {
            if (index >= 0 && index < Items.Length)
            {
                if (Items[index] == null) return false;

                Items[index].Quantity -= quantity;
                InventoryChanged?.Invoke();
                return true;
            }
            return false;
        }

    }
}