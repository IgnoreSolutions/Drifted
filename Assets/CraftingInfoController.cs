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
using Drifted;
using Drifted.Crafting;
using Drifted.Crafting.UI;
using Drifted.Items.Next;
using UnityEngine;
using UnityEngine.UI;

public class CraftingInfoController : MonoBehaviour
{
    [SerializeField]
    Text descriptionText;

    [SerializeField]
    IngredientsController ingredientsController;

    [SerializeField]
    Image spritePreview;

    [SerializeField]
    RenderTexture preview3d;

    [SerializeField]
    Button craftButton;

    private Color Transparent = new Color(0f, 0f, 0f, 0f);

    private void Start()
    {
    }

    public void UpdateInformationText(RecipeContainer container, Drifted.NextGen.Inventory.Inventory inventoryInstance) => UpdateInformationText(container.Recipe, inventoryInstance);

    public void UpdateInformationText(Crafting recipe, Drifted.NextGen.Inventory.Inventory inventoryInstance)
    {
        Item output = recipe.GetOutput().GetItem();

        if (output == null) return;

        descriptionText.text = output.Description;

        spritePreview.sprite = output.Icon;
        spritePreview.color = Color.white;

        //if (preview3d == null) preview3d.SetActive(false);

        if (CanCraft(recipe, inventoryInstance))
        {
            descriptionText.text += "\n\n<color=green>This is craftable!</color>";
            craftButton.interactable = true;
        }
        else
        {
            descriptionText.text += "\n\n<color=red>You don't have all of the ingredients to craft this.</color>";
            craftButton.interactable = false;
        }

        ingredientsController.UpdateIngredientsList(recipe, inventoryInstance);
    }

    public bool CanCraft(Crafting recipe, Drifted.NextGen.Inventory.Inventory inventoryInstance)
    {
        int ingredientTotalCount = recipe.GetIngredients().Count;
        int ingredientsPresent = 0;
        foreach(var item in recipe.GetIngredients())
        {
            int index;

            if ((index = inventoryInstance.HasItemAndCount(item)) > -1)
            {
                Debug.Log($"Has {item.GetItem().ItemName}");
                ingredientsPresent++;
            }
            else Debug.Log($"Doesn't have {item.GetItem().ItemName}");

        }

        return (ingredientsPresent == ingredientTotalCount);
    }
}
