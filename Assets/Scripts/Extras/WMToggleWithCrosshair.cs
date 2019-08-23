using System;
using System.Collections;
using System.Collections.Generic;
using Drifted;
using Drifted.CustomInput;
using Drifted.Input;
using Drifted.UI.WindowManager;
using UnityEngine;

[DisallowMultipleComponent]
public class WMToggleWithCrosshair : MonoBehaviour
{
    public string Key = "ToggleInventory";
    public GameObject Target;
    public CanvasGroup Grouping;
    public MikeWindowManager WM;

    public bool ControllerOnly = false;
    public bool KeyboardOnly = false;
    public bool IgnoreUI = false;

    public bool Display = false;
    private EzTimer KeyDelayTimer = new EzTimer(0.35f, null, false);
    public bool AllowedToToggle = true;

    private void Awake()
    {
        UpdateDisplay();
        KeyDelayTimer.ChangeAction(() => AllowedToToggle = true);
    }

    void UpdateDisplay()
    {
        Target.SetActive(Display);
        if (Grouping != null) Grouping.alpha = Display ? 1.0f : 0.0f;
    }

    void ToggleWindow()
    {
        Display = !Display;
        AllowedToToggle = false;
        UpdateDisplay();
        //Target.SetActive(Display);
        KeyDelayTimer.Start();

        if (WM != null)
        {
            if (Display) WM.GetMouseManager().ShowCrosshair();
            else WM.GetMouseManager().HideCrosshair();
        }
    }

    void Update()
    {
        if (KeyDelayTimer.enabled) KeyDelayTimer.Tick(Time.deltaTime);

        if (ControllerOnly && !DriftedInputManager.IsController) return;
        if (KeyboardOnly && DriftedInputManager.IsController) return;

//        bool uiIsActive = DriftedConstants.Instance.UIFocused & DriftedConstants.Instance.MenusActive;
  //      if (IgnoreUI) uiIsActive = false;

        if (DriftedInputManager.KeyDown(Key) && AllowedToToggle)
            ToggleWindow();
    }
}
