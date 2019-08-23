/*
    Copyright (C) 2019 Mike Santiago - All Rights Reserved
    axiom@ignoresolutions.xyz

    Permission to use, copy, modify, and/or distribute this software for any
    purpose with or without fee is hereby granted, provided that the above
    copyright notice and this permission notice appear in all copies.

    THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
    WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
    MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
    ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
    WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
    ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF
    OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
*/

using System;
using UnityEngine;

namespace Drifted.Skills
{
    public interface ISkill
    {
        int Level { get; set; }
        int CurrentXP { get; set; }
        int ExperienceBase { get; set; }
        int ExperienceLeft { get; set; }
        float ExperienceModifier { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        void GainExp(int amount);
        void LevelUp();
        int CurrentExperience();
    }
    [Serializable]
    public abstract class ExperienceSkill
    {
        // These are all also overrideable.
        public int Level = 1;
        public int CurrentXP = 0;
        public int ExperienceBase = 100;
        public int ExperienceLeft = 100;
        public float ExperienceModifier = 1.2f;

        //Every new skill requires these
        public string Name;
        public string Description;

        // These are overrideable
        // So if you want to change the behaviour of any of these functions (most likely the Level up one) you can do so
        public virtual void GainExp(int amount)
        {
            CurrentXP += amount;
            ExperienceLeft -= amount;

            if (ExperienceLeft <= 0) LevelUp();
        }

        public virtual void LevelUp()
        {
            Level++;
            ExperienceBase = (int)Math.Ceiling(ExperienceModifier * ExperienceBase);
            ExperienceModifier *= .4f;
            ExperienceLeft = ExperienceBase;
            CurrentXP = 0;
        }

        public virtual int CurrentExperience() => ExperienceBase - ExperienceLeft;
    }

    [Serializable]
    public class WoodCutting : ExperienceSkill
    {
        public WoodCutting()
        {
            Name = "Wood Cutting";
            Description = "Chop Chop Chop Chop Chop Chop Chop Chop";
        }
    }
    [Serializable]
    public class FishingSkill : ExperienceSkill
    {
        public FishingSkill()
        {
            ExperienceModifier = 1.8f;
            Name = "Fishing";
            Description = "How good are you with poles?";
        }
    }
    [Serializable]
    public class HarvestingSkill : ExperienceSkill
    {
        /*
        public override float ExperienceModifier { get; set; } = 1.4f;

        public override string Name { get; set; } = "Harvesting";
        public override string Description { get; set; } = "Your ability to pick shit up from your surroundings.\nI'd say this is pretty much necessary to survive.";
        */       
    }
    [Serializable]
    public class HuntingSkill : ExperienceSkill
    {
        /*
        public override float ExperienceModifier { get; set; } = 1.9f;
        public override string Name { get; set; } = "Hunting";
        public override string Description { get; set; } = "How well are you able to rip the flesh from the bones of your enemies?";
        */       
    }
    [Serializable]
    public class BuildingSkill : ExperienceSkill
    {
        /*
        public override float ExperienceModifier { get; set; } = 1.34f;
        public override string Name { get; set; } = "Building";
        public override string Description { get; set; } = "Your ability to build competent structures in the environment.";
        */       
    }
    [Serializable]
    public class BlacksmithingSkill : ExperienceSkill
    {
        /*
        public override float ExperienceModifier { get; set; } = 2.8f;
        public override string Name { get; set; } = "Blacksmithing";
        public override string Description { get; set; } = "Your ability to craft weapons to survive in the environment.";
        */       
    }
    [Serializable]
    public class CraftingSkill : ExperienceSkill
    {
        /*
        public override float ExperienceModifier { get; set; } = 1.24f;
        public override string Name { get; set; } = "Crafting";
        public override string Description { get; set; } = "The ability to craft items.";
        */       
    }
    [Serializable]
    public class FarmingSkill : ExperienceSkill
    {
        public override void GainExp(int amount)
        {
            base.GainExp(amount);
        }
        /*
        public override float ExperienceModifier { get; set; } = 2.0f;
        public override string Name { get; set; } = "Farming";
        public override string Description { get; set; } = "Your ability to take care of plants. Affects what you're able to plant and how well they grow.";
        */       
    }
    [Serializable]
    public class FiremakingSkill : ExperienceSkill
    {
        /*
        public override float ExperienceModifier { get; set; } = 1.43f;
        public override string Name { get; set; } = "Fire";
        public override string Description { get; set; } = "Are you a pyromaniac or something?";
        */       
    }
    [Serializable]
    public class RanchingSkill : ExperienceSkill
    {
        /*
        public override float ExperienceModifier { get; set; } = 2.3f;
        public override string Name { get; set; } = "Ranching";
        public override string Description { get; set; } = "They call me ranch cos I be dressin.";
        */       
    }
}