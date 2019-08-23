using System;
using System.Collections;
using System.Collections.Generic;
using Drifted.UI;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attach this to a UI element in the inspector and the WM will recognize it as a window.
/// </summary>
[DisallowMultipleComponent]
public class WMWindowize : MonoBehaviour
{
    public string WindowManagerName = "Custom window";
    public bool Moveable = true;
    public bool NeedsCloseButton = true;
    public bool DestroyOnClose = false;
    public bool NeedsShadow = true;

    private bool IgnoresLayout = true;

    public bool HiddenByDefault = false;

    [SerializeField]
    private Color WindowBGColor = Color.gray;
    private void Awake()
    {
        if(transform.gameObject.GetComponent<GridLayout>() != null) IgnoresLayout = true;
        WindowProperties props = null;
        if ((props = GetComponent<WindowProperties>()) == null) props = this.gameObject.AddComponent<WindowProperties>();
        props.WindowTitle = WindowManagerName;
        props.SetTextColor(Color.white);
        props.SetWindowColor(WindowBGColor);
        props.Moveable = this.Moveable;
        props.NeedsCloseButton = NeedsCloseButton;
        props.DestroyOnClose = DestroyOnClose;
        props.IgnoreLayout = IgnoresLayout;
        props.HiddenByDefault = HiddenByDefault;

        if (NeedsCloseButton)
        {
            RectTransform windowTransform = transform.gameObject.GetComponent<RectTransform>();
            if (windowTransform == null) throw new Exception("This component needs a RectTransform to work");
            props.MakeTitlebar(windowTransform.sizeDelta.x, windowTransform.sizeDelta.y);
        }

        if (NeedsShadow)
        {
            Shadow dropShadow;
            if ((dropShadow = GetComponent<Shadow>()) == null) dropShadow = transform.gameObject.AddComponent<Shadow>();
            dropShadow.effectColor = new Color(.27f, .27f, .27f, .45f);
            dropShadow.effectDistance = new Vector2(7, -7);
        }
    }
}
