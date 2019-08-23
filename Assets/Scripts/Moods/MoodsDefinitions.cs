// /**
// MoodsDefinition.cs
// Created 3/24/2019 7:49 AM by Mike Santiago
// axiom@ignoresolutions.xyz
// */

using System;
using Drifted.Items.ItemDefinitions;
using UnityEngine;

namespace Drifted.Moods.MoodDefinitions
{
    public enum DriftedMoods
    {
        Hunger,
        Sleep,
        Restroom,
        Shower,
        Entertainment,
        Health,
        DONTUSE_Total
    }

    public interface IMood
    {
        string Name { get; set; }
        string Description { get; set; }

        float CurrentMoodLevel { get; set; }
        float MaxMoodLevel { get; set; }
        float MoodDecayFactor { get; set; }

        void Update(float deltaTime);
    }

    [Serializable]
    public class AutoDecayMood
    {
        public string Name;

        public float CurrentMoodLevel = 100;
        public float MaxMoodLevel = 100;
        public float MoodDecayFactor = 0.01f;
        public string Description;

        private int FramesSinceLow = 0;
        private float LowThreshold = 21.1f;
        private int CriticalMoodTime = 150;

        public virtual void Update(float deltaTime)
        {
            float reduceFactor = MoodDecayFactor * deltaTime;
            float NewMoodLevel = (CurrentMoodLevel + reduceFactor);

            CurrentMoodLevel -= reduceFactor;

            if (CurrentMoodLevel <= LowThreshold) FramesSinceLow++;
            if(FramesSinceLow > 0 && FramesSinceLow >= CriticalMoodTime)
            {
                // TODO: the player's mood has been critical for too long.
                DriftedConstants.Instance.UI().Console.AddLine($"[DEBUG] {Name} has been critical for too long.");
            }
        }
    }

    [Serializable]
    public class HungerMood : AutoDecayMood
    {
        public int FramesSinceLastEaten = 1;

        private EzTimer HungerDecayTimer;

        public HungerMood()
        {
            Name = "Hunger";
            FramesSinceLastEaten = 1;

            HungerDecayTimer = new EzTimer(3.2f, ReduceHunger, true);
        }

        /*
        public override float MoodDecayFactor
        {
            get => FramesSinceLastEaten / MaxMoodLevel;
        }
        */

        private void ReduceHunger()
        {
            float reduceFactor = MoodDecayFactor * Time.deltaTime;
            CurrentMoodLevel = Mathf.Clamp(CurrentMoodLevel - reduceFactor, 0, MaxMoodLevel);

            HungerDecayTimer.Start();
            HungerDecayTimer.enabled = true;
        }

        public override void Update(float deltaTime)
        {
            if (HungerDecayTimer != null && HungerDecayTimer.enabled) HungerDecayTimer.Tick(Time.deltaTime);
            FramesSinceLastEaten++;
        }

        public void Eat(float goodnessFactor, float immediateHealthRestoration = 0)
        {
            float lastEatenFactor = Mathf.Clamp(FramesSinceLastEaten / 4, 0.1f, 4f);
            CurrentMoodLevel += Mathf.Clamp(goodnessFactor * lastEatenFactor, 0, MaxMoodLevel);

            FramesSinceLastEaten = 0;
        }
    }

    [Serializable]
    public sealed class BasicMood : AutoDecayMood
    {
        public BasicMood(string name, string description)
        {
            Name = name; Description = description;
        }
    }

    // YET TODO: actual mood definitions and how they'll decay over time
}