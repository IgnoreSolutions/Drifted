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

[Flags]
public enum FirePitStates
{
    RockPile,

    RockRing1,
    RockRing2,
    RockRing3,

    LogsNotLit,

    LogsBurning,
    LogsHalfBurnt,
    Charcoal,

    Ashe
}

public class FirePitScript : DriftedSceneInteractable
{
    // Obtained at runtime.
    public CameraFollow mainCamera;
    public PlayerMovement player;

    //Main Feild
    public Transform FirePitPrefab; //main prefab
    public Transform FireRotate; //fixes the rotate issue
    public GameObject Empty; //emptys game object
    public GameObject FirePitStage1Prefab;  //Rock Pile
    public GameObject FirePitStage2Prefab;  //First rock ring
    public GameObject FirePitStage3Prefab;  //Second rock ring
    public GameObject FirePitStage4Prefab;  //Third rock ring and final
    public GameObject FirePitStage5Prefab;  //Logs are added
    public GameObject FirePitStage6Prefab;  //Logs are kinda burnt
    public GameObject FirePitStage7Prefab;  //Logs are charcoal
    public GameObject FirePitStage8Prefab;  //Ashe

    public GameObject FirePrefab;  //Fire

    public Light FirePitLight;

    //Clocks
    public float BuildTime = 1.0f;
    public float FireBurnTime = 60.0f;
    public float CleaningTime = 2.0f;
    public float LightFireTime = 2.0f;

    //FireLighting
    public bool ableToBuild = false;
    public bool ableToAddLogs = false;
    public bool ableToFireMake = false;
    public bool ableToCleanAsh = false;

    public FirePitStates CurrentPitState = FirePitStates.RockPile;

    //Sound
    public AudioSource speaker;

