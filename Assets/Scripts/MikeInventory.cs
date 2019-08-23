/**

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
using System.Linq;
using Drifted;
using Drifted.Input;
using Drifted.Items.ItemDefinitions;
using Drifted.Items.Next;
using Drifted.UI;
using Drifted.UI.WindowManager;
using UnityEngine;
using UnityEngine.UI;


namespace Drifted.Inventory
{


    /// <summary>
    /// Defines the inventory class that holds instances of Item
    /// </summary>
    [Obsolete("USE DRIFTED.NEXTGEN.INVENTORY")]
    public class MikeInventory : MonoBehaviour
    {
        /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// 

        /**
         * Public properties, ones that Unity will be able to see :)
         */
        public int MaxInventorySlots = 15;

        public MikeWindowManager WindowManager;
        public InventoryHandler InventoryController;

        public Vector2 InventoryGridSize = new Vector2(5, 3);
        public Vector2 InventoryWindowSize = new Vector2(350, 275);

        public Image InventoryBaseImage;
        public Canvas InventoryCanvas = null;
        public bool IsInventoryActive = false;

        public int CurrentActiveItemIndex = 0;
        /**
         * End
         */

        /**
         * Handling of key timeout so we don't get super presses
         */
        private const float inventoryKeyDelay = 0.25f;
        private EzTimer keyDelayTimer;
        private bool allowedPress = true;

        public GameObject InventoryWindow;
        private Font DefaultFont;

        private const string InventoryWindowName = "Inventory";
        private const string InventorySquareName = "Inventory Square";
        /**
         * End  
         */

        /**
         * Methods
         */
        void Start()
        {

        }

        private void ClearExisting()
        {
            if (InventoryCanvas == null) throw new Exception("Inventory canvas is null");

            for (int i = 0; i < InventoryCanvas.transform.childCount; i++)
            {
                GameObject child = InventoryCanvas.transform.GetChild(i).gameObject;
                if (child != null)
                {
                    if (child.name == InventoryWindowName)
                        DestroyImmediate(child);
                }
            }
        }

        private GameObject GetChildByName(string childName)
        {
            if (InventoryCanvas == null) throw new Exception("Inventory canvas is null.");

            for (int i = 0; i < InventoryCanvas.transform.childCount; i++)
            {
                GameObject child = InventoryCanvas.transform.GetChild(i).gameObject;
                if (child != null)
                {
                    if (child.name == childName)
                        return child;
                }
            }

            return null;
        }

        private List<GameObject> ReturnAllContaining(Transform parent, string containing)
        {
            List<GameObject> gameObjects = new List<GameObject>();

            for (int i = 0; i < parent.childCount; i++)
            {
                GameObject child = parent.GetChild(i).gameObject;
                if (child != null && child.name.Contains(containing)) gameObjects.Add(child);
            }

            return gameObjects;
        }

        private void Awake()
        {
            // Validate grid size against Max slots
            int gridSizeSlots = Mathf.FloorToInt(InventoryGridSize.x * InventoryGridSize.y);
            if (gridSizeSlots > MaxInventorySlots) throw new Exception("Mismatch between grid size slots and max inventory slots.");
            // End

            // Default font
            DefaultFont = Resources.Load<Font>("Fonts/game");
            // End

            // Validate Grid size
            InventoryGridSize = new Vector2(Mathf.Floor(InventoryGridSize.x), Mathf.Floor(InventoryGridSize.y));
            // End

            // Build raw inventory data structures
            if (keyDelayTimer == null) keyDelayTimer = new EzTimer(0, () => { });
            // End

            CurrentActiveItemIndex = 0;

            // TODO: Do this better. Disable this before u start doing it better.
            if (this.InventoryWindow != null && UnityEngine.Application.isEditor) DestroyImmediate(InventoryWindow);
            if (this.InventoryWindow != null) Destroy(InventoryWindow);

            InventoryWindow = BuildInventory();
        }

        GameObject BuildInventoryWindowSlim()
        {
            if (WindowManager == null) throw new Exception("Window manager cannot be null.");

            GameObject newWindow = WindowManager.CreateWindow(InventoryWindowSize, "Inventory", new Color(0.2f, 0.2f, 0.2f, 0.87f));
            if (newWindow != null)
            {
                newWindow.SetActive(IsInventoryActive);
                newWindow.transform.SetSiblingIndex(0);
                return newWindow;
            }
            throw new Exception("Couldn't make a new Inventory Window?!?!");
        }

        private GameObject MakeTrashSquare(GameObject parent)
        {
            // Square
            var newSqrGO = new GameObject();
            newSqrGO.name = $"Trash Square";

            var layoutElementProperties = newSqrGO.AddComponent<LayoutElement>();
            layoutElementProperties.ignoreLayout = true;
            // Square

            // Square -> Sprite
            var newSqrSprite = new GameObject();
            newSqrSprite.name = "Inv Sprite";
            var newSqrSpriteImage = newSqrSprite.AddComponent<Image>();
            var newSqrSprRect = newSqrSprite.GetComponent<RectTransform>();
            newSqrSpriteImage.color = new Color(0, 0, 0, 0); // totally invisible.

            newSqrSprRect.SetParent(newSqrGO.transform);
            newSqrSprRect.sizeDelta = new Vector2(32, 32);
            newSqrSprRect.localPosition = Vector2.zero;
            // End Item Sprite

            // Square, Image component
            Image inventoryBaseSqr = newSqrGO.AddComponent<Image>();
            inventoryBaseSqr.color = new Color(0.65f, 0.25f, 0.25f, 0.89f);

            var rectTransform = newSqrGO.GetComponent<RectTransform>();
            rectTransform.SetParent(parent.transform);
            rectTransform.sizeDelta = new Vector2(32, 32);
            rectTransform.localPosition = new Vector3(-100f, -100f, 1);
            // End Background Square

            // Square, Inventory Sqr Controller
            var invSqrCon = newSqrGO.AddComponent<InvTrashSquare>();

            newSqrGO.SetActive(true);
            // End

            return newSqrGO;
        }

        private GameObject MakeInformationSquare(GameObject parent)
        {
            // Square
            var newSqrGO = new GameObject();
            newSqrGO.name = $"Info Square";

            var layoutElementProperties = newSqrGO.AddComponent<LayoutElement>();
            layoutElementProperties.ignoreLayout = true;
            // Square

            // Square -> Sprite
            var newSqrSprite = new GameObject();
            newSqrSprite.name = "Inv Sprite";
            var newSqrSpriteImage = newSqrSprite.AddComponent<Image>();
            var newSqrSprRect = newSqrSprite.GetComponent<RectTransform>();
            newSqrSpriteImage.color = new Color(0, 0, 0, 0); // totally invisible.

            newSqrSprRect.SetParent(newSqrGO.transform);
            newSqrSprRect.sizeDelta = new Vector2(32, 32);
            newSqrSprRect.localPosition = Vector2.zero;
            // End Item Sprite

            // Square, Image component
            Image inventoryBaseSqr = newSqrGO.AddComponent<Image>();
            inventoryBaseSqr.color = new Color(0.65f, 0.65f, 0.65f, 0.89f);

            var rectTransform = newSqrGO.GetComponent<RectTransform>();
            rectTransform.SetParent(parent.transform);
            rectTransform.sizeDelta = new Vector2(32, 32);
            rectTransform.localPosition = new Vector3(145, -100f, 1);
            // End Background Square

            // Square, Inventory Sqr Controller
            var invSqrCon = newSqrGO.AddComponent<InvInfoSquare>();

            newSqrGO.SetActive(true);
            // End

            return newSqrGO;
        }

        private GameObject MakeInventorySquare(GameObject parent, int indexInInv, int x, int y)
        {
            // Square
            var newSqrGO = new GameObject();
            newSqrGO.name = $"{InventorySquareName} {x} x {y}";

            var layoutElementProperties = newSqrGO.AddComponent<LayoutElement>();
            layoutElementProperties.ignoreLayout = true;
            // Square

            // Square -> Sprite
            var newSqrSprite = new GameObject();
            newSqrSprite.name = "Inv Sprite";
            var newSqrSpriteImage = newSqrSprite.AddComponent<Image>();
            var newSqrSprRect = newSqrSprite.GetComponent<RectTransform>();
            newSqrSpriteImage.color = new Color(0, 0, 0, 0); // totally invisible.

            newSqrSprRect.SetParent(newSqrGO.transform);
            newSqrSprRect.sizeDelta = new Vector2(32, 32);
            newSqrSprRect.localPosition = Vector2.zero;
            // End Item Sprite

            // Square -> Stack Count
            var stackCount = new GameObject("Stack Count");
            Text stackCountText = stackCount.AddComponent<Text>();
            stackCountText.font = DefaultFont;
            stackCountText.text = "0";
            stackCountText.color = Color.black;


            var stackCountTransform = stackCount.GetComponent<RectTransform>();
            stackCountTransform.SetParent(newSqrGO.transform);
            stackCountTransform.localPosition = Vector2.zero;
            stackCountTransform.sizeDelta = newSqrSprRect.sizeDelta; // reduce, re-use, recycle motherfucker!!!
            stackCount.SetActive(false);
            // End

            // Square, Image component
            Image inventoryBaseSqr = newSqrGO.AddComponent<Image>();
            inventoryBaseSqr.color = new Color(0.65f, 0.65f, 0.65f, 0.89f);

            var rectTransform = newSqrGO.GetComponent<RectTransform>();
            rectTransform.SetParent(parent.transform);
            rectTransform.sizeDelta = new Vector2(32, 32);
            rectTransform.localPosition = new Vector3((36 * x), 32 + (36 * y), 1);
            // End Background Square

            // Square, Inventory Sqr Controller
            var invSqrCon = newSqrGO.AddComponent<InventorySquareController>();
            invSqrCon.InventoryIndex = indexInInv;
            //invSqrCon.WM = WindowManager;
            //invSqrCon.InventoryController = InventoryController;
            invSqrCon.StackCountText = stackCountText;
            //invSqrCon.Init(WindowManager, this, stackCountText);

            newSqrGO.SetActive(true);
            // End

            return newSqrGO;
        }

        // Builds the game objects needed for the inventory.
        GameObject BuildInventory()
        {
            var inventoryWindow = BuildInventoryWindowSlim();

            int sqrIndex = 0;
            for (int y = 0; y < (int)InventoryGridSize.y; y++)
            {
                for (int x = 0; x < (int)InventoryGridSize.x; x++)
                {
                    var invSquare = MakeInventorySquare(inventoryWindow, sqrIndex, x, y);
                    if (invSquare != null)
                    {
                        //InventorySquares.Add(invSquare);
                        sqrIndex++;
                    }
                }
            }

            MakeInformationSquare(inventoryWindow);
            MakeTrashSquare(inventoryWindow);
            MakePlayerPreview(inventoryWindow);

            return inventoryWindow;
        }

        void MakePlayerPreview(GameObject parent)
        {
            GameObject playerPreview = new GameObject("Player Preview");
            RawImage camRender = playerPreview.AddComponent<RawImage>();
            camRender.texture = Resources.Load<Texture>("InvCameraTexture");
            //camRender.raycastTarget = false; // for now
            camRender.uvRect = new Rect(.25f,0f,.5f,1f);

            playerPreview.transform.SetParent(parent.transform);

            var layout = playerPreview.AddComponent<LayoutElement>();
            layout.ignoreLayout = true;

            RectTransform rectTransform = playerPreview.GetComponent<RectTransform>();
            rectTransform.localPosition = new Vector3(-100, 30, 0);
            rectTransform.sizeDelta = new Vector2(100, 200);

        }

        void HandleMouseDrop(ItemDropEventArgs dropEventArgs)
        {
            // TODO
            //int inventoryIndex = dropEventArgs.HeldItem.InventoryIndex;

            /*
            Item = dropEventArgs.HeldItem;
            itemToDrop.InventoryIndex = -1;

            //var sqrController = InventorySquares[inventoryIndex].GetComponent<InventorySquareController>();
            //sqrController.ClearSpace();

            Debug.Log($"Dropped {itemToDrop.Quantity}x {itemToDrop.Name}");
            */           
        }

        void ToggleInventory()
        {
            IsInventoryActive = !IsInventoryActive;
            InventoryWindow.SetActive(IsInventoryActive);


            allowedPress = false;
            keyDelayTimer = new EzTimer(inventoryKeyDelay, () =>
            {
                allowedPress = true;
            });
            keyDelayTimer.Start();
        }

        // Update is called once per frame
        void Update()
        {
            if (keyDelayTimer != null && keyDelayTimer.enabled)
                keyDelayTimer.Tick(Time.deltaTime);

            if (WindowManager != null)
            {
                if (WindowManager.GetMouseManager() != null)
                {
                    if (!DriftedInputManager.IsController)
                    {
                        if (DriftedInputManager.KeyDown(CustomInputManager.GetCurrentMapping().ToggleInventory) && allowedPress && (WindowManager.GetMouseManager().GetHeldItem() == null))
                        {  // Only toggle the inventory if the correct button is pressed, the key timer has elapsed, and there's nothing in the mouse hand.
                            ToggleInventory();
                        }
                    }
                }
            }
        }

        public int HasItemInInventory(Item item)
        {
            return InventoryController.HasItemInInventory(item);
        }

        public int GetNextFreeInventorySpace()
        {
            return InventoryController.GetNextFreeInventorySpace();
        }

        public bool MoveItemInInventory(int oldSlot, int newSlot)
        {
            return InventoryController.MoveItemInInventory(oldSlot, newSlot);
        }


        public void RemoveItemFromInventory(Item item, int index = -1)
        {
            InventoryController.RemoveItemFromInventory(item, index);
        }

        /**
         * End
         */
    }
}