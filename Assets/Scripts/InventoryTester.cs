using Drifted.Inventory;
using UnityEngine;
using static Drifted.Inventory.MikeInventory;
using Drifted.Items.ItemDefinitions;
using System.Collections.Generic;
using Drifted.Items.Next;


/// <summary>
/// Attaches to an instance of a MikeInventory
/// </summary>
public class InventoryTester : MonoBehaviour
{
    public List<ItemContainer> ItemsToAdd;
    public void Awake()
    {
        GameObject Player = GameObject.FindWithTag("Player");
        if(Player != null)
        {
            var playerInv = Player.GetComponent<PlayerInventory>();
            if (playerInv != null)
            {
                foreach(var item in ItemsToAdd)
                {
                    //playerInv.Inventory.AddItem(item);
                }
            }
            /*
            Player.GetComponent<PlayerInventory>().Inventory.AddItems(
                new ItemCoconut() { Quantity = 2 }, 
                new ItemAvocado(), 
                new ItemPalmLeaf() { Quantity = 6 }
            );
            */
        }
    }
}
