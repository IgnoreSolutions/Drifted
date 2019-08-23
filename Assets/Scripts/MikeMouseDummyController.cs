/**

MikeMouseDummyController.cs

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
using System.Collections;
using System.Collections.Generic;
using Drifted;
using Drifted.Extras;
using Drifted.Inventory;
using Drifted.Items.ItemDefinitions;
using Drifted.Items.Next;
using Drifted.UI.WindowManager;
using UnityEngine;
using UnityEngine.UI;

public class MikeMouseDummyController : MonoBehaviour
{
    public Vector3 ImageDrawOffset = Vector3.zero;

    public MikeWindowManager WM;
    public InventoryHandler InventoryController;

    public Image Cursor;

    public Image HeldSprite;
    public Text StackCountText;

    private Color JohnCena = new Color(0f, 0f, 0f, 0f);

    private ItemContainer m_MouseHeldItem = null;

    public delegate void MouseDroppedItemDelegate(ItemDropEventArgs eventArgs);
    public event MouseDroppedItemDelegate MouseDroppedItem;

    private bool initialInitDone = false;

    private void Awake()
    {
        if (HeldSprite == null || StackCountText == null) { }

    }

    public bool IsActive() => Cursor.enabled;

    void Init()
    {
        if (Cursor != null)
        {
            Cursor.color = new Color(0f, 0f, 0f, 0f);
            Cursor.rectTransform.sizeDelta = new Vector2(64, 64f);

            if (CustomInputManager.GetCurrentMapping().IsController)
            {
                Cursor.sprite = Resources.Load<Sprite>("Sprites/Cursor/crosshair");
                HideCrosshair();
            }
            else
            {
                Texture2D cursor = Resources.Load<Texture2D>("Sprites/Cursor/cursor");
                if (PlatformUtilities.GetCurrentPlatform() == Platform.Windows)
                    UnityEngine.Cursor.SetCursor(cursor, Vector2.zero/*new Vector2(32, 32)*/, CursorMode.ForceSoftware); // Fucking windows.
                else
                    UnityEngine.Cursor.SetCursor(cursor, Vector2.zero /*new Vector2(32, 32)*/, CursorMode.Auto);
            }
        }
        initialInitDone = true;
    }

    public void ShowCrosshair()
    {
        Cursor.color = Color.white;
    }

    public void HideCrosshair()
    {
        this.Cursor.color = new Color(0f, 0f, 0f, 0f);
    }

    private void Update()
    {
        if (!initialInitDone) Init();
        if(!CustomInputManager.IsController)
        {
            if(Cursor.gameObject.activeSelf != UnityEngine.Cursor.visible) Cursor.gameObject.SetActive(UnityEngine.Cursor.visible);

            transform.position = Input.mousePosition + ImageDrawOffset;
        }

        transform.SetAsLastSibling();
    }

    public void SetHeldItem(ItemContainer item)
    {
        if (m_MouseHeldItem != null && MouseDroppedItem != null) // Has item and subscriber to event.
        {
            //MouseDroppedItem(new ItemDropEventArgs { HeldItem = m_MouseHeldItem });
        }
        m_MouseHeldItem = item;

        if (HeldSprite == null || StackCountText == null) { Debug.LogWarning("Warning: cursor handler should not be null."); return; }
        if(item == null)
        {
            HeldSprite.sprite = null;
            HeldSprite.color = JohnCena;
            StackCountText.gameObject.SetActive(false);
        }
        else
        {
            HeldSprite.sprite = item.GetItem().Icon;
            HeldSprite.color = Color.white;
            StackCountText.text = item.Quantity.ToString();
            StackCountText.gameObject.SetActive(item.Quantity > 1);
        }
    }

    public ItemContainer GetHeldItem() => m_MouseHeldItem;

    public ItemContainer DropHeldItem()
    {
        if(InventoryController != null)
        {
            var droppedItem = GetHeldItem().Copy();


            HeldSprite.sprite = null;
            HeldSprite.color = JohnCena;

            StackCountText.text = "";
            

            SetHeldItem(null);
            //TODO:
            //InventoryController.RemoveItemFromInventory(droppedItem, droppedItem.InventoryIndex);

            return droppedItem;
        }
        else
        {
            Debug.Log($"InventoryController Null: {InventoryController == null}");
        }

        return null;
    }
}
