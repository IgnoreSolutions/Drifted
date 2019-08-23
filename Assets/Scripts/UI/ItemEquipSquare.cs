using System.Collections;
using System.Collections.Generic;
using Drifted.Inventory;
using Drifted.Items.Next;
using UnityEngine;
using UnityEngine.UI;

public enum ItemEquipType
{
    Tool,
    Helmet,
    Body,
    Pants,
    Shoes
}

public class ItemEquipSquare : MonoBehaviour
{
    [SerializeField]
    ItemContainer itemContainer;

    [SerializeField]
    PlayerInventory InventoryInstance;

    [SerializeField]
    ItemEquipType ItemEquipmentType;

    [SerializeField]
    [EnumFlags]
    ItemFlags Filter;

    [SerializeField]
    Image itemImage;

    private void Awake()
    {

    }
}
