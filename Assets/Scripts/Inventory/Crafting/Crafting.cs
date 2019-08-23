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

using System;
using System.Collections;
using System.Collections.Generic;
using Drifted.Items.Next;
using UnityEditor;
using UnityEngine;

namespace Drifted.Crafting
{
    [CreateAssetMenu(menuName = "Drifted/Crafting/Crafting Recipe")]
    public class Crafting : ScriptableObject
    {
        [SerializeField]
        List<ItemContainer> InputItems;

        [SerializeField]
        ItemContainer OutputItem = new ItemContainer();

        public ItemContainer GetOutput() => OutputItem;
        public List<ItemContainer> GetIngredients() => InputItems;

        public ItemContainer TryCraft(NextGen.Inventory.Inventory inventory)
        {
            ItemContainer result = OutputItem.Copy();

            int ingredientsTotal = InputItems.Count;
            int[] ingredientsIndexes = new int[ingredientsTotal];

            for(int i = 0; i < InputItems.Count; i++)
            {

                int inventoryIndex;
                if ((inventoryIndex = inventory.HasItemAndCount(InputItems[i])) > -1)
                {
                    ingredientsIndexes[i] = inventoryIndex;
                }
                else return null; // Not displaying anything so we can return right when we don't have anything.
                               
            }

            // We don't want to remove anything until we're sure we have everything.
            for(int i = 0; i < InputItems.Count; i++)
            {
                if (inventory.RemoveCountFromIndex(ingredientsIndexes[i], InputItems[i].Quantity) == false) return null;
            }

            return result;
        }
    }

    [CustomEditor(typeof(Crafting))]
    public class CraftingEditor : Editor
    {
        private Texture2D _tex;
        public override Texture2D RenderStaticPreview(string assetPath, UnityEngine.Object[] subAssets, int width, int height)
        {
            Crafting _target = target as Crafting;
            if (_target.GetOutput() == null) return null;
            if (_target.GetOutput().GetItem() == null) return null;
            if (_target.GetOutput().GetItem().Icon == null) return null;
            Texture2D preview = AssetPreview.GetAssetPreview(_target.GetOutput().GetItem());
            if (preview == null) _tex = null;
            else
            {
                _tex = new Texture2D(preview.width, preview.height);
                _tex.SetPixels(preview.GetPixels());
                _tex.Apply();
            }

            return _tex;
        }
    }
}


