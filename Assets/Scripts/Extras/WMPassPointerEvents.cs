using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class WMPassPointerEvents : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public string OptData;

    public delegate void PointerClickDelegate(PointerEventData eventData, string optionalData);
    public delegate void PointerEnterDelegate(PointerEventData eventData, string optionalData);
    public delegate void PointerExitDelegate(PointerEventData eventData, string optionalData);
    public event PointerClickDelegate OnClick;
    public event PointerEnterDelegate OnCursorEnter;
    public event PointerExitDelegate OnCursorExit;

    public void OnPointerClick(PointerEventData eventData) => OnClick(eventData, OptData);
    public void OnPointerEnter(PointerEventData eventData) => OnCursorEnter(eventData, OptData);
    public void OnPointerExit(PointerEventData eventData) => OnCursorExit(eventData, OptData);
}
