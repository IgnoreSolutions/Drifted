using Drifted;
using Drifted.Interactivity;
using Drifted.UI;
using UnityEngine;

public class CoconutSeedling : DriftedSceneInteractable
{
    [Header("New Stuff")]
    public GameEvent newCameraTargetEvent;
    public PlayerMovement playerMovement;

    // Growing Values
    [Header("Growing Values")]
    public Transform coconutPrefab;
    public GameObject empty;
    public GameObject palmTreeStage1Prefab;
    public GameObject palmTreeStage2Prefab;
    public GameObject palmTreeStage3Prefab;
    public GameObject palmTreeStage4Prefab;
    public GameObject palmTreeStage5Prefab;
    public GameObject palmTreeStage6Prefab;
    public float growthTime = 60.0f;
    public float harvestTime = 0.0f;
    public float decayTime = 120.0f;
    public float fruitTime = 120.0f;
    public bool ableToHarvest = false;
    public bool ableToCut = false;
    public float harvestPalm = 60.0f;
    public bool ableToHarvestPalm = false;

    protected override void Awake()
    {
        base.Awake();

        GameObject playerController = GameObject.FindWithTag("Player");
        playerMovement = playerController.GetComponent<PlayerMovement>();
    }

    private void OnDrawGizmos()
    {
        if(Application.isEditor && palmTreeStage5Prefab != null)
        {
            MeshFilter mesh = palmTreeStage5Prefab.GetComponent<MeshFilter>();
            Gizmos.DrawMesh(mesh.sharedMesh, transform.position, Quaternion.Euler(90, 0, 0));
        }

    }

    public override void Interact(MonoBehaviour sender)
    {
        if (palmTreeStage3Prefab == null || palmTreeStage4Prefab == null)
        {
            if (newCameraTargetEvent != null) newCameraTargetEvent.Raise(gameObject);
            BuildMenu();
        }
    }

    int menuID = -1;

    void BuildMenu()
    {
        if (menuManager == null) return;

        AbstractMenuItem[] items = new AbstractMenuItem[3];

        items[0] = PopUpMenu.MakeMenuItem("Harvest Coconuts", Harvest, false);
        items[1] = PopUpMenu.MakeMenuItem("Harvest Palm Leaves", HarvestPalm, false);
        items[2] = PopUpMenu.MakeMenuItem("Cut Coconut Tree", CutTree, false);

        menuManager.MakePopUpMenu(this.gameObject, items);
        //var menu = DriftedConstants.Instance.UI().MenuController.MakePopUpMenu(Input.mousePosition, this, items);
        //menuID = menu.ID;
        //menu.MenuClosed += () => menuID = -1;
        //menu.AutoShowPopup(gameObject);
    }

    [SerializeField]
    ItemContainer coconut;

    [SerializeField]
    ItemContainer palmDrop;

    [SerializeField]
    ItemContainer woodDrop;

    /*
    bool PlayAnimation()
    {
        playerMovement.anim.Play("Woodcutting");
        return true;
    }

    bool StopAnimation()
    {
        playerMovement.anim.StopPlayback();
        return true;
    }
    */

    bool Harvest()  // Does not currently work, i need to save the Stage 4 and Stage 5 so i can call on them later
    {
        if (playerMovement == null) return false;

        if (Skills == null) return false;

        if (Skills.Harvesting.Level >= 1 && ableToHarvest == true)
        {
            playerMovement.EnqueueAction(playerMovement.MovePlayerTo(transform));
            playerMovement.EnqueueWait(3.0f);
            playerMovement.EnqueueAction(() =>
            {
                Skills.Harvesting.GainExp(5);
                notificationManager.PushNotification(this.gameObject, new DriftedNotification($"You put {coconut.Quantity} {coconut.GetItem().ItemName} in your inventory.", coconut.GetItem().Icon));
                notificationManager.PushNotification(this.gameObject, new DriftedNotification($"You gained 5 experience in <b>Harvesting</b>"));
                //if(Console != null) Console.AddLine("You gained <color=cyan>5 Exp</color> in <b>Harvest</b>");
                //Console.AddLine();
                playerInventory.AddItem(coconut);
                Invoke("HarvestCoconut", harvestTime);
            });
            return true;
        }
        else
        {
            Console.AddLine("This tree isn't ready to harvest!");
            return false;
        }
    }

    bool HarvestPalm()
    {
        if (Skills.Harvesting.Level >= 2 && ableToHarvestPalm == true)
        {
            playerMovement.EnqueueAction(playerMovement.MovePlayerTo(transform));
            // Enqueue an animation wait in between these, when we get an animation.
            playerMovement.EnqueueWait(1.0f);
            playerMovement.EnqueueAction(() =>
            {
                Skills.Harvesting.GainExp(5);
                Console.AddLine("You gained <color=cyan>5 Exp</color> in <b>Harvesting</b>");

                Invoke("PalmLeavesCycle", harvestPalm);
            });
            return true;
        }
        else if (Skills.Harvesting.Level < 2)
        {
            Console.AddLine("You need to be <color=cyan>Level 2</color> <b>Harvesting</b>");
        }
        else if (ableToHarvestPalm == false)
        {
            Console.AddLine("You already harvested some Palm Leaves give it time to regrow, so bug off");
        }
        return false;
    }

