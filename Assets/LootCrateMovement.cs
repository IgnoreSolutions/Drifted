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
using Drifted.Items.Next;

public class LootCrateMovement : DriftedSceneInteractable
{
    [SerializeField] GameObject lootCrateDestination;
    Vector3 distanceToLand;    // this is the magnitude from the barrel's spawn point to it's destination

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);

        distanceToLand = lootCrateDestination.transform.position - gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
