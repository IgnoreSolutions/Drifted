using UnityEngine;
using System.Collections;
using Drifted.Interactivity;
using Drifted.UI;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Drifted;

[System.Serializable]
public class SerializablePopUpAction
{
    public string name;
    public UnityEvent action;

    public Action UnityEventToAction()
    {
        return () => action.Invoke();
    }

    public Func<bool> UnityEventToFunc()
    {
        return () =>
        {
            action.Invoke();
            return true;
        };
    }
}

public class PopUpMenuOnClick : MonoBehaviour, ISceneInteractable, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    SerializablePopUpAction[] Actions = new SerializablePopUpAction[2];

    void Awake()
    {
    }

    PopUpMenuView BuildMenu()
    {
        List<AbstractMenuItem> menuItems = new List<AbstractMenuItem>();
        for(int i = 0; i < Actions.Length; i++)
        {
            menuItems.Add(PopUpMenu.MakeMenuItem(Actions[i].name, Actions[i].UnityEventToFunc()));
        }

        PopUpMenuView newMenu = DriftedConstants.Instance.UI().MenuController.MakePopUpMenu(this, menuItems.ToArray());

        if (newMenu != null) Debug.Log("Created OK.");

        return newMenu;
    }

    public void Interact(MonoBehaviour sender)
    {
        PopUpMenuView newDisplay = BuildMenu();
        if (newDisplay != null)
        {
//            newDisplay.ShowPopupAtMouse();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    public void OnMouseDown()
    {
        Interact(this);
    }
}
