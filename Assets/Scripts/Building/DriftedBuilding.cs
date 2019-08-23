/// Copyright (c) 2019 Mike Santiago (admin@ignoresolutions.xyz)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Drifted.NextGen.Inventory;

namespace Drifted.Building
{
    [CreateAssetMenu(menuName = "Drifted/Building Template")]
    public class DriftedBuilding : ScriptableObject
    {
        [Header("Name")]
        public string FriendlyName = "Unknown Building";

        [Header("Design")]
        [SerializeField]
        GameObject BuildingPrefab;

        [SerializeField]
        GameObject BuildingPreview;

        [Header("Materials")]
        [SerializeField]
        ItemContainer[] RequiredMaterials;

        public GameObject GetPrefab() => BuildingPrefab;
        public GameObject GetPreview() => BuildingPreview;

        // TODO: methods to look for required materials and such

        public bool CanBuild(NextGen.Inventory.Inventory inventoryInstance)
        {
            if(RequiredMaterials.Length == 0) return true;

            bool hasAllIngredients = false;
            int[] ingredientsIndexes = new int[RequiredMaterials.Length];
            for(int i = 0; i < RequiredMaterials.Length; i++)
            {
                int index = inventoryInstance.HasItemAndCount(RequiredMaterials[i]);
                if(index >= 0)
                { 
                    ingredientsIndexes[i] = index;
                    hasAllIngredients = true;
                }
                else hasAllIngredients = false;
            }

            return hasAllIngredients;
        }

        private bool RemoveIngredientsFrom(NextGen.Inventory.Inventory inventory)
        {
            bool success = true;
            for(int i = 0; i < RequiredMaterials.Length; i++)
            {
                int index = inventory.HasItemAndCount(RequiredMaterials[i]);
                if(index >= 0)
                {
                    if(!inventory.RemoveCountFromIndex(index, RequiredMaterials[i].Quantity)) success = false;
                } else success = false;
            }

            return success;
        }

        public bool TryBuild(NextGen.Inventory.Inventory inventoryInstance, 
                            Vector3 position, 
                            Quaternion rotation, 
                            Transform parent)
        {
            if(CanBuild(inventoryInstance))
            {
                bool ingredientsRemoved = RemoveIngredientsFrom(inventoryInstance);
                if(ingredientsRemoved)
                {
                    GameObject newBuilding = Instantiate(BuildingPrefab, position, rotation);
                    newBuilding.transform.SetParent(parent);
                    return true;
                }
            }

            return false;
        }
    }
}