using System.Collections;
using System.Collections.Generic;
using Drifted;
using Drifted.Skills;
using Drifted.UI.WindowManager;
using UnityEngine;
using UnityEngine.UI;

// Eventually this will also handle creating the skills display window.
[CreateAssetMenu]
public class SkillController : ScriptableObject
{
    public enum DriftedSkills
    {
        WoodCutting,
        Harvest,
        Fishing,
        Hunting,
        Ranching,
        FireMaking,
        Farming,
        Crafting,
        Blacksmithing,
        Building
    }

    public bool ShouldResetOnEnable = true;

    public WoodCutting WoodCutting = new WoodCutting();
    public HarvestingSkill Harvesting = new HarvestingSkill();
    public FishingSkill Fishing = new FishingSkill();
    public HuntingSkill Hunting = new HuntingSkill();
    public BuildingSkill Building = new BuildingSkill();
    public BlacksmithingSkill Blacksmithing = new BlacksmithingSkill();
    public CraftingSkill Crafting = new CraftingSkill();
    public FarmingSkill Farming = new FarmingSkill();
    public FiremakingSkill Firemaking = new FiremakingSkill();
    public RanchingSkill Ranching = new RanchingSkill();

    //public GameObject SkillsWindow;

    //private GameObject SkillsDisplayTooltip;
    private void MakeTooltip()
    {
        /*SkillsDisplayTooltip = new GameObject("SkillsTooltip");
        WMTooltip tooltipOptions = SkillsDisplayTooltip.AddComponent<WMTooltip>();

        tooltipOptions.WM = WM;
        tooltipOptions.TurnIntoTooltip();
        */       
    }

    //public GameObject GetTooltip() => SkillsDisplayTooltip;

    public ExperienceSkill SkillFromEnum(DriftedSkills skill)
    {
        switch (skill)
        {
            case DriftedSkills.WoodCutting: return WoodCutting;
            case DriftedSkills.Harvest: return Harvesting;
            case DriftedSkills.Fishing: return Fishing;
            case DriftedSkills.Hunting: return Hunting;
            case DriftedSkills.Blacksmithing: return Blacksmithing;
            case DriftedSkills.Building: return Building;
            case DriftedSkills.Crafting: return Crafting;
            case DriftedSkills.Farming: return Farming;
            case DriftedSkills.FireMaking: return Firemaking;
            case DriftedSkills.Ranching: return Ranching;
        }
        return null;
    }

    private void OnEnable()
    {
        if(ShouldResetOnEnable)
        {
            ResetSkills();
        }
    }

    void ResetSkills()
    {
        WoodCutting = new WoodCutting();
        Harvesting = new HarvestingSkill();
        Fishing = new FishingSkill();
        Building = new BuildingSkill();
        Hunting = new HuntingSkill();
        Blacksmithing = new BlacksmithingSkill();
        Crafting = new CraftingSkill();
        Farming = new FarmingSkill();
        Firemaking = new FiremakingSkill();
        Ranching = new RanchingSkill();
    }

    private void Awake()
    {
        
    }

    public T SkillFromEnum<T>(DriftedSkills skill) where T : ExperienceSkill
    {
        ExperienceSkill thisSkill = null;
        if ((thisSkill = SkillFromEnum(skill)) == null) return default(T);
        return (T)SkillFromEnum(skill);
    }
}