using System;
using System.Collections;
using System.Collections.Generic;
using Drifted;
using Drifted.Skills;
using Drifted.UI;
using Drifted.UI.WindowManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class SkillsDisplayController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public SkillController.DriftedSkills thisSkill = SkillController.DriftedSkills.WoodCutting;
    public SkillController skillController;
    public float CurrentSkillProgress;
    public Image skillProgress;
    public Text LevelProgressText;
    public bool SimpleLevelText = false;

    [HideInInspector]
    public ExperienceSkill representedSkill;

    private WindowProperties WindowProps;

    private WMTooltip SkillsDisplayTooltip;

    private void Awake()
    {
        //skillController = DriftedConstants.Instance.UI().Skills;
        //representedSkill = skillController.SkillFromEnum(thisSkill);

        //if (representedSkill == null) throw new Exception("This skill is either not implemented yet or invalid!");
    }

    void Start()
    {
        //if (skillController == null) throw new Exception("Please assign a SkillController to this script.");
        if (WindowProps == null)
        {
            if ((WindowProps = GetComponentInParent<WindowProperties>()) == null) Debug.LogWarning("No Window Properties attached.");
        }
        if(SkillsDisplayTooltip == null)
        {
            //SkillsDisplayTooltip = skillController.GetTooltip().GetComponent<WMTooltip>();
        }
    }

    private void OnEnable()
    {

    }

    void UpdateSliders()
    {
        if (representedSkill == null) representedSkill = skillController.SkillFromEnum(thisSkill);
        float currentSkillPerc;
        if (representedSkill.CurrentXP == 0) currentSkillPerc = 0.01f;
        else currentSkillPerc = (float)representedSkill.CurrentXP / (float)representedSkill.ExperienceBase;


        CurrentSkillProgress = currentSkillPerc;

        skillProgress.fillAmount = currentSkillPerc;

        if (LevelProgressText != null)
        {
            if (SimpleLevelText) LevelProgressText.text = $"{representedSkill.Level}";
            else LevelProgressText.text = $"Lvl {representedSkill.Level}: {representedSkill.CurrentExperience()} / {representedSkill.ExperienceBase}";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (skillController == null || skillProgress == null) return; // don't even bother.

        UpdateSliders();
    }

    private string SkillInfoString()
    {
        if (representedSkill == null) return "Invalid";
        return $"{representedSkill.Name}\nLevel: {representedSkill.Level} ({representedSkill.ExperienceBase - representedSkill.ExperienceLeft}/{representedSkill.ExperienceBase})\n\n{representedSkill.Description}";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //SkillsDisplayTooltip.gameObject.SetActive(true);
        //if (SkillsDisplayTooltip.NeedsTextUpdate(SkillInfoString())) { SkillsDisplayTooltip.SetTooltipText(SkillInfoString()); }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //SkillsDisplayTooltip.gameObject.SetActive(false);
    }
}
