using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using Drifted.UI.WindowManager;

// Whatever object this attaches to, it will transform into a WM tooltip.
[RequireComponent(typeof(RectTransform))]
[DisallowMultipleComponent]
public class WMTooltip : MonoBehaviour
{
    public MikeWindowManager WM;
    public Color TooltipBackgroundColor = new Color(0f, 0f, .24f);
    public Color TooltipTextColor = Color.white;

    private Text TooltipText;
    private RectTransform thisRectTransform;

    private bool SetupDone = false;

    public GameObject FollowTarget = null;

    private TextGenerator textGenerator = new TextGenerator();
    private TextGenerationSettings textGenerationSettings = new TextGenerationSettings();

    private void Start()
    {
        if (WM == null) Debug.LogWarning("Please assign a Window Manager");
        else if(!SetupDone) TurnIntoTooltip();

        thisRectTransform = transform.gameObject.GetComponent<RectTransform>();

        textGenerationSettings.font = Resources.Load<Font>("Fonts/game");
    }

    public bool IsLastChild() => transform.GetSiblingIndex() == (transform.childCount - 1);

    public bool NeedsTextUpdate(string text) => !Equals(text, TooltipText.text);

    public void SetTooltipText(string text)
    {
        if(TooltipText == null) { Debug.LogWarning("Tool tip text object null"); return; }
        if (object.Equals(text, TooltipText.text)) return;

        //float newW, newH;
        //newW = textGenerator.GetPreferredWidth(text, textGenerationSettings);
        //newH = textGenerator.GetPreferredWidth(text, textGenerationSettings);
        //thisRectTransform.sizeDelta = new Vector2(newW, newH);

        TooltipText.text = text;
        Canvas.ForceUpdateCanvases();
    }

    private void OnEnable() => transform.SetAsLastSibling(); // Keep on top
    private void Update()
    {
        if(!IsLastChild()) transform.SetAsLastSibling();
    }

    public void TurnIntoTooltip()
    {
        // TODO: fix this
        //var UIMaster = GameObject.FindWithTag("UIMaster");

        //Canvas masterCanvas = UIMaster.GetComponent<Canvas>();
        //transform.SetParent(masterCanvas.transform);
        //Debug.Log("Look at this");
        transform.gameObject.tag = "UITooltip";
        //transform.localPosition = new Vector3(1000f, 1000f);
        

        var layout = transform.gameObject.AddComponent<HorizontalLayoutGroup>();
        if (layout == null) Debug.LogWarning("Layout adding was null");
        else
        {
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            layout.padding = new RectOffset(5, 5, 5, 5);
        }

        var fitter = transform.gameObject.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

        var textGo = new GameObject("Text");
        Text tooltipText = textGo.AddComponent<Text>();
        tooltipText.font = Resources.Load<Font>("Fonts/game");
        tooltipText.color = TooltipTextColor;
        tooltipText.raycastTarget = false;
        tooltipText.verticalOverflow = VerticalWrapMode.Overflow;
        tooltipText.horizontalOverflow = HorizontalWrapMode.Overflow;

        tooltipText.text = "";
        textGo.transform.SetParent(transform);
        //itemName.rectTransform.sizeDelta = new Vector2(120f, 24f);
        tooltipText.rectTransform.localPosition = Vector3.zero;


        Image bgImg = transform.gameObject.AddComponent<Image>();
        //rgb(0, 65, 158)
        bgImg.color = new Color(0, 0.25f, 0.61f, .85f);
        bgImg.transform.localPosition = Vector3.zero;
        bgImg.raycastTarget = false;


        Shadow shadow = transform.gameObject.AddComponent<Shadow>();
        shadow.effectDistance = new Vector2(7, -7);
        shadow.effectColor = new Color(.27f, .27f, .27f, .45f);


        RectTransform rt = transform.gameObject.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(0, 0);
        //rt.sizeDelta = new Vector2(120f, 24f);

        WMStayAtMouse mStayAtMouse = transform.gameObject.AddComponent<WMStayAtMouse>();
        mStayAtMouse.FollowTarget = FollowTarget;


        mStayAtMouse.OffsetFromMouse = new Vector3((rt.rect.width / 2) + 32f,
                                                   -(rt.rect.height / 2) - 20f,
                                                    0);
                                                                                                       
        TooltipText = tooltipText;
        transform.gameObject.SetActive(false);

        SetupDone = true;
    }
}
