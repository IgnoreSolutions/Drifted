using Drifted;
using Drifted.Interactivity;
using Drifted.UI;
using UnityEngine;


public class PapayaSeedling : DriftedSceneInteractable
{
    [Header("New Stuff")]
    public GameEvent newCameraTargetEvent;
    public PlayerMovement playerMovement;

    // Growing Values
    [Header("Growing Values")]
    public Transform PapayaTreeSeed;
    public GameObject Empty; // The empty that the GameObjects spawn on
    public GameObject PapayaTreeStage1Prefab;
    public GameObject PapayaTreeStage2Prefab;
    public GameObject PapayaTreeStage3Prefab;
    public GameObject PapayaTreeStage4Prefab;
    public GameObject PapayaTreeStage5Prefab;
    public GameObject PapayaTreeStage6Prefab;
    public float growthTime = 60.0f;
    public float harvestTime = 0.0f;
    public float decayTime = 120.0f;
    public float fruitTime = 120.0f;

    public bool harvest = false;
    public bool cut = false;

    protected override void Awake()
    {
        base.Awake();
        GameObject playerController = GameObject.FindWithTag("Player");
        playerMovement = playerController.GetComponent<PlayerMovement>();

    }

    /*private void OnDrawGizmos()
    {
        if (Application.isEditor && PapayaTreeStage5Prefab != null)
        {
            MeshFilter mesh = PapayaTreeStage5Prefab.GetComponent<MeshFilter>();
            Gizmos.DrawMesh(mesh.sharedMesh, transform.position, Quaternion.Euler(-90, 0, 0));
        }

    }*/

    public override void Interact(MonoBehaviour sender)
    {
        if (PapayaTreeStage3Prefab == null || PapayaTreeStage4Prefab == null)
        {
            BuildMenu();
        }
    }

    //private int menuID = -1;
    void BuildMenu()
    {
        if (menuManager == null) return; // No need to build another, it's already open.

        AbstractMenuItem[] items = new AbstractMenuItem[2];
        items[0] = PopUpMenu.MakeMenuItem("Harvest Papaya", Harvest, false);
        items[1] = PopUpMenu.MakeMenuItem("Cut Papaya Tree", CutTree, false);

        menuManager.MakePopUpMenu(this.gameObject, items);
    }

    bool Harvest()  // Does not currently work, i need to save the Stage 4 and Stage 5 so i can call on them later
    {
        if (Skills.Harvesting.Level >= 5 && harvest == true)
        {
            /*
            InventoryItem itemToAdd = new ItemAvocado();
            itemToAdd.Quantity = 3;
            InventoryController.AddItem(itemToAdd);
            */           

            Console.AddLine("You gained 5 Exp in Harvest");

            Skills.Harvesting.GainExp(5);

            Invoke("HarvestPapaya", harvestTime);
            return true;
        }
        else
        {
            Console.AddLine("You cannot Harvest this Tree, You need to be Harvest lvl 5");
        }
        return false;
    }

    bool CutTree()
    {
        if (Skills.WoodCutting.Level >= 10 && cut == true) // Checks if Player is lvl 10 Wood Cutting
        {
            /*
            InventoryItem itemToAdd = new ItemAvocadoWood();
            itemToAdd.Quantity = 3;
            InventoryController.AddItem(itemToAdd);
            */           

            Invoke("DecayCycle", harvestTime);
            Destroy(PapayaTreeStage4Prefab);
            Destroy(PapayaTreeStage5Prefab);
            PapayaTreeStage4Prefab.SetActive(false);
            PapayaTreeStage5Prefab.SetActive(false);

            Skills.WoodCutting.GainExp(15);

            Console.AddLine("You Gained 15 Exp in Wood Cutting");
            return true;
        }
        else
        {
            Console.AddLine("You cannot cut down this tree you are not Wood Cutting lvl 10");
        }
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        PapayaTreeStage1Prefab = Instantiate(PapayaTreeStage1Prefab, PapayaTreeSeed.position, PapayaTreeSeed.rotation) as GameObject;
        PapayaTreeStage1Prefab.tag = PapayaTreeSeed.tag; PapayaTreeStage1Prefab.layer = PapayaTreeSeed.gameObject.layer;
        PapayaTreeStage1Prefab.transform.SetParent(PapayaTreeSeed);

        //Skills.Farming.GainExp(10);

        Invoke("FirstCycle", growthTime);
    }

