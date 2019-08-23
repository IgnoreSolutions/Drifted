/// Copyright (c) Mike Santiago 2019 (admin@ignoresolutions.xyz)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Drifted.Building;
using UnityEngine.UI;
using System;

namespace Drifted.Building
{

public class BuildingContainer : MonoBehaviour
{
    [SerializeField]
    DriftedBuilding representedBuilding;

    [SerializeField]
    Text buildingName;

    public void SetRepresentedBuilding(DriftedBuilding building) => representedBuilding = building;

    void Awake()
    {

    }

    void OnValidate()
    {
        if(buildingName != null) buildingName.text = "Testing testing";

        if(representedBuilding != null && buildingName != null) 
            buildingName.text = $"<b>{representedBuilding.FriendlyName}</b>";
    }

    public DriftedBuilding GetBuilding() => representedBuilding;
}
}