/**

InventorySquareController.cs

Copyright (C) 2019 Mike Santiago - All Rights Reserved
axiom@ignoresolutions.xyz

Permission to use, copy, modify, and/or distribute this software for any
purpose with or without fee is hereby granted, provided that the above
copyright notice and this permission notice appear in all copies.

THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF
OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.

*/

using System;
using System.Collections.Generic;
using Drifted.Inventory;
using Drifted.Items.Next;
using Drifted.UI;
using Drifted.UI.WindowManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Drifted.UI
{
    // Small script that attaches to the inventory squares to handle mouse over, click, etc.
    public class InventorySquareController : MonoBehaviour
    {
        //public Drifted.NextGen.Inventory.Inventory DisplayedInventory;
        public InventoryDisplayController Parent;


        #region Unity Properties

        public Image InventoryImage;
        public Image BackgroundImage;
        public Text StackCountText;

        public int InventoryIndex;
        public Vector3 PopUpOffset = new Vector3(0, -60, 0);
        public Vector3 MouseFollowOffset = new Vector3(22, -22, 0);
        #endregion

        #region Drifted API Properties
        public ItemContainer ContainedItem
        {
            get
            {
                if (Parent == null || Parent.InventoryToDisplay == null) return null;
                if (InventoryIndex > Parent.InventoryToDisplay.InventorySize()) return null;
                return Parent.InventoryToDisplay.GetItemAt(InventoryIndex);
            }
            set
            {
                Parent.InventoryToDisplay.SetItemAt(value, InventoryIndex);
            }
        }

        private GameObject Tooltip;
        private MikeMouseDummyController MouseDummyController;
        #endregion

        private Color DefaultSquareColor = new Color(.65f, .65f, .65f, .89f);
        private Color HighlightColor = new Color(.87f, .87f, .87f, 1.0f);
        private Font popUpFont;
        private bool initDone = false;
        private bool itemHeldByMouse = false;

        public void Awake()
        {
            if (InventoryImage == null || StackCountText == null)
            {
                InventoryImage = transform.GetChild(0).GetComponent<Image>();
                StackCountText = transform.GetChild(1).GetComponent<Text>();
            }

            BackgroundImage = transform.GetComponent<Image>();
            Refresh();
        }

        /// <summary>
        /// Refresh the specified item view.
        /// </summary>
        public void Refresh()
        {
            if(ContainedItem == null || ContainedItem.GetItem() == null)
            {
                InventoryImage.sprite = null;
                InventoryImage.color = new Color(0f, 0f, 0f, 0f);
                StackCountText.text = 0.ToString();
                StackCountText.gameObject.SetActive(false);
            }
            else
            {
                if (ContainedItem.Quantity == 0)
                {
                    ContainedItem.SetItem(null);
                    Refresh();
                }
                else
                {
                    InventoryImage.sprite = ContainedItem.GetItem().Icon;
                    InventoryImage.color = Color.white;
                    StackCountText.transform.gameObject.SetActive(ContainedItem.Quantity > 1);
                    StackCountText.text = ContainedItem.Quantity.ToString();
                }
            }
        }

        public void SimulateLeftClick()
        {
            if (Parent.InventoryToDisplay.itemInMouse == null || Parent.InventoryToDisplay.itemInMouse.GetItem() == null) // Nothing in our hand
            {
                if (ContainedItem.GetItem() == null) // Nothing in this slot
                    Refresh();

                else // We have picked up an item.
                {
                    Debug.Log($"Item at slot {InventoryIndex}: {ContainedItem.GetItem().ItemName}");
                    Parent.InventoryToDisplay.MoveItemToMouse(InventoryIndex);
                    Refresh();
                }
            }
            else // There's something in our hand
            {
                if(ContainedItem == null || ContainedItem.GetItem() == null) // Slot is empty
                {
                    Parent.InventoryToDisplay.MoveItemFromMouseTo(InventoryIndex);
                    Refresh();
                    return;
                }

                if(Parent.InventoryToDisplay.itemInMouse.GetItem().Stackable)
                {
                    if(Parent.InventoryToDisplay.itemInMouse.GetItem().ItemName == ContainedItem.GetItem().ItemName)
                    {
                        ContainedItem.Quantity += Parent.InventoryToDisplay.itemInMouse.Quantity;
                        Parent.InventoryToDisplay.itemInMouse = null;
                        Parent.MouseItemChangedEvent.Raise();
                        Refresh();
                        return;
                    }
                }

                if(Parent.InventoryToDisplay.itemInMouse != ContainedItem)
                {
                    ItemContainer currentlyInMouse = Parent.InventoryToDisplay.itemInMouse;
                    ItemContainer inSquare = ContainedItem;

                    Parent.InventoryToDisplay.itemInMouse = inSquare;
                    ContainedItem = currentlyInMouse;
                    Parent.MouseItemChangedEvent.Raise();
                    Refresh();
                    return;
                }
            }
        }

        public virtual AbstractMenuItem[] MakeInventoryPopup()
        {
            if (ContainedItem == null)
            {
                Debug.Log("Item null, skipping menu build.");
                return null;
            }

            List<AbstractMenuItem> menuItems = new List<AbstractMenuItem>();
            if (ContainedItem.GetItem() is UsableItem)
            {
                UsableItem asUsable = (ContainedItem.GetItem() as UsableItem);
                // TODO: Consume item on use?
                menuItems.Add(PopUpMenu.MakeMenuItem("Use", () =>
                {
                    asUsable.Use();
                    return true;
                }));
            }

            if (ContainedItem.GetItem() is PlantableFood)
            {
                PlantableFood asPlantable = (ContainedItem.GetItem() as PlantableFood);
                if (asPlantable.IsFood)
                {
                    menuItems.Add(PopUpMenu.MakeMenuItem("Eat", () =>
                    {
                        if (DriftedConstants.Instance.Player().Movement.IsEating)
                        {
                            Debug.Log("Eating, no more eat");
                            return false;
                        }

                        if (asPlantable.Eat())
                        {
                            ContainedItem.Quantity--;
                            return true;
                        }
                        return false;
                    }));
                }
                menuItems.Add(PopUpMenu.MakeMenuItem("Plant", () =>
                {
                    if (DriftedConstants.Instance.Player().Movement.IsBuilding) return false;

                    if (asPlantable.Plant())
                    {
                        Parent.player.EnqueueAction(() => DriftedConstants.Instance.Player().Movement.PlayBuildingAnimation(true));
                        Parent.player.EnqueueWait(2.0f);
                        Parent.player.EnqueueAction(() =>
                        {
                            ContainedItem.Quantity--;
                            DriftedConstants.Instance.Player().Movement.PlayBuildingAnimation(false);
                        });
                        return true;
                    }
                    return false;
                }));
            }

            if (ContainedItem.GetItem() is EdibleItem)
            {
                EdibleItem edible = (ContainedItem.GetItem() as EdibleItem);
                menuItems.Add(PopUpMenu.MakeMenuItem("Eat", () =>
                {
                    if (edible.Eat())
                    {
                        ContainedItem.Quantity--;
                        return true;
                    }
                    return false;
                }));
            }

            return menuItems.ToArray();
        }

        public void SimulateRightClick()
        {
            //var menus = ContainedItem.MakeInventoryPopup();
            var menus = MakeInventoryPopup();

            if(menus != null && menus.Length > 0)
            {
                if (Parent.MenuManager == null) return;

                var finalMenu = Parent.MenuManager.MakePopUpMenu(null, menus);
            }
        }

        private Transform oldParent;
        private bool InvSqrClickLock = true;
        public void OnPointerClick(PointerEventData eventData)
        {
            if (CustomInputManager.IsController) return;
            BackgroundImage.color = DefaultSquareColor;
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                SimulateLeftClick();
            }
            else if (eventData.button == PointerEventData.InputButton.Right && ContainedItem != null) // Show Context menu
            {
                SimulateRightClick();
            }
        }

        private EzTimer HoldTimer = new EzTimer(3.0f, () => { }, false);

        [ReadOnly]
        public string ThisItem = "Nothing";
        void Update()
        {
            /*
            if (ContainedItem == null || ContainedItem.GetItem() == null) ThisItem = "Nothing";
            else ThisItem = ContainedItem.GetItem().ItemName;
            */           

            //Refresh();
        }

        private void MakeToolTip()
        {
            if (ContainedItem == null) return;
            if (ContainedItem.GetItem() == null) return;

            WMTooltip toolTipParams;
            if (Tooltip == null)
            {
                Tooltip = new GameObject("Item Tooltip");
                //Tooltip.transform.SetParent(WM.GetCanvas().transform);
                toolTipParams = Tooltip.AddComponent<WMTooltip>();
            }
            else toolTipParams = Tooltip.GetComponent<WMTooltip>();

            if(toolTipParams != null)
            {
                toolTipParams.FollowTarget = gameObject;
                toolTipParams.TurnIntoTooltip();
                toolTipParams.SetTooltipText($"{ContainedItem.GetItem().ItemName}");
            }
        }

        private void OnDisable()
        {
            if (Tooltip != null) mDestroyTooltip();
        }

        public void HighlightCurrentItem()
        {
            if (BackgroundImage == null) return;

            if (ContainedItem != null && Tooltip == null) MakeToolTip();

            if (Tooltip != null) Tooltip.SetActive(true);

            BackgroundImage.color = HighlightColor;
            Highlighted = true;
        }

        public void Unhighlight()
        {
            if (BackgroundImage == null) { Debug.LogWarning("Null"); return; }

            BackgroundImage.color = DefaultSquareColor;
            Highlighted = false;
            // TODO: this needs to be done better.
            if (Tooltip != null) Invoke("mDestroyTooltip", 0.10f);
        }

        private bool Highlighted = false;

        private void mDestroyTooltip() => Destroy(Tooltip);

        public void ShowTooltip()
        {
            if (Tooltip == null) MakeToolTip();

            if (Tooltip != null) Tooltip.SetActive(true);
        }

        public void HideTooltip()
        {
            if (Tooltip != null) Invoke("mDestroyTooltip", 0.1f);
        }
    }

}