    bool CutTree()
    {
        if (Skills.WoodCutting.Level >= 1 && ableToCut == true) // Checks if Player is lvl 1 Wood Cutting
        {
            playerMovement.EnqueueAction(playerMovement.MovePlayerTo(transform));

            // Start the animation
            playerMovement.EnqueueAction(() => playerMovement.PlayChoppingAnimation(true));
            playerMovement.EnqueueWait(6.0f);
            //End the animation
            playerMovement.EnqueueAction(() => playerMovement.PlayChoppingAnimation(false));
            //Use this wait to let the animation finish before giving execution back
            playerMovement.EnqueueWait(2.5f);
            playerMovement.EnqueueAction(() =>
            {
                playerInventory.AddItem(woodDrop);

                Invoke("DecayCycle", harvestTime);
                Destroy(palmTreeStage4Prefab);
                Destroy(palmTreeStage5Prefab);
                palmTreeStage4Prefab.SetActive(false);
                palmTreeStage5Prefab.SetActive(false);

                Skills.WoodCutting.GainExp(5);

                Console.AddLine("You gained <color=cyan>10 Exp</color> in <b>Woodcutting</b>");
            });
            return true;
        }
        else
        {
            Console.AddLine("You already Cut this Tree Down!");
            return false;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        palmTreeStage1Prefab = Instantiate(palmTreeStage1Prefab, coconutPrefab.position, coconutPrefab.rotation) as GameObject;
        palmTreeStage1Prefab.tag = coconutPrefab.gameObject.tag;
        palmTreeStage1Prefab.layer = coconutPrefab.gameObject.layer;
        palmTreeStage1Prefab.transform.SetParent(coconutPrefab.transform);

        //Skills.Farming.GainExp(5);

        Invoke("FirstCycle", growthTime);
    }

    void FirstCycle()
    {
        palmTreeStage2Prefab = Instantiate(palmTreeStage2Prefab, coconutPrefab.position, coconutPrefab.rotation) as GameObject;
        palmTreeStage2Prefab.tag = coconutPrefab.gameObject.tag;
        palmTreeStage2Prefab.layer = coconutPrefab.gameObject.layer;
        palmTreeStage2Prefab.transform.SetParent(coconutPrefab);

        Destroy(palmTreeStage1Prefab);

        Invoke("SecondCycle", growthTime);
    }

    void SecondCycle()
    {
        palmTreeStage3Prefab = Instantiate(palmTreeStage3Prefab, coconutPrefab.position, coconutPrefab.rotation) as GameObject;
        palmTreeStage3Prefab.tag = coconutPrefab.gameObject.tag;
        palmTreeStage3Prefab.layer = coconutPrefab.gameObject.layer;
        palmTreeStage3Prefab.transform.SetParent(coconutPrefab);
        palmTreeStage3Prefab.transform.SetParent(coconutPrefab);

        Destroy(palmTreeStage2Prefab);

        Invoke("ThirdCycle", growthTime);
    }

    void ThirdCycle()
    {
        palmTreeStage4Prefab = Instantiate(palmTreeStage4Prefab, coconutPrefab.position, coconutPrefab.rotation) as GameObject;
        palmTreeStage4Prefab.tag = coconutPrefab.gameObject.tag;
        palmTreeStage4Prefab.layer = coconutPrefab.gameObject.layer;
        palmTreeStage4Prefab.transform.SetParent(coconutPrefab);

        Destroy(palmTreeStage3Prefab);

        Invoke("FourthCycle", growthTime);
    }

    void FourthCycle()
    {
        palmTreeStage5Prefab = Instantiate(palmTreeStage5Prefab, coconutPrefab.position, coconutPrefab.rotation) as GameObject;
        palmTreeStage5Prefab.tag = coconutPrefab.gameObject.tag;
        palmTreeStage5Prefab.layer = coconutPrefab.gameObject.layer;
        palmTreeStage5Prefab.transform.SetParent(coconutPrefab);

        ableToCut = true;
        ableToHarvest = true;

        palmTreeStage4Prefab.SetActive(false);
    }

    void HarvestCoconut() // When Papayas are havrvested it goes back to the ThirdCycle
    {
        palmTreeStage4Prefab.SetActive(true);
        palmTreeStage5Prefab.SetActive(false);

        ableToHarvest = false;
        ableToCut = true;

        PalmLeavesCycle();

        Invoke("FruitCycle", fruitTime);
    }

    void FruitCycle()
    {
        palmTreeStage5Prefab.SetActive(true);
        palmTreeStage4Prefab.SetActive(false);

        ableToCut = true;
        ableToHarvest = true;

        PalmLeavesCycle();
    }

    void PalmLeavesCycle() // Does not work completely 
    {
        if (harvestPalm >= 60)
        {
            ableToHarvestPalm = true;
        }
    }

    void DecayCycle() // When cut down makes a Stump that will decay and destroy the empty
    {
        palmTreeStage6Prefab = Instantiate(palmTreeStage6Prefab, coconutPrefab.position, coconutPrefab.rotation) as GameObject;

        palmTreeStage6Prefab.transform.SetParent(coconutPrefab);
        palmTreeStage6Prefab.tag = coconutPrefab.tag; palmTreeStage6Prefab.layer = coconutPrefab.gameObject.layer;

        Destroy(palmTreeStage5Prefab);
        Destroy(palmTreeStage4Prefab);

        ableToCut = false;
        ableToHarvest = false;
        ableToHarvestPalm = false;

		SetFriendlyName("Coconut Tree Stump");

        Invoke("DestroyEmpty", decayTime);
    }

    void DestroyEmpty() // Destroys the empty
    {
        Destroy(palmTreeStage6Prefab);
        Destroy(empty);
        Destroy(this);
    }

    
}
