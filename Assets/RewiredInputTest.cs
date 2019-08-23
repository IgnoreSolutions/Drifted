using System.Collections;
using System.Collections.Generic;
using Drifted;
using Drifted.Input;
using UnityEngine;

public class RewiredInputTest : MonoBehaviour
{
    public bool toggleInventory = false;
    public float camPanX = 0;
    public float camPanY = 0;

    // Update is called once per frame
    void Update()
    {
        toggleInventory = DriftedInputManager.GetKey(CustomInputManager.GetCurrentMapping().ToggleInventory);
        camPanX = DriftedInputManager.GetAxis(CustomInputManager.GetCurrentMapping().CameraPanX);
        camPanY = DriftedInputManager.GetAxis(CustomInputManager.GetCurrentMapping().CameraPanY);
    }
}
