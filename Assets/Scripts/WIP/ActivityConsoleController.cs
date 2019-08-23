using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActivityConsoleController : MonoBehaviour
{
    [Header("New Stuff")]
    public ActivityConsoleManager consoleManager;

    public float VisibilityTimeout = 5f;
    public Text ActivityLogText;
    public CanvasGroup Group;
    public WMWindowize Windowizer;

    public Scrollbar scrollbar;

    public Color TransparentColor = new Color(0f, 0f, 0f, 0f);
    public Color DisplayColor = new Color(.443f,.443f,.443f, .80f);

    public EzTimer TimeoutTimer;
    private EzTimer InternalLerpTimer;

    private void Awake()
    {
        TimeoutTimer = new EzTimer(VisibilityTimeout, HideConsole, true);
    }

    public void ShowConsole()
    {
        TimeoutTimer.Stop();
        TimeoutTimer.ResetTime();
        fade = false;

        Group.alpha = 1.0f;
        Group.interactable = true;
        Group.blocksRaycasts = true;
        TimeoutTimer.Start();
    }

    public void HideConsole()
    {
        startFade = true;
    }

    public void DelayHide()
    {
        Invoke("HideConsole", 3.5f);
    }

    void HandleFade()
    {
        if (startFade)
        {
            alphaT = 0f;
            startFade = false;
            fade = true;
        }

        if (fade)
        {
            alphaT = Mathf.Clamp01(alphaT + Time.deltaTime);
            Group.alpha = Mathf.Lerp(0, 1f, 1.0f - alphaT);

            if (Group.alpha <= 0.1f)
            {
                Group.alpha = 0f;
                Group.interactable = false;
                Group.blocksRaycasts = false;
                fade = false;
            }
        }
    }

    public void UpdateConsoleText()
    {
        if (consoleManager == null) return;

        ActivityLogText.text = consoleManager.ConsoleText;
        ShowConsole();
    }

    [SerializeField]
    float alphaT;
    [ReadOnly]
    [SerializeField]
    bool startFade = false, fade = false;
    void Update()
    {
        if (TimeoutTimer != null && TimeoutTimer.enabled) TimeoutTimer.Tick(Time.deltaTime);

        HandleFade();
    }

    public void AddLine(string text)
    {

    }
}
