/*
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

using Drifted.Input;
using Drifted.Items.ItemDefinitions;
using Drifted.UI;
using Drifted.UI.WindowManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Drifted.Inventory
{
    [Serializable]
    public class InventorySquareGameObject
    {
        public Image BGImage;
        public Image Sprite;
        public Text StackCountText;
    }

    public class InventoryDisplayController : MonoBehaviour
    {
        public NextGen.Inventory.Inventory InventoryToDisplay;
        public MenuManager MenuManager;
        public MoodsManager PlayerMoods;
        public GameEvent MouseItemChangedEvent;
        public PlayerMovement player;

        public Transform InventorySquaresContainer;
        private List<InventorySquareController> InventorySquares;

        private EzTimer ToggleKeyDelayTimer = new EzTimer(0.25f, null, false);
        private bool AllowedToToggle = true;
        private bool Display = false;

        public bool GiveDebugItems = true;

        [SerializeField]
        [ReadOnly]
        private Vector2 CursorPosition = new Vector2(0, 0);

        public void Start()
        {
            InventoryToDisplay.InventoryChanged += RefreshView;
        }

        public void Awake()
        {
            // TODO: this must be ok for scriptable objects
            /*
            if (InventoryToDisplay == null) InventoryToDisplay = Instantiate(InventoryToDisplay);
            if (MenuManager == null) MenuManager = Instantiate(MenuManager);
            if (PlayerMoods == null) PlayerMoods = Instantiate(PlayerMoods);
            */

            if(InventoryToDisplay != null)
            {
                //                    PlayerInventoryController.ResizeInventory(InventorySquares.Count);
                FindSquareControllers();
                ToggleKeyDelayTimer.ChangeAction(() => AllowedToToggle = false);
            }

            FindSquareControllers();
            //WindowManager = DriftedConstants.Instance.UI().WindowManager;
            if (InventoryToDisplay == null) return;

            ToggleKeyDelayTimer.ChangeAction(() => AllowedToToggle = false);

            player = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        }



        private void FindSquareControllers()
        {
            InventorySquares = new List<InventorySquareController>();
            for(int i = 0; i < InventorySquaresContainer.childCount; i++)
            {
                Transform child = InventorySquaresContainer.transform.GetChild(i);
                if(child != null)
                {
                    InventorySquareController sqrController = child.GetComponent<InventorySquareController>();
                    if(sqrController != null)
                    {
                        sqrController.InventoryIndex = i;
                        sqrController.Parent = this;
                        sqrController.Refresh();
                        InventorySquares.Add(sqrController);
                    }
                }
            }
        }

        private void Update()
        {
            if (ToggleKeyDelayTimer.enabled) ToggleKeyDelayTimer.Tick(Time.deltaTime);

            if(DriftedInputManager.IsController)
            {
                PositionCrosshair();
            }
        }

        private void PositionCrosshair()
        {
            //GameObject currentSelected = EventSystem.current.currentSelectedGameObject;

            //if(currentSelected != null)
            //{
                //DriftedConstants.Instance.UI().CursorHandler.SetPosition(currentSelected.transform.position);
            //}
        }

        void OnDrawGizmos()
        {
            if (Debug.isDebugBuild) // Draws a nice debug placement sphere
            {
                GameObject marty = GameObject.FindWithTag("Player");
                Vector3 plantPosition = Vector3.zero;
                if (marty != null) { plantPosition = marty.transform.position + marty.transform.forward; plantPosition.y += 1.0f; }
                Gizmos.color = new Color(1f, 0f, 0f, .34f);
                Gizmos.DrawSphere(plantPosition, 0.25f);
            }
        }

        private void RefreshView()
        {
            if(InventorySquares != null && InventorySquares.Count > 0)
            {
                for(int i = 0; i < InventorySquares.Count; i++)
                {
                    if (InventorySquares[i] != null) InventorySquares[i].Refresh();

                    //if (InventorySquares[i].ContainedItem != null && InventorySquares[i].ContainedItem.Quantity == 0) PlayerInventoryController.Inventory.NullItemAt(i);
                }
            }
        }
    }
}
