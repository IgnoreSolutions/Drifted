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
using Drifted.Building;
using UnityEngine;
using Drifted.UI;

public class BuildingPopulator : MonoBehaviour
{
    public GameObject BuildingDisplayPrefab;

    public Transform BuildingScrollList;

    [SerializeField]
    BuildCursorMovement CursorPreview;

    // Start is called before the first frame update
    void Start()
    {
        LoadBuildingsList();
    }

    void ClearList()
    {
        if(BuildingScrollList == null) return;
        for(int i = (BuildingScrollList.childCount - 1); i >= 0; i--)
        {
            Destroy(BuildingScrollList.GetChild(i));
        }
    }

    private void LoadBuildingsList()
    {
        ClearList();

        DriftedBuilding[] buildings = Resources.LoadAll<DriftedBuilding>("Definitions/Buildings");

        if(BuildingDisplayPrefab == null)
        {
            Debug.LogError("Recipe prefab was null!", this);
            return;
        }

        if(BuildingScrollList == null)
        {
            Debug.LogError("No where to put recipes!", this);
            return;
        }

        for(int i = 0; i < buildings.Length; i++)
        {
            var newRecipe = Instantiate(BuildingDisplayPrefab, this.BuildingScrollList);
            BuildingContainer recipeContainer = newRecipe.GetComponent<BuildingContainer>();
            UIClickController clickHandler = newRecipe.GetComponent<UIClickController>();

            if(recipeContainer == null)
            {
                Debug.LogError("Oh fuck");
                return;
            }

            recipeContainer.SetRepresentedBuilding(buildings[i]);
            clickHandler.OnSelect.AddListener(() =>
            {
                if(CursorPreview != null)
                {
                    CursorPreview.SetBuildPrefab(recipeContainer.GetBuilding().GetPrefab());
                }
                //infoController.UpdateInformationText(recipeContainer, playerInventory);
                //SelectedRecipe = recipeContainer.Recipe;
                Debug.Log("Selected " + recipeContainer.GetBuilding().FriendlyName);
            });

            if(i == 0) clickHandler.gameObject.AddComponent<DefaultSelectable>();
        }
    }
}
