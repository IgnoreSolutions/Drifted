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
using Drifted.NextGen.Inventory;

public class GrassSeedling : DriftedSceneInteractable
{
    private PlayerMovement player;

    //Growing Values
    public GameObject Empty;
    public GameObject GrassStage1;
    public GameObject GrassStage2;
    public GameObject GrassStage3;
    public float GrowthTime = 60.0f;
    public bool HarvestReady;

    [SerializeField]
    ItemContainer itemDrop;

    protected override void Awake()
    {
        base.Awake();
        if(Skills == null) Skills = ScriptableObject.CreateInstance<SkillController>();
        if(Skills == null) playerInventory = ScriptableObject.CreateInstance<Inventory>();


        player = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
    }

    public override void Interact(MonoBehaviour sender)
    {
        if (HarvestReady == true)
        {
            BuildMenu();
        }
    }

    void BuildMenu()
    {
        if(menuManager == null) return;

        AbstractMenuItem[] items = new AbstractMenuItem[1];
        items[0] = PopUpMenu.MakeMenuItem("Harvest Grass", Harvest);

        menuManager.MakePopUpMenu(this.gameObject, items);
    }

    private void OnDrawGizmos()
    {
        if (Application.isEditor && GrassStage3 != null)
        {
            MeshFilter mesh = GrassStage3.GetComponent<MeshFilter>();
            Gizmos.DrawMesh(mesh.sharedMesh, transform.position, Quaternion.Euler(90, 0, 0), new Vector3(.1f,.1f,.1f));
        }
    }

    bool Harvest() 
    {
        if (Skills.Harvesting.Level >= 2 && HarvestReady == true)
        {
            playerInventory.AddItem(itemDrop);
            Skills.Harvesting.GainExp(7);
            
            Console.AddLine("You gained 5 Exp in Harvest");
            return true;
        }
        else if (HarvestReady == false)
        {
            Console.AddLine("This Grass isn't ready to Harvest!");
        }
        else if (Skills.Harvesting.Level < 2)
        {
            Console.AddLine("You must be level 2 Harvesting");
        }
        return false;
    }

    void Start()
    {
        GrassStage1 = Instantiate(GrassStage1, Empty.transform.position, GrassStage1.transform.rotation) as GameObject;
        GrassStage1.transform.SetParent(Empty.transform);

        HarvestReady = false;

        //Skills.Farming.GainExp(5);

        Invoke("FirstCycle", GrowthTime);
    }

    void FirstCycle()
    {
        GrassStage2 = Instantiate(GrassStage2, Empty.transform.position, GrassStage2.transform.rotation) as GameObject;
        GrassStage2.transform.SetParent(Empty.transform);

        Destroy(GrassStage1);

        HarvestReady = false;

        Invoke("SecondCycle", GrowthTime);
    }

    void SecondCycle()
    {
        GrassStage3 = Instantiate(GrassStage3, Empty.transform.position, GrassStage3.transform.rotation) as GameObject;
        GrassStage3.transform.SetParent(Empty.transform);

        Destroy(GrassStage2);

        HarvestReady = true;
    }

    
}