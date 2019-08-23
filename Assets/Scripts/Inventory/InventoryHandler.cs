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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Drifted.Inventory
{
    [Serializable]
    public class SerializableItemContainer
    {
        public int Quantity;
        public string ItemName;

        public SerializableItemContainer()
        {
            ItemName = "Unknown";
            Quantity = 1;
        }

        public SerializableItemContainer(string itemName, int quantity)
        {
            ItemName = itemName;
            Quantity = quantity;
        }
    }

    [Serializable]
    public class InventoryHandler
    {
        [NonSerialized]
        public MikeWindowManager WindowManager;

        public ItemContainer[] Inventory;

        public event InventoryUpdated InventoryChanged;
        public delegate void InventoryUpdated();

        public InventoryHandler(int inventorySlots = 15)
        {
            Inventory = new ItemContainer[inventorySlots];
            for (int i = 0; i < inventorySlots; i++) Inventory[i] = new ItemContainer();
        }

        public ItemContainer DeserializeItemContainer(SerializableItemContainer serializedContainer)
        {
            if (serializedContainer == null) return new ItemContainer();

            //return new ItemContainer(DriftedConstants.Instance.ItemDB.GetItemByName(serializedContainer.ItemName), serializedContainer.Quantity);
            return null; // TODO:
        }

        public SerializableItemContainer[] SerializeInventory()
        {
            SerializableItemContainer[] inventory = new SerializableItemContainer[Inventory.Count()];

            for(int i = 0; i < Inventory.Count(); i++)
            {
                if(Inventory[i] == null || Inventory[i].GetItem() == null)
                {
                    continue;
                }
                string itemName = Inventory[i].GetItem().ItemName;
                int quantity = Inventory[i].Quantity;

                inventory[i] = new SerializableItemContainer(itemName, quantity);
            }

            return inventory;
        }

        public void DeserializeInventory(SerializableItemContainer[] newInv)
        {
            Inventory = new ItemContainer[newInv.Count()];

            for(int i = 0; i < newInv.Count(); i++)
            {
                Inventory[i] = DeserializeItemContainer(newInv[i]);
            }

            ForceRefresh();
            Debug.Log("Inventory deserialized");
        }

        public void ForceRefresh()
        {
            if(InventoryChanged != null) InventoryChanged();
        }

        public void ResizeInventory(int newSize)
        {
            ItemContainer[] _emptyNewSize = new ItemContainer[newSize];
            if (newSize < Inventory.Length)
            {
                for (int i = 0; i < newSize; i++) _emptyNewSize[i] = Inventory[i];
            }
            else
            {
                for (int i = 0; i < Inventory.Length; i++) _emptyNewSize[i] = Inventory[i];
            }

            Inventory = _emptyNewSize;
        }

        public int HasItemInInventory(Item item)
        {
            for (int i = 0; i < Inventory.Length; i++)
            {
                var invItem = Inventory[i];
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
            for (int i = 0; i < Inventory.Length; i++)
            {
                int index = HasItemInInventory(item);

                if (index != -1)
                {
                    ItemContainer fromInventory = Inventory[index];
                    if (fromInventory.Quantity >= item.Quantity) return index;
                }
            }
            return -1;
        }

        public int HasItemInInventory(int id)
        {
            if (Inventory == null) throw new Exception("Inventory was null");

            for (int i = 0; i < Inventory.Length; i++)
            {
                var item = Inventory[i];
                // TODO:
                //if (item != null && item.name == id) return i;
                if (item != null && item.GetItem().ItemName == Inventory[i].GetItem().ItemName) return i;
            }

            return -1;
        }

        public int HasItemInInventory(int id, int quantity)
        {
            int item = HasItemInInventory(id);
            if (item > -1)
            {
                if (Inventory[item].Quantity >= quantity) return item;
            }

            return -1;
        }

        public int HasItemInInventory(ItemContainer item, bool matchQuantity)
        {
            int index = HasItemInInventory(item);
            if (index == -1) return -1;
            if (matchQuantity)
            {
                if (Inventory[index].Quantity >= item.Quantity) return index;
            }
            else return index;

            return -1;
        }

        public int GetNextFreeInventorySpace()
        {
            for (int i = 0; i < Inventory.Length; i++) if (Inventory[i].GetItem() == null) return i;
            return -1;
        }

        public bool MoveItemInInventory(int oldSlot, int newSlot)
        {
            if (newSlot > Inventory.Length || oldSlot > Inventory.Length || newSlot < 0 || oldSlot < 0) return false;

            ItemContainer itemInOldSlot = Inventory[oldSlot];
            ItemContainer itemInNewSlot = Inventory[newSlot];

            if (itemInOldSlot != null && itemInNewSlot == null) // Direct move
            {
                Inventory[newSlot] = Inventory[oldSlot].Copy(); // TODO: necessary?
                Inventory[oldSlot] = null;
                InventoryChanged?.Invoke(); ;
                return true;
            }
            else if (itemInOldSlot != null && itemInNewSlot != null) // swap item in mouse for item in inventory
            {
                var itemInHand = WindowManager.GetMouseManager().GetHeldItem();

                Inventory[newSlot] = Inventory[oldSlot];
                var itemToPutInHand = Inventory[newSlot];
                Inventory[newSlot] = itemInHand;
                WindowManager.GetMouseManager().SetHeldItem(itemToPutInHand);
                InventoryChanged?.Invoke(); ;
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
                Inventory[index].Quantity += item.Quantity;
                InventoryChanged?.Invoke();
                return index;
            }

            index = GetNextFreeInventorySpace();
            if (index != -1)
            {
                Inventory[index] = item.Copy();
                InventoryChanged?.Invoke();
                return index;
            }

            DriftedConstants.Instance.UI().Console.AddLine("Your inventory is full!");
            return -1;
        }

        /*
        private int AddItemToInventory(Item item)
        {
            if (item == null) return -1;
            if ( == 0) { Debug.LogWarning("Quantity of 0"); return -1; }
            int index = HasItemInInventory(item);
            bool exists = (index != -1);
            if(exists && item.Stackable)
            {
                Inventory[index].Quantity += item.GetQuantity();
                InventoryChanged?.Invoke();
                return index;
            }

            // Put in next free space
            index = GetNextFreeInventorySpace();
            if(index != -1)
            {
                Inventory[index].SetItem(item);
                InventoryChanged?.Invoke();
                return index;
            }

            DriftedConstants.Instance.Console.AddLine("Your inventory is full!");
            return -1;
        }
    */

        [Obsolete]
        public Sprite GetItemSprite(string itemSpriteName)
        {
            return null;
            //if (itemSpriteName.StartsWith("mc", StringComparison.Ordinal)) return PlaceholderItemsAtlas.FirstOrDefault(x => x.name == itemSpriteName);
            //return DriftedItemsAtlas.FirstOrDefault(x => x.name == itemSpriteName);
        }

        /*
        public int AddItem(Item item)
        {
            if (Inventory != null)
            {
                return AddItemToInventory(item);
            }
            return -1;
        }
        */

        public int AddItem(ItemContainer item)
        {
            if (Inventory != null)
            {
                return AddItemToInventory(item);
            }
            return -1;
        }

        /*
        public void AddItems(params Item[] items)
        {
            foreach (var item in items) if (AddItemToInventory(item) == -1) return; // Something went wrong.
        }
        */

        /// <summary>
        /// Remove a specific item in the inventory at a specific slot.
        /// If index is -1, the item will be located and removed from the 
        /// first slot it finds it in.
        /// </summary>
        /// <returns><c>true</c>, if item from inventory was removed, <c>false</c> otherwise.</returns>
        /// <param name="item">Item.</param>
        /// <param name="index">Index.</param>
        public bool RemoveItemFromInventory(Item item, int index = -1)
        {
            if (index == -1) index = HasItemInInventory(item);

            if (index >= 0 && index < Inventory.Length)
            {
                Inventory[index] = null;
                InventoryChanged?.Invoke();
                return true;
            }

            return false;
        }

        public bool RemoveCountFromIndex(int index, int quantity)
        {
            //Debug.Log($"Requested to remove {quantity}x from index {index}. Contains {Inventory[index].GetItem()}");
            if(index >= 0 && index < Inventory.Length)
            {
                if (Inventory[index] == null) return false;

                Inventory[index].Quantity -= quantity;
                InventoryChanged?.Invoke();
                return true;
            }
            return false;
        }

        public void NullItemAt(int index)
        {
            Inventory[index].SetItem(null);
            Inventory[index].Quantity = 0;
        }

        public void SetItemAt(ItemContainer container, int index)
        {
            Inventory[index] = container;
            InventoryChanged?.Invoke();
        }

        public void SetItemAt(Item item, int index)
        {
            Inventory[index].SetItem(item);
            InventoryChanged?.Invoke();
        }
    }
}
