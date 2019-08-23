using System;
using System.Collections;
using System.Collections.Generic;
using Drifted;
using Drifted.CustomInput;
using Drifted.Input;
using UnityEngine;
using UnityEngine.Events;

public class WMToggleOnKeypress : MonoBehaviour
{
    [SerializeField]
    BoolReference menusActiveRef;

    [SerializeField]
    UnityEvent OnVisible;

    [SerializeField]
    UnityEvent OnInvisible;

    public string Key = RewiredConsts.Action.ToggleInventory.ToString();
    public GameObject Target;
    public CanvasGroup Grouping;

    public bool ControllerOnly = false;
    public bool KeyboardOnly = false;

    public bool Display = false;
    private EzTimer KeyDelayTimer = new EzTimer(0.35f, null, false);
    public bool AllowedToToggle = true;

    public bool AllowWithModernOpen = false;

    private void Awake()
    {
        UpdateDisplay();
        KeyDelayTimer.ChangeAction(() => AllowedToToggle = true);

        menusActiveRef = new BoolReference();
    }

    void UpdateDisplay()
    {
        if (Grouping != null)
        {
            Grouping.alpha = Display ? 1.0f : 0f;
            Grouping.interactable = Display;
            Grouping.blocksRaycasts = Display;
        }
        //if(Target != null) Target.SetActive(Display);

    }

    private void OnValidate()
    {
        UpdateDisplay();
    }

    void ToggleWindow()
    {
        Display = !Display;

        if (Display)
        {
            if (ControllerOnly && DriftedInputManager.IsController) DriftedInputManager.SetUIActive();
            OnVisible?.Invoke();
        }
        else
        {
            if (ControllerOnly && DriftedInputManager.IsController) DriftedInputManager.SetPlayActive();
            OnInvisible?.Invoke();
        }

        AllowedToToggle = false;
        UpdateDisplay();
        KeyDelayTimer.Start();
        //DriftedConstants.Instance.UIFocused = Display;
        //DriftedConstants.Instance.UIFocused = Display;
    }

    void Update()
    {
        if (KeyDelayTimer.enabled) KeyDelayTimer.Tick(Time.deltaTime);
        /*
        if (DriftedConstants.Instance.FullScreenUIActive) return;
        if (AllowWithModernOpen && DriftedConstants.Instance.UIFocused) return;



            */

        if (ControllerOnly && !DriftedInputManager.IsController) return;
        if (KeyboardOnly && DriftedInputManager.IsController) return;

        if(Display && ControllerOnly && DriftedInputManager.IsController)
        {
            if(DriftedInputManager.KeyDown("UICancel") && !menusActiveRef.Value)
            {
                ToggleWindow();
            }
        }
        else
        {
            if (DriftedInputManager.KeyDown(Key) && AllowedToToggle)
            ToggleWindow();
        }
    }
}
