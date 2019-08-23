using Drifted;
using Drifted.Interactivity;
using Drifted.Inventory;
using Drifted.Items.ItemDefinitions;
using Drifted.Skills;
using Drifted.UI;
using Drifted.UI.WindowManager;
using MikeSantiago.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Drifted.Inventory.MikeInventory;

public class BarrelInteraction : DriftedSceneInteractable
{
    bool isBarrelInteractable = false; // is false while barrel is floating towards it's destination

    //Sound
    public AudioSource speaker;

    //menu
    [ReadOnly]
    public MikeWindowManager WindowManager;
    [ReadOnly]
    public PlayerInventory PlayerInventoryController;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Interact(MonoBehaviour sender)
    {
        BuildMenu();
    }

    int menuID = -1;

    void BuildMenu()
    {
        if (menuID == -1)
        {
            //AbstractMenuItem[] menuItems = new AbstractMenuItem[4];
            List<AbstractMenuItem> menuItems = new List<AbstractMenuItem>();

            //if (ableToBuild) menuItems.Add(PopUpMenu.MakeMenuItem("Build Fire Pit", BuildFirePit));

            var menu = DriftedConstants.Instance.UI().MenuController.MakePopUpMenu(Input.mousePosition, this, menuItems.ToArray());
            menuID = menu.ID;
            menu.MenuClosed += () => menuID = -1;

            //menu.AutoShowPopup(gameObject);
        }
    }
}
