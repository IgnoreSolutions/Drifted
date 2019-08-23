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

using System.Collections;
using System.Collections.Generic;
using Drifted.Crafting;
using UnityEngine;
using Drifted.NextGen.Inventory;

namespace Drifted.Crafting.UI
{
    /// <summary>
    /// Behaviour that handles the display and management of the ingredients shown on the crafting menu.
    /// </summary>
    public class IngredientsController : MonoBehaviour
    {
        [SerializeField]
        GameObject ingredientSquarePrefab;

        [SerializeField]
        private IngredientSquareController[] ingredientSquares;

        void ClearIngredients()
        {
            if (ingredientSquares == null) return;
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        public void UpdateIngredientsList(Crafting recipe, NextGen.Inventory.Inventory inventory)
        {
            ClearIngredients();
            var ingredients = recipe.GetIngredients();

            ingredientSquares = new IngredientSquareController[ingredients.Count];

            for (int i = 0; i < ingredientSquares.Length; i++)
            {
                var square = Instantiate(ingredientSquarePrefab, transform);
                if (square == null)
                {
                    Debug.LogError("Couldn't instantiate ingredient square.");
                    return;
                }
                IngredientSquareController controller = square.GetComponent<IngredientSquareController>();
                controller.inventoryInstance = inventory;
                controller.SetHeldItem(ingredients[i], inventory);
                ingredientSquares[i] = controller;
            }
        }
    }

}