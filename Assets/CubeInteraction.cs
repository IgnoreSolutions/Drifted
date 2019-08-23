using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Drifted;
using Drifted.Interactivity;
using Drifted.UI;

public class CubeInteraction : DriftedSceneInteractable
{
    [SerializeField]
    Light theLight;

    [SerializeField]
    MenuManager menuManager;

    protected override void Awake()
    {
        base.Awake();
    }

    public void SetLight(bool enabled)
    {
        if (theLight != null) theLight.enabled = enabled;
    }

    public override void Interact(MonoBehaviour sender)
    {
        if (theLight == null) return;
        if (menuManager == null) return;
        
        var menuItem = PopUpMenu.MakeMenuItem("Toggle", () => 
        { 
            return (theLight.enabled = !theLight.enabled); 
        }, false);

        menuManager.MakePopUpMenu(gameObject, menuItem);
        //var menu = DriftedConstants.Instance.UI().MenuController.MakePopUpMenu(this, menuItem);

        //menu.AutoShowPopup(gameObject);
    }
}
