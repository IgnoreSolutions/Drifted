using System;
using System.Collections;
using System.Collections.Generic;
using Drifted;
using Drifted.Extras;
using Drifted.Input;
using Drifted.Inventory;
using Drifted.Items.ItemDefinitions;
using Drifted.Items.Next;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Drifted.UI
{
    public enum DriftedCursorStyle
    {
        Crosshair,
        Pointer,
        Invisible
    }
    public class DriftedCursorHandler : MonoBehaviour
    {
        // TODO: set this.
        public Drifted.NextGen.Inventory.Inventory InventoryController;
        public Image CursorImage;
        public Image HoldingImage;
        public Text StackCountText;

        private Color Transparent = new Color(0f, 0f, 0f, 0f);

        [SerializeField]
        private DriftedCursorStyle CurrentCursorStyle = DriftedCursorStyle.Pointer;

        public ItemContainer GetHeldItem() => InventoryController.itemInMouse;

        private void Awake()
        {
            CursorImage.sprite = Resources.Load<Sprite>("Sprites/Cursor/crosshair");
            SetupCursor();
            UpdateCursor();
            SetHoldingVisibility(false);
        }

        private void SetupCursor()
        {
            Texture2D cursor = Resources.Load<Texture2D>("Sprites/Cursor/cursor");
            if (PlatformUtilities.GetCurrentPlatform() == Platform.Windows)
                UnityEngine.Cursor.SetCursor(cursor, Vector2.zero/*new Vector2(32, 32)*/, CursorMode.ForceSoftware); // Fucking windows.
            else
                UnityEngine.Cursor.SetCursor(cursor, Vector2.zero /*new Vector2(32, 32)*/, CursorMode.Auto);
        }

        private void UpdateCursor()
        {
            switch(CurrentCursorStyle)
            {
                case DriftedCursorStyle.Pointer:
                    CursorImage.color = Transparent;
                    Cursor.visible = true;
                    break;
                case DriftedCursorStyle.Crosshair:
                    CursorImage.color = Color.white;
                    Cursor.visible = false;
                    break;
                case DriftedCursorStyle.Invisible:
                    CursorImage.color = Transparent;
                    Cursor.visible = false;
                    break;
            }

            transform.SetAsLastSibling();
        }
        public bool CursorVisible;
        public void Update()
        {
            //if (InventoryController == null) InventoryController = DriftedConstants.Instance.Player().Inventory.Inventory;

            if (!DriftedInputManager.IsController)
            {
                transform.position = UnityEngine.Input.mousePosition;
            }
            else
            {
                if (Cursor.visible == true)
                {
                    Cursor.visible = false;
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                }

                if(EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null)
                {
                    transform.position = EventSystem.current.currentSelectedGameObject.transform.position;
                }
            }
        }

        public void SetHoldingVisibility(bool visible)
        {
            if(visible)
            {
                HoldingImage.color = Color.white;
                StackCountText.enabled = true;
            }
            else
            {
                HoldingImage.sprite = null;
                HoldingImage.color = Transparent;
                StackCountText.enabled = false;
            }
        }

        public void UpdateCursorHold()
        {
            if (InventoryController == null) return;

            ItemContainer heldItem = InventoryController.itemInMouse;
            if(heldItem == null)
            {
                HoldingImage.sprite = null;
                HoldingImage.color = Transparent;
                StackCountText.gameObject.SetActive(false);
            }
            else
            {
                HoldingImage.sprite = heldItem.GetItem().Icon;
                HoldingImage.color = Color.white;
                StackCountText.text = heldItem.Quantity.ToString();
                StackCountText.enabled = heldItem.Quantity > 1;
            }
        }


        public void Refresh()
        {
            UpdateCursorHold();
        }

        public void SetCursorStyle(DriftedCursorStyle style)
        {
            CurrentCursorStyle = style;
            UpdateCursor();
        }
        public DriftedCursorStyle GetCursorStyle() => CurrentCursorStyle;

        public void SetPosition(Vector2 position) => transform.position = position;
    }
}