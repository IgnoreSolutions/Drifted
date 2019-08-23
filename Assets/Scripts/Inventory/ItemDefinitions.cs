using System;
using System.Collections.Generic;
using Drifted.Inventory;
using Drifted.Items.ItemDefinitions;
using Drifted.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Drifted.Items.ItemDefinitions
{


    /// <summary>
    /// Defines a template for an InventoryItem
    /// </summary>
    public abstract class InventoryItem
    {
        public event EventHandler QuantityChanged;

        // Parameters
        public virtual int ID { get; internal set; }
        public virtual string Name { get; internal set; }
        public virtual string Description { get; internal set; }

        public Sprite Icon { get; internal set; }

        public virtual string ItemSpriteName { get; internal set; }

        private int m_Quantity = 1;
        public int Quantity 
        { 
            get => m_Quantity;
            internal set
            {
                if(value == 0) ID = -1;
                m_Quantity = value;
                //Debug.Log("Quantity Changed.");
                QuantityChanged?.Invoke(this, null);
            } 
        }

        public int InventoryIndex { get; internal set; }

        public virtual bool Stackable { get; internal set; } = true;

        // Methods

        public virtual AbstractMenuItem[] MakeInventoryPopup() { return null; }

        public InventoryItem()
        {
        }
        public InventoryItem(int quantity = 1)
        {
            Quantity = quantity;
        }
    }

    public abstract class PlantableFood : InventoryItem, IPlantable, IEdible
    {
        public abstract string SeedlingPrefabName { get; set; }
        public virtual int ExperienceGain { get; set; } = 5;
        public float GoodnessFactor { get; set; }
        public float ImmediateHealthRestoration { get; set; }

        public virtual GameObject Plant()
        {
            if (SeedlingPrefabName == "NONE") return null;
            GameObject player = GameObject.FindWithTag("Player");
            GameObject environmentalLayer = GameObject.FindWithTag("Environment");
            if (environmentalLayer == null)
            {
                Debug.LogError("Environmental group not tagged!");
                return null;
            }

            if (player.GetComponent<PlayerMovement>().PlayerIsMoving)
            {
                Debug.Log("Player moving. return nothing.");
                return null; // Can't plant while moving.
            }

            Quantity--;
            
            //else return null; // just in case

            Vector3 plantPosition = GetPlantingPosition();

            GameObject planted = (GameObject)UnityEngine.Object.Instantiate(Resources.Load(SeedlingPrefabName));
            planted.tag = "Environment";
            planted.layer = 10;
            planted.transform.SetParent(environmentalLayer.transform);
            planted.transform.position = plantPosition + new Vector3(0, 5f, 0);
            if (planted.GetComponent<SnapToGround>() == null) planted.AddComponent<SnapToGround>();

            if (DriftedConstants.Instance.UI().Skills != null && plantPosition != Vector3.zero)
            {
                DriftedConstants.Instance.UI().Skills.Farming.GainExp(ExperienceGain);
            }
            else Debug.Log("It's null.");

            return planted;
        }

        private Vector3 GetPlantingPosition()
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player == null) Debug.LogError("Couldn't find a player");
            Vector3 plantingPosition = player.transform.position + (player.transform.forward * 2);
            plantingPosition.y += 1.0f;
            return plantingPosition;
        }

        public override AbstractMenuItem[] MakeInventoryPopup()
        {
            AbstractMenuItem[] menuItems = new AbstractMenuItem[3];
            menuItems[0] = PopUpMenu.MakeMenuItem("Eat", () =>
            {
                //DriftedConstants.Instance.PlayerMoods.Eat(this);
                Quantity--;
                return true;
            });

            menuItems[1] = PopUpMenu.MakeMenuItem("Cook", () =>
            {
                DriftedConstants.Instance.UI().Console.AddLine("<i>TODO:</i> Cook this!");
                return true;
            });
            menuItems[2] = PopUpMenu.MakeMenuItem("Plant", () =>
            {
                Vector3 plantPosition = GetPlantingPosition();
                var hitColliders = Physics.OverlapSphere(plantPosition, 0.25f);
                if (hitColliders.Length > 0) DriftedConstants.Instance.UI().Console.AddLine("You can't plant here!");
                else
                {
                    if (Plant() != null)
                    {
                        DriftedConstants.Instance.UI().Console.AddLine($"Planting at <b>({plantPosition.x}, {plantPosition.y}, {plantPosition.z})</b>");
                        return true;
                    }
                    else DriftedConstants.Instance.UI().Console.AddLine("Unable to plant here.");
                }
                return false;
            });

            return menuItems;
        }
    }

    public abstract class EdibleItem : InventoryItem, IEdible
    {
        /// <summary>
        /// The goodness factor, used by the hungermood for calculating how much of your hunger to restore.
        /// 
        /// Keep this value pretty low otherwise things can get funky.
        /// </summary>
        /// <value>The goodness factor.</value>
        public float GoodnessFactor { get; set; } = 1;
        public float ImmediateHealthRestoration { get; set; } = 0;

        public override AbstractMenuItem[] MakeInventoryPopup()
        {
            AbstractMenuItem[] menus = new AbstractMenuItem[2];
            menus[0] = PopUpMenu.MakeMenuItem("Eat", () =>
            {
                //DriftedConstants.Instance.PlayerMoods.Eat(this);
                Quantity--;
                return true;
            });
            if (this is ICookable)
            {
                menus[1] = PopUpMenu.MakeMenuItem("Cook", () =>
                {
                    DriftedConstants.Instance.UI().Console.AddLine("<i>TODO:</i> Cook this!");
                    return true;
                });
            }
            return menus;
        }
    }

    public abstract class PlantableItem : InventoryItem, IPlantable
    {
        public string SeedlingPrefabName { get; set; } = "NONE";

        // How much Exp should be gained from planting this.
        public virtual int ExperienceGain { get; set; } = 5;

        public virtual GameObject Plant()
        {
            if (SeedlingPrefabName == "NONE") return null;

            GameObject player = GameObject.FindWithTag("Player");
            GameObject environmentalLayer = GameObject.FindWithTag("Environment");

            Vector3 plantPosition = player.transform.position + player.transform.forward;

            GameObject planted = (GameObject)UnityEngine.Object.Instantiate(Resources.Load(SeedlingPrefabName));
            planted.tag = "Environment";
            planted.layer = 10;
            planted.transform.SetParent(environmentalLayer.transform);
            planted.transform.position = plantPosition + new Vector3(0, 5f, 0);
            planted.AddComponent<SnapToGround>();

            if(DriftedConstants.Instance.UI().Skills != null) 
            {
                Debug.Log("Experience gain");
                DriftedConstants.Instance.UI().Skills.Farming.GainExp(ExperienceGain);
            }
            else Debug.Log("It's null.");

            
            return planted;
        }
    }

    public interface IPlantable
    {
        string SeedlingPrefabName { get; set; }
        int ExperienceGain { get; set; }
    }

    public interface IEdible
    {
        float GoodnessFactor { get; set; }
        float ImmediateHealthRestoration { get; set; }
    }

    public interface ICookable
    {
        InventoryItem OnCook();
    }

    #region Begin Item Defs

    public class ItemSword : InventoryItem
    {
        public override string Name { get; internal set; } = "Sword";
        public override string Description { get; internal set; } = "Was once mighty, not so much anymore.";
        public override string ItemSpriteName { get; internal set; } = "mc_items_66";
        public override int ID { get; internal set; } = 1;
        public override bool Stackable { get; internal set; } = false;
    }

    public class ItemStick : InventoryItem
    {
        public override string Name { get; internal set; } = "Stick";
        public override string Description { get; internal set; } = "I should find a dog now.";
        public override string ItemSpriteName { get; internal set; } = "item_stick";
        public override int ID { get; internal set; } = 2;
    }

    public class ItemPalmLeaf : InventoryItem
    {
        public override string Name { get; internal set; } = "Palm Frond";
        public override string Description { get; internal set; } = "A large green leaf that fell from a palm. Might make a nice fan.";
        public override string ItemSpriteName { get; internal set; } = "item_palm_leaf";
        public override int ID { get; internal set; } = 3;
    }

    public class ItemCoconut : PlantableFood
    {
        public override string Name { get; internal set; } = "Coconut";
        public override string Description { get; internal set; } = "Fell from a tree. If I found a rock, I bet I could bust it wide open.";
        public override string ItemSpriteName { get; internal set; } = "item_coconut";
        public override int ID { get; internal set; } = 4;

        public override string SeedlingPrefabName { get; set; } = "Prefabs/Environment Prefabs/Coconut Tree/CoconutTreeSeed";
        //public override int ExperienceGain { get; set; } = 50;
        public override int ExperienceGain { get; set; } = 10;


        public ItemCoconut()
        {
            // Edible
            GoodnessFactor = 1.134f;
        }
    }

    public class ItemRedMeat : EdibleItem, ICookable
    {
        public override string Name { get; internal set; } = "Raw Meat";
        public override string Description { get; internal set; } = "A little <i>too</i> rare for my tastes.";
        public override string ItemSpriteName { get; internal set; } = "item_raw_meat";
        public override int ID { get; internal set; } = 5;

        public ItemRedMeat()
        {
            GoodnessFactor = 0.43f;
        }

        public InventoryItem OnCook() => new ItemCookedMeat();
    }

    public class ItemCookedMeat : EdibleItem
    {
        public override string Name { get; internal set; } = "Cooked Meat";
        public override string Description { get; internal set; } = "Now this is class!";
        public override string ItemSpriteName { get; internal set; } = "mc_items_cookedmeat";
        public override int ID { get; internal set; } = 6;
        public ItemCookedMeat()
        {
            GoodnessFactor = 1.43f;
        }
    }

    public class ItemSpear : InventoryItem
    {
        public override string Name { get; internal set; } = "Spear";
        public override string Description { get; internal set; } = "This would be great to fish with.";
        public override string ItemSpriteName { get; internal set; } = "item_spear";
        public override int ID { get; internal set; } = 6;
        public override bool Stackable { get; internal set; } = false;
    }

    public class ItemAxe : InventoryItem
    {
        public override string Name { get; internal set; } = "Axe";
        public override string Description { get; internal set; } = "The latest advancement in violent technology.";
        public override string ItemSpriteName { get; internal set; } = "mc_items_axe";
        public override int ID { get; internal set; } = 7;
        public override bool Stackable { get; internal set; } = false;
    }

    public class ItemRawShark : EdibleItem, ICookable
    {
        public override string Name { get; internal set; } = "Raw Shark Meat";
        public override string Description { get; internal set; } = "Tonight, we feast!";
        public override string ItemSpriteName { get; internal set; } = "item_raw_shark";
        public override int ID { get; internal set; } = 8;

        public ItemRawShark()
        {
            GoodnessFactor = 1.308f;
        }

        public InventoryItem OnCook() => new ItemCookedMeat();
    }

    public class ItemAvocado : EdibleItem
    {
        public override string Name { get; internal set; } = "Avocado";
        public override string Description { get; internal set; } = "I didn't know these grew on trees.";
        public override string ItemSpriteName { get; internal set; } = "item_avocado";
        public override int ID { get; internal set; } = 9;

        public ItemAvocado()
        {
            GoodnessFactor = 1.2075f;

            // TODO on item eat event
        }
    }

    public class ItemAvocadoSeed : PlantableItem
    {
        public override string Name { get; internal set; } = "Avocado Seed";
        public override string Description { get; internal set; } = "It's massive.";
        public override string ItemSpriteName { get; internal set; } = "item_avocado_seed";
        public override int ID { get; internal set; } = 15;

        public ItemAvocadoSeed()
        {
            SeedlingPrefabName = "NONE";
        }
    }

    public class ItemAvocadoWood : InventoryItem
    {
        public override string Name { get; internal set; } = "Avocado Wood";
        public override string Description { get; internal set; } = "Wood from an avocado tree.";
        public override string ItemSpriteName { get; internal set; } = "item_avocado_log";
        public override int ID { get; internal set; } = 11;
    }

    public class ItemCoconutWood : InventoryItem
    {
        public override string Name { get; internal set; } = "Coconut Wood";
        public override string Description { get; internal set; } = "Wood from a coconut tree";
        public override string ItemSpriteName { get; internal set; } = "item_coconut_log";
        public override int ID { get; internal set; } = 12;
    }

    public class ItemBamboo : InventoryItem
    {
        public override string Name { get; internal set; } = "Bamboo";
        public override string Description { get; internal set; } = "It's technically a weed, but at least it's a useful one.";
        public override string ItemSpriteName { get; internal set; } = "item_bamboo";
        public override int ID { get; internal set; } = 13;
    }

    public class ItemGrass : InventoryItem
    {
        public override string Name { get; internal set; } = "Grass";
        public override string Description { get; internal set; } = "Put that in your pipe and smoke it!";
        public override string ItemSpriteName { get; internal set; } = "item_grass";
        public override int ID { get; internal set; } = 14;
    }

    public class ItemPapaya : EdibleItem
    {
        public override string Name { get; internal set; } = "Papaya";
        public override string Description { get; internal set; } = "It looks exotic, like a Mercedes so it must be good.";
        public override string ItemSpriteName { get; internal set; } = "item_papaya";
        public override int ID { get; internal set; } = 15;
    }

    public class ItemRock : InventoryItem
    {
        public override string Name { get; internal set; } = "Rock";
        public override string Description { get; internal set; } = "Blunt force object inbound in 3..2...1....";
        public override string ItemSpriteName { get; internal set; } = "item_rock";
        public override int ID { get; internal set; } = 16;
        public override bool Stackable { get; internal set; } = true;
    }

    public class ItemSharkSpear : InventoryItem
    {
        public override string Name { get; internal set; } = "Sharktooth Spear";
        public override string Description { get; internal set; } = "Spear++";
        public override string ItemSpriteName { get; internal set; } = "item_shark_spear";
        public override int ID { get; internal set; } = 17;
        public override bool Stackable { get; internal set; } = false;
    }
    #endregion

    [Serializable]
    public class DriftedItems : Singleton<MonoBehaviour>
    {
        public Dictionary<int, InventoryItem> ItemDefinitions = new Dictionary<int, InventoryItem>();

        private void Awake()
        {
        }
    }
}