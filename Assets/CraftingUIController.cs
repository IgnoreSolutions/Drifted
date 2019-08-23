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
using Drifted.Input;
using Drifted.UI;
using UnityEngine;

public class CraftingUIController : MonoBehaviour
{
    public Transform CraftableItemsList;

    [SerializeField]
    GameObject recipePrefab;
    
    public bool Displayed = false;

    [SerializeField]
    CraftingInfoController infoController;

    [SerializeField]
    private Crafting SelectedRecipe;

    [SerializeField]
    ActivityConsoleManager Console;

    [SerializeField]
    public Drifted.NextGen.Inventory.Inventory playerInventory;

    private CanvasGroup canvasGroup;
    // Start is called before the first frame update
    void Start()
    {
        if (Displayed) ShowUI();
        else HideUI();
    }

    void Awake()
    {
        if (Application.isPlaying) LoadCraftingList();
        canvasGroup = GetComponent<CanvasGroup>();

        if (Console == null) Console = (ActivityConsoleManager)ScriptableObject.CreateInstance(typeof(ActivityConsoleManager));
    }

    public void ShowUI()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        Displayed = true;

        DriftedInputManager.SetUIActive();
    }

    public void HideUI()
    {
        canvasGroup.alpha = 0.0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        Displayed = false;

        DriftedInputManager.SetPlayActive();
        // Hack to not trigger the pause menu :/
        //Invoke("DisableFocus", 0.25f);
    }

    void DisableFocus()
    {
        //DriftedConstants.Instance.FullScreenUIActive = false;
    }

    public void Update()
    {
        if (Displayed && DriftedInputManager.KeyDown("UICancel")) HideUI();
    }

    void ClearList()
    {
        if(CraftableItemsList == null && Application.isPlaying)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
    }

    public void CraftItem()
    {
        if (SelectedRecipe == null) return;

        //Debug.Log("Requested to craft " + SelectedRecipe.GetOutput().GetItem().ItemName);

        ItemContainer craftedItem = SelectedRecipe.TryCraft(playerInventory);
        if (craftedItem == null) Debug.Log("Crafting result was null");
        else
        {
            playerInventory.AddItem(craftedItem);
            Console.AddLine($"Crafted {craftedItem.Quantity}x {craftedItem.GetItem().ItemName}");
            //DriftedConstants.Instance.Player().AddItemToInventory(craftedItem);
            //DriftedConstants.Instance.UI().Console.AddLine($"Crafted {craftedItem.Quantity}x {craftedItem.GetItem().ItemName}");
        }

        infoController.UpdateInformationText(SelectedRecipe, playerInventory);
    }

    private void LoadCraftingList()
    {
        ClearList();

        // Resources/Definitions/Crafting
        Crafting[] recipes = Resources.LoadAll<Crafting>("Definitions/Crafting");

        if(recipePrefab == null)
        {
            Debug.LogError("Recipe prefab was null!", this);
            return;
        }

        if(CraftableItemsList == null)
        {
            Debug.LogError("No where to put recipes!", this);
            return;
        }

        for(int i = 0; i < recipes.Length; i++)
        {
            var newRecipe = Instantiate(recipePrefab, CraftableItemsList);
            RecipeContainer recipeContainer = newRecipe.GetComponent<RecipeContainer>();
            UIClickController clickHandler = newRecipe.GetComponent<UIClickController>();



            if(recipeContainer == null)
            {
                Debug.LogError("Oh fuck");
                return;
            }

            recipeContainer.SetRecipe(recipes[i]);
            clickHandler.OnSelect.AddListener(() =>
            {
                infoController.UpdateInformationText(recipeContainer, playerInventory);
                SelectedRecipe = recipeContainer.Recipe;

            });

            if(i == 0) clickHandler.gameObject.AddComponent<DefaultSelectable>();
        }
    }
}
