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

public class BananaTreeSeedling : DriftedSceneInteractable
{
    //Growing Values
    public GameObject emptyG;
    public GameObject banana1;
    public GameObject banana2;
    public GameObject banana3;
    public GameObject bananaStartFruit;
    public GameObject bananaFruit;
    public GameObject bananaNoFruit;
    public GameObject bananaCut;
    public GameObject bananaLeaves;

    //Harvest 
    public bool ableToHarvestLeaves = false;
    public bool ableToHarvestFruit = false;
    public bool ableToCut = false;

}
