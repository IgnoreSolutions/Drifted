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


public class FishSchool : DriftedSceneInteractable
{
    // Some bullshit
    public Transform empty;
    public GameObject fish;
    public float chance = 5.0f;
    public bool ableToFish = false;

    protected override void Awake()
    {
        base.Awake();
        GameObject playerController = GameObject.FindWithTag("Player");
    }

    public override void Interact(MonoBehaviour sender)
    {
        BuildMenu();
    }

    int menuID = -1; //Something the menu needs to work

    void BuildMenu()
    {
        AbstractMenuItem[] items = new AbstractMenuItem[2];

        items[0] = PopUpMenu.MakeMenuItem("Spear Fish", SpearFish);
        items[1] = PopUpMenu.MakeMenuItem("Do Nothing", Test);


        var menu = menuManager.MakePopUpMenu(this.gameObject, items);
    }

    bool SpearFish()
    {
        if (Skills.Fishing.Level >= 1 && ableToFish == true)
        {
            Skills.Fishing.GainExp(5);
            return true;
        }

        return false;
    }

    bool Test()
    {
        Debug.Log("This is a Test");
        return true;
    }

    void Start()
    {
        fish = Instantiate(fish, empty.position, fish.transform.rotation) as GameObject;
    }

    void HasFish()
    {
        ableToFish = true;

        fish = Instantiate(fish, empty.position, fish.transform.rotation) as GameObject;



    }
}