    protected override void Awake()
    {
        base.Awake();
        if (Console == null) Console = (ActivityConsoleManager)ScriptableObject.CreateInstance(typeof(ActivityConsoleManager));
        Debug.Log("Awake called");
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
            mainCamera.SetFollowTarget(this.gameObject);

            List<AbstractMenuItem> menuItems = new List<AbstractMenuItem>();

            if (ableToBuild) menuItems.Add(PopUpMenu.MakeMenuItem("Build Fire Pit", BuildFirePit));
            if (ableToAddLogs) menuItems.Add(PopUpMenu.MakeMenuItem("Put Logs in", AddLogs));
            if (ableToFireMake) menuItems.Add(PopUpMenu.MakeMenuItem("Light Fire", LightFire));
            if (ableToCleanAsh) menuItems.Add(PopUpMenu.MakeMenuItem("Clean out Ash", CleanAsh));

            var menu = menuManager.MakePopUpMenu(this.gameObject, menuItems.ToArray());

            //menu.AutoShowPopup(gameObject);
        }

    }

    bool BuildFirePit()
    {
        if (Skills.Firemaking.Level >= 1 && ableToBuild == true)
        {
            if(player == null) Debug.LogWarning("Player is null?");
            player.EnqueueAction(player.MovePlayerTo(this.transform));
            player.EnqueueAction(() => player.PlayBuildingAnimation(true));
            player.EnqueueAction(() => BuildFirePitCycle1());
            player.EnqueueWait(10.0f);
            player.EnqueueAction(() => player.PlayBuildingAnimation(false));
            player.EnqueueAction(() =>
            {
                Skills.Building.GainExp(5);
                Console.AddLine("ur a thicc bitch, marty");
            });
            return true;
        }
        else
        {
            //WindowManager.GetConsole().AddLine("<color=red>It is already Built</color>");
            return false;
        }
    }

    bool AddLogs()
    {
        /*
        var allWoods = playerInventory;
        bool hasLogs = false;
        foreach (var wood in allWoods)
        {

            /*int index = DriftedConstants.Instance.Player().HasItemInInventory(wood);
            if (index >= 0)
            {
                DriftedConstants.Instance.Player().RemoveCountFromIndex(index, 1);
                hasLogs = true;
                break;
            }

        }
        */
        bool hasLogs = true;

        if (hasLogs && Skills.Firemaking.Level >= 1 && ableToAddLogs == true)
        {
            player.EnqueueAction(player.MovePlayerTo(this.transform));
            player.EnqueueAction(() => player.PlayBuildingAnimation(true));
            player.EnqueueWait(3.0f);
            player.EnqueueAction(() =>
            {
                LogsInFirePitCycle();
                player.PlayBuildingAnimation(false);
            });
            player.EnqueueAction(() =>
            {
                if (FirePitStage5Prefab.activeSelf)
                {
                    Skills.Firemaking.GainExp(1);
                    //WindowManager.GetConsole().AddLine("You gained <color=cycan>1 Exp</color> in <b>Fire Making</b>");

                    //Invoke("LogsInFirePitCycle", BuildTime);
                }
                if (!FirePitStage5Prefab.activeSelf)
                {
                    FirePitStage5Prefab.SetActive(true);
                    FirePitStage4Prefab.SetActive(false);

                    ableToAddLogs = false;
                    ableToFireMake = true;
                }

            });
            return true;

        }
        else
        {
            //WindowManager.GetConsole().AddLine("<color=red>You cannot add logs at this time</color>");
        }


        return false;
    }

    private void OnDrawGizmos()
    {
        /*
        if (Application.isEditor && FirePitStage5Prefab != null)
        {
            MeshFilter mesh = FirePitStage5Prefab.GetComponent<MeshFilter>();
            Gizmos.DrawMesh(mesh.sharedMesh, transform.position, Quaternion.Euler(90, 0, 0));
        }
        */
    }

    bool LightFire()
    {
        float fireChance = Mathf.Clamp(Skills.Firemaking.Level * 2, 15, 100);
        float chance = UnityEngine.Random.Range(1, 15);

        if (ableToFireMake == true)
        {
            player.EnqueueAction(() =>
            {
                player.MovePlayerTo(this.transform);
            });

            player.EnqueueAction(()=> { player.PlayFireAnimation(true); });
            player.EnqueueWait(6.0f);
            player.EnqueueAction(() =>
            {
                if (chance <= fireChance)
                {
                    Skills.Firemaking.GainExp(5);
                    //WindowManager.GetConsole().AddLine("You gained <color=cyan>5 Exp</color> in <b>Fire Making</b>");
                    Debug.Log("Fire Chance:" + chance);
                    Invoke("LogsBurningCycle1", LightFireTime);
                }
                else
                {
                    //WindowManager.GetConsole().AddLine("<color=red>You Failed to start a Fire</color>");
                    Debug.Log("Fire Chance" + chance);
                }

                if (!FirePrefab.activeSelf)
                {
                    if (chance <= fireChance)
                    {
                        Skills.Firemaking.GainExp(5);

                        //WindowManager.GetConsole().AddLine("You gained <color=cyan>5 Exp</color> in <b>Fire Making</b>");

                        Debug.Log("Fire Chance:" + chance);

                        Invoke("LogsBurnAgain1", LightFireTime);
                    }
                }
            });
            player.EnqueueAction(() => { player.PlayFireAnimation(false); });
            return true;
        }
        else
        {
            //WindowManager.GetConsole().AddLine("<color=red>You can not start a fire yet</color>");
        }
        return false;
    }

    bool CleanAsh()
    {
        if (ableToCleanAsh == true)
        {
            player.EnqueueAction(() => { player.MovePlayerTo(this.transform); });
            player.EnqueueAction(() => { player.PlayBuildingAnimation(true); });
            player.EnqueueWait(6.0f);
            player.EnqueueAction(() =>
            {
                Skills.Firemaking.GainExp(1);

                //WindowManager.GetConsole().AddLine("You gained <color=cycan>1 Exp</color> in <b>Fire Making</b>");

                FirePitStage8Prefab.SetActive(false);
                FirePitStage4Prefab.SetActive(true);

                ableToAddLogs = true;
                ableToCleanAsh = false;
            });
            player.EnqueueAction(() => { player.PlayBuildingAnimation(false); });
            return true;
        }
        else
        {
            //WindowManager.GetConsole().AddLine("<color=red>There is no ashe to remove</color>");
            return false;
        }
    }

    void OnEnable()
    {
        var marty = GameObject.FindWithTag("Player");
        if(marty != null) player = marty.GetComponent<PlayerMovement>();
        mainCamera = Camera.main.GetComponent<CameraFollow>();
    }

    void Start()  //Pile of rocks
    {

        FirePitStage1Prefab = Instantiate(FirePitStage1Prefab, Empty.transform.position, FirePitStage1Prefab.transform.rotation) as GameObject;
        FirePitStage1Prefab.tag = FirePitPrefab.gameObject.tag;
        FirePitStage1Prefab.layer = FirePitPrefab.gameObject.layer;
        FirePitStage1Prefab.transform.SetParent(Empty.transform);
        //FirePitStage1Prefab.transform.rotation = Quaternion.Euler(0, 0, 0);

        ableToBuild = true;
        ableToFireMake = false;
        ableToAddLogs = false;
        ableToCleanAsh = false;

        if(FirePitLight != null) FirePitLight.enabled = false;

        SetFriendlyName("Rock Pile");
    }

    void BuildFirePitCycle1()  //First rock ring
    {
        FirePitStage2Prefab = Instantiate(FirePitStage2Prefab, Empty.transform.position, FirePitStage2Prefab.transform.rotation) as GameObject;
        FirePitStage2Prefab.tag = FirePitPrefab.gameObject.tag;
        FirePitStage2Prefab.layer = FirePitPrefab.gameObject.layer;
        FirePitStage2Prefab.transform.SetParent(Empty.transform);

        ableToBuild = false;
        ableToFireMake = false;
        ableToAddLogs = false;
        ableToCleanAsh = false;

        FirePitStage1Prefab.SetActive(false);
        CurrentPitState = FirePitStates.RockRing1;


        Invoke("BuildFirePitCycle2", BuildTime);
    }

    void BuildFirePitCycle2()  //Second rock ring
    {
        FirePitStage3Prefab = Instantiate(FirePitStage3Prefab, Empty.transform.position, FirePitStage3Prefab.transform.rotation) as GameObject;
        FirePitStage3Prefab.tag = FirePitPrefab.gameObject.tag;
        FirePitStage3Prefab.layer = FirePitPrefab.gameObject.layer;
        FirePitStage3Prefab.transform.SetParent(Empty.transform);

        ableToBuild = false;
        ableToFireMake = false;
        ableToAddLogs = false;
        ableToCleanAsh = false;

        FirePitStage2Prefab.SetActive(false);

        Invoke("BuildFirePitCycle3", BuildTime);
    }

    void BuildFirePitCycle3()  //Third and Final ring
    {
        FirePitStage4Prefab = Instantiate(FirePitStage4Prefab, Empty.transform.position, FirePitStage4Prefab.transform.rotation) as GameObject;
        FirePitStage4Prefab.tag = FirePitPrefab.gameObject.tag;
        FirePitStage4Prefab.layer = FirePitPrefab.gameObject.layer;
        FirePitStage4Prefab.transform.SetParent(Empty.transform);

        ableToBuild = false;
        ableToFireMake = false;
        ableToAddLogs = true;
        ableToCleanAsh = false;

        SetFriendlyName("Fire Pit");

        FirePitStage3Prefab.SetActive(false);
    }

    /// <summary>
    /// Logs added to the fire Pit but no fire
    /// </summary>
    void LogsInFirePitCycle()  //
    {
        FirePitStage5Prefab = Instantiate(FirePitStage5Prefab, Empty.transform.position, FirePitStage5Prefab.transform.rotation) as GameObject;
        FirePitStage5Prefab.tag = FirePitPrefab.gameObject.tag;
        FirePitStage5Prefab.layer = FirePitPrefab.gameObject.layer;
        FirePitStage5Prefab.transform.SetParent(Empty.transform);

        ableToBuild = false;
        ableToFireMake = true;
        ableToAddLogs = false;
        ableToCleanAsh = false;

        FirePitStage4Prefab.SetActive(false);
    }

    void LogsBurningCycle1()  //Fire is set to the logs
    {
        FirePrefab = Instantiate(FirePrefab, Empty.transform.position, FirePrefab.transform.rotation) as GameObject;
        FirePrefab.tag = FirePitPrefab.gameObject.tag;
        FirePrefab.layer = FirePitPrefab.gameObject.layer;
        FirePrefab.transform.SetParent(Empty.transform);

        ableToBuild = false;
        ableToFireMake = false;
        ableToAddLogs = false;
        ableToCleanAsh = false;
        if(FirePitLight != null)
        {
             FirePitLight.enabled = true;
             FirePitLight.intensity = .5f;
        }
        Invoke("LogsBurningCycle2", FireBurnTime);
    }

    void LogsBurningCycle2()  //Logs have some charcoal
    {
        FirePitStage6Prefab = Instantiate(FirePitStage6Prefab, Empty.transform.position, FirePitStage6Prefab.transform.rotation) as GameObject;
        FirePitStage6Prefab.tag = FirePitPrefab.gameObject.tag;
        FirePitStage6Prefab.layer = FirePitPrefab.gameObject.layer;
        FirePitStage6Prefab.transform.SetParent(Empty.transform);

        FirePitStage5Prefab.SetActive(false);

        ableToBuild = false;
        ableToAddLogs = false;
        ableToFireMake = false;
        ableToCleanAsh = false;

        Invoke("LogsBurningCycle3", FireBurnTime);
    }

    void LogsBurningCycle3()  //Logs are charcoal
    {
        FirePitStage7Prefab = Instantiate(FirePitStage7Prefab, Empty.transform.position, FirePitStage7Prefab.transform.rotation) as GameObject;
        FirePitStage7Prefab.tag = FirePitPrefab.gameObject.tag;
        FirePitStage7Prefab.layer = FirePitPrefab.gameObject.layer;
        FirePitStage7Prefab.transform.SetParent(Empty.transform);

        FirePitStage6Prefab.SetActive(false);

        ableToFireMake = false;
        ableToAddLogs = false;
        ableToBuild = false;
        ableToCleanAsh = false;
        if(FirePitLight != null) FirePitLight.intensity = .3f;
        Invoke("AsheFirePit", FireBurnTime);
    }

    void AsheFirePit()  //Ashe Pile
    {
        FirePitStage8Prefab = Instantiate(FirePitStage8Prefab, Empty.transform.position, FirePitStage8Prefab.transform.rotation) as GameObject;
        FirePitStage8Prefab.tag = FirePitPrefab.gameObject.tag;
        FirePitStage8Prefab.layer = FirePitPrefab.gameObject.layer;
        FirePitStage8Prefab.transform.SetParent(Empty.transform);

        FirePitStage7Prefab.SetActive(false);
        FirePrefab.SetActive(false);

        ableToAddLogs = false;
        ableToBuild = false;
        ableToFireMake = false;
        ableToCleanAsh = true;
        if(FirePitLight != null) FirePitLight.enabled = true;
        SetFriendlyName("Dirty Fire Pit");
    }

    //Allows the Logs to Burn again, and again (Repeat Feature)

    void LogsBurnAgain1() //Fire is set to logs
    {
        FirePrefab.SetActive(true);

        ableToFireMake = false;

        Invoke("LogsBurnAgain2", FireBurnTime);
    }

    void LogsBurnAgain2() //Logs are kinda Charded
    {
        FirePitStage6Prefab.SetActive(true);
        FirePitStage5Prefab.SetActive(false);

        Invoke("LogsBurnAgain3", FireBurnTime);
    }

    void LogsBurnAgain3() //Logs are CharCoal
    {
        FirePitStage7Prefab.SetActive(true);
        FirePitStage6Prefab.SetActive(false);

        Invoke("AsheAgain", FireBurnTime);
    }

    void AsheAgain() //Turns logs back to Ashe
    {
        FirePitStage8Prefab.SetActive(true);
        FirePrefab.SetActive(false);

        ableToCleanAsh = true;
    }
}
