using System;
using UnityEngine;
using Drifted.UI;
using Drifted.UI.WindowManager;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Drifted.Moods;
using Drifted.Inventory;
using System.Collections;
using Drifted.Items.Next;
using UnityEngine.SceneManagement;

namespace Drifted
{
    [Obsolete]
    [Serializable]
    public class PlayerScripts
    {
        public PlayerInventory Inventory;
        public PlayerMovement Movement;
        public MoodsController PlayerMoods;
        public GameObject Marty;

        // Queue Functions
        public void EnqueueWait(float waitTime) => Movement.EnqueueWait(waitTime);
        public void EnqueueAction(Action action) => Movement.EnqueueAction(action);
        public void EnqueueAction(Func<bool> action) => Movement.EnqueueAction(action);
        public void EnqueueAction(IEnumerator action) => Movement.EnqueueAction(action);

        public IEnumerator MovePlayerTo(Transform position) => Movement.MovePlayerTo(position);
        //

        // Inventory Functions
#if OOOOO
        /// <summary>
        /// Checks the player's inventory to verify if a specified item exists.
        /// </summary>
        /// <returns>-1 if it doesn't exist, the index that the item is located at if it does.</returns>
        /// <param name="item">Item.</param>
        public int HasItemInInventory(Item item) => Inventory.Inventory.HasItemInInventory(item);

        /// <summary>
        /// Checks the player's inventory to verify if a specified item and minimum quantity exist.
        /// </summary>
        /// <returns>-1 if the item either doesn't exist or doesn't have the correct quantity.
        /// Otherwise, returns the index that the item is located at. 
        /// </returns>
        /// <param name="item">Item.</param>
        public int HasItemInInventory(ItemContainer item) => Inventory.Inventory.HasItemAndCount(item);

        /// <summary>
        /// Adds the given item and quantity to the player's inventory.
        /// </summary>
        /// <returns>-1 if the inventory is full or unable to add. Otherwise, returns the index the item was placed into.</returns>
        /// <param name="item">Item.</param>
        public int AddItemToInventory(ItemContainer item) => Inventory.Inventory.AddItem(item);

        /// <summary>
        /// Removes the given item
        /// </summary>
        /// <returns><c>true</c>, if item from inventory was removed, <c>false</c> otherwise.</returns>
        /// <param name="item">Item.</param>
        /// <param name="index">An optional index. -1 means it will search for the item.</param>
        public bool RemoveItemFromInventory(Item item, int index = -1) => Inventory.Inventory.RemoveItemFromInventory(item, index);

        public bool RemoveCountFromIndex(int index, int quantity) => Inventory.Inventory.RemoveCountFromIndex(index, quantity);

        /// <summary>
        /// Changes the item and quantity at the given index to the item and quantity provided.
        /// </summary>
        /// <param name="container">Container.</param>
        /// <param name="index">Index.</param>
        public void SetItemAt(ItemContainer container, int index) => Inventory.Inventory.SetItemAt(container, index);
        //
#endif
    }

    [Obsolete]
    [Serializable]
    public class UIScripts
    {
        public DriftedCursorHandler CursorHandler;
        public MikeWindowManager WindowManager;
        public NewPopUpMenuController MenuController;
        public ActivityConsoleController Console;
        public SkillController Skills;
        public CraftingUIController CraftingUI;
    }

    public static class LayerDefinitions
    {
        /// <summary>
        /// Entities layer.
        /// </summary>
        public static int DriftedEntities = 9;

        /// <summary>
        /// Environmental layer. (Plants, trees, grass, etc.)
        /// </summary>
        public static int DriftedEnvironment = 10;

        /// <summary>
        /// The base level layer (eg. the island)
        /// </summary>
        public static int DriftedLevel = 11;

        /// <summary>
        /// The layer containing the player and the camera attached to him.
        /// </summary>
        public static int DriftedPlayer = 12;

        /// <summary>
        /// The layer containing the light that only renders in the inventory screen.
        /// </summary>
        public static int DriftedPlayerLight = 13;

        /// <summary>
        /// Rendering layer for Drifted Minimap icons
        /// </summary>
        public static int DriftedMinimap = 14;
    }

    [Flags]
    public enum DriftedLayers
    {
        /// <summary>
        /// Entities layer.
        /// </summary>
        DriftedEntities = 9,

        /// <summary>
        /// Environmental layer. (Plants, trees, grass, etc.)
        /// </summary>
        DriftedEnvironment = 10,

        /// <summary>
        /// The base level layer (eg. the island)
        /// </summary>
        DriftedLevel = 11,

        /// <summary>
        /// The layer containing the player and the camera attached to him.
        /// </summary>
        DriftedPlayer = 12,

        /// <summary>
        /// The layer containing the light that only renders in the inventory screen.
        /// </summary>
        DriftedPlayerLight = 13,

        /// <summary>
        /// Rendering layer for Drifted Minimap icons
        /// </summary>
        DriftedMinimap = 14
    }

    [Obsolete("Move on")]
    public class DriftedConstants : Singleton<DriftedConstants>
    {
        public bool Setup = false;

        public event OnSceneSetup OnSceneLoaded;
        public delegate void OnSceneSetup();
        [SerializeField]
        UIScripts _UI;

        [SerializeField]
        PlayerScripts _Player;

        [Header("Misc.")]
        public Font DefaultFont;
        //public Rect ScreenRect;
        public CameraFollow Camera;

        public bool UIFocused = false;
        public bool FullScreenUIActive = false;
        [SerializeField]
        private bool m_MenusActive = false;
        //public bool MenusActive => _UI.MenuController.MenusActive;
        public bool MenusActive = false;

        public Vector2 UIDropShadowOffset = new Vector2(7, -7);
        public Color UIDropShadowColor = new Color(0f, 0, 0f, .27f);

        public bool IsGamePaused = false;

        public UIScripts UI() => _UI;
        public PlayerScripts Player() => _Player;

        public void LoadScene(string sceneName)
        {
            Debug.Log("Loading Scene");
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);

            //Debug.Log("Re-grabbing references");
            //SetupSceneManager();
        }
        public static bool IsPointerOverUIElement()
        {
            var eventData = new PointerEventData(EventSystem.current);
            eventData.position = UnityEngine.Input.mousePosition;
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            
            return results.Count > 0;
        }
    }
}