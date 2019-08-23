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

// TODO: Still more updates

public class ClayMound : DriftedSceneInteractable
{
    //Main Feild
    public Transform clayMound;
    public Transform player;

    //Distance Calculator
    public float dist;

    //Menu Feild
    [ReadOnly]
    public MikeWindowManager WindowManager;
    [ReadOnly]
    public PlayerInventory PlayerInventoryController;


    //Time Feild
    public float harvestTime = 5.0f;

    void Update()
    {
        dist = Vector3.Distance(clayMound.position, player.position);
    }

    protected override void Awake()
    {
        base.Awake();
        GameObject masterCanvas = GameObject.FindWithTag("UIMaster");
        GameObject playerController = GameObject.FindWithTag("Player");
        if (masterCanvas != null)
        {
            WindowManager = masterCanvas.GetComponent<MikeWindowManager>();
        }

        if (playerController != null)
        {
            PlayerInventoryController = playerController.GetComponent<PlayerInventory>();
            if (PlayerInventoryController == null) Debug.LogWarning("Inventory controller null.");
        }
    }

    public override void Interact(MonoBehaviour sender)
    {
        BuildMenu();
    }

    int menuID = -1;

    void BuildMenu()
    {
        if (menuID == -1 && dist <= 6)
        {
            AbstractMenuItem[] menuItems = new AbstractMenuItem[1];

            menuItems[0] = PopUpMenu.MakeMenuItem("Harvest Clay", HarvestClay);

            var menu = menuManager.MakePopUpMenu(this.gameObject, menuItems);
            //menu.AutoShowPopup(gameObject);
        }
        
    }

    bool HarvestClay()
    {
        // ...?
        if (dist <= 10)
        {
            Debug.Log("Youre Good");
            return true;
        }
        else
        {
            Debug.Log("Move Closer");
            return false;
        }
    }
}
