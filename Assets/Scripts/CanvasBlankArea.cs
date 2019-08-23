using System;
using Drifted;
using Drifted.UI.WindowManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class CanvasBlankArea : MonoBehaviour, IPointerClickHandler
{
    public MikeWindowManager WM;

    private Image placeHolder;

    public delegate void BlankAreaClickedDelegate(PointerEventData eventData);
    public event BlankAreaClickedDelegate BlankClicked;

    public void Start()
    {
        placeHolder = GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(WM != null)
        {
            BlankClicked?.Invoke(eventData);

            DriftedConstants.Instance.UI().MenuController.CloseAll();
            gameObject.SetActive(false);
            /*
            InventoryItem itemToDrop = WM.GetMouseManager().GetHeldItem();
            if(itemToDrop != null)
            {
                WM.GetMouseManager().SetHeldItem(null);
                WM.SetMouseFollowSprite(null);
                transform.gameObject.SetActive(false);
            }
            */
        }
        else
        {
            throw new Exception("WM can't be null.");
        }
    }
}
