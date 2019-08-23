using System.Collections;
using System.Collections.Generic;
using Drifted;
using Drifted.Interactivity;
using Drifted.UI;
using UnityEngine;

public class NewSceneTest : DriftedSceneInteractable
{
    [ReadOnly]
    public string About = "Testing out some new things.";
    public override void Interact(MonoBehaviour sender)
    {
        base.Interact(sender);

        BuildTestMenu();
    }

    void BuildTestMenu()
    {
        AbstractMenuItem[] items = new AbstractMenuItem[2];
        items[0] = PopUpMenu.MakeMenuItem("Test 1", () => true, false);
        items[1] = PopUpMenu.MakeMenuItem("Test 2", () => true, false);

        var menu = DriftedConstants.Instance.UI().MenuController.MakePopUpMenu(this, items);

        menu.AutoShowPopup();
    }
}