    void FirstCycle()
    {
        PapayaTreeStage2Prefab = Instantiate(PapayaTreeStage2Prefab, PapayaTreeSeed.position, PapayaTreeSeed.rotation) as GameObject;
        PapayaTreeStage2Prefab.tag = PapayaTreeSeed.tag; PapayaTreeStage2Prefab.layer = PapayaTreeSeed.gameObject.layer;
        PapayaTreeStage2Prefab.transform.SetParent(PapayaTreeSeed);
        Destroy(PapayaTreeStage1Prefab);

        Invoke("SecondCycle", growthTime);
    }

    void SecondCycle()
    {
        PapayaTreeStage3Prefab = Instantiate(PapayaTreeStage3Prefab, PapayaTreeSeed.position, PapayaTreeSeed.rotation) as GameObject;
        PapayaTreeStage3Prefab.tag = PapayaTreeSeed.tag; PapayaTreeStage3Prefab.layer = PapayaTreeSeed.gameObject.layer;
        PapayaTreeStage3Prefab.transform.SetParent(PapayaTreeSeed);
        Destroy(PapayaTreeStage2Prefab);

        Invoke("ThirdCycle", growthTime);
    }

    void ThirdCycle() // Full Grown but has no Fruit
    {
        PapayaTreeStage4Prefab = Instantiate(PapayaTreeStage4Prefab, PapayaTreeSeed.position, PapayaTreeSeed.rotation) as GameObject;

        PapayaTreeStage4Prefab.transform.SetParent(PapayaTreeSeed);
        PapayaTreeStage4Prefab.tag = PapayaTreeSeed.tag; PapayaTreeStage4Prefab.layer = PapayaTreeSeed.gameObject.layer;
        PapayaTreeStage4Prefab.transform.rotation = Quaternion.Euler(90, PapayaTreeStage4Prefab.transform.rotation.y, PapayaTreeStage4Prefab.transform.rotation.z);

        Destroy(PapayaTreeStage3Prefab);

        cut = true;

        Invoke("FourthCycle", growthTime);
    }

    void FourthCycle() // Papaya Tree Has Fruit
    {
        PapayaTreeStage5Prefab = Instantiate(PapayaTreeStage5Prefab, PapayaTreeSeed.position, PapayaTreeSeed.rotation) as GameObject;

        PapayaTreeStage5Prefab.transform.SetParent(PapayaTreeSeed);
        PapayaTreeStage5Prefab.tag = PapayaTreeSeed.tag; PapayaTreeStage5Prefab.layer = PapayaTreeSeed.gameObject.layer;
        PapayaTreeStage5Prefab.transform.rotation = Quaternion.Euler(90, PapayaTreeStage5Prefab.transform.rotation.y, PapayaTreeStage5Prefab.transform.rotation.z);

        cut = true;
        harvest = true;

        PapayaTreeStage4Prefab.SetActive(false);
    }

    void HarvestPapaya() // When Papayas are havrvested it goes back to the ThirdCycle
    {
        PapayaTreeStage4Prefab.SetActive(true);
        PapayaTreeStage5Prefab.SetActive(false);

        cut = true;

        Invoke("FruitCycle", fruitTime);
    }

    void FruitCycle()
    {
        PapayaTreeStage5Prefab.SetActive(true);
        PapayaTreeStage4Prefab.SetActive(false);

        harvest = true;
        cut = true;
    }

    void DecayCycle() // When cut down makes a Stump that will decay and destroy the empty
    {
        PapayaTreeStage6Prefab = Instantiate(PapayaTreeStage6Prefab, PapayaTreeSeed.position, PapayaTreeSeed.rotation) as GameObject;

        PapayaTreeStage6Prefab.transform.rotation = Quaternion.Euler(90, PapayaTreeStage6Prefab.transform.rotation.y, PapayaTreeStage6Prefab.transform.rotation.z);

        PapayaTreeStage6Prefab.transform.SetParent(PapayaTreeSeed);
        PapayaTreeStage6Prefab.tag = PapayaTreeSeed.tag; PapayaTreeStage6Prefab.layer = PapayaTreeSeed.gameObject.layer;

        Destroy(PapayaTreeStage5Prefab);
        Destroy(PapayaTreeStage4Prefab);

        cut = false;
        harvest = false;

        Invoke("DestroyEmpty", decayTime);
    }

    void DestroyEmpty()
    {
        Destroy(Empty);
        Destroy(PapayaTreeStage6Prefab);
    }

    
}
