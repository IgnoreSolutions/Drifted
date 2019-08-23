// /**
// MoodsController.cs
// Created 3/24/2019 7:39 AM
//
// Copyright (C) 2019 Mike Santiago - All Rights Reserved
// axiom@ignoresolutions.xyz
//
// Permission to use, copy, modify, and/or distribute this software for any
// purpose with or without fee is hereby granted, provided that the above
// copyright notice and this permission notice appear in all copies.
//
// THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
// WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
// MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
// ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
// WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
// ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF
// OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
//
// */
using UnityEngine;
using System.Collections;
using Drifted.Moods.MoodDefinitions;
using System;
using Drifted.Items.Next;

namespace Drifted.Moods
{
    public class HealthMood : AutoDecayMood
    {
        public event EventHandler EntityDied;

        public void SetOverallMood(int moodLevelTotal, int totalMoods) => CurrentMoodLevel = Mathf.Clamp(Mathf.Ceil(moodLevelTotal / totalMoods), 0, 100);
        public override void Update(float deltaTime)
        {}

        public void Damage(int amount)
        {
            this.CurrentMoodLevel = Mathf.Clamp(CurrentMoodLevel - amount, 0, MaxMoodLevel);
            if (CurrentMoodLevel <= 0) EntityDied?.Invoke(this, null);
        }

        public void Heal(int amount)
        {
            this.CurrentMoodLevel = Mathf.Clamp(amount + CurrentMoodLevel, 0, MaxMoodLevel);
        }
    }

    // This attaches to Marty and his is actual moods
    public class MoodsController : MonoBehaviour
    {
#region Unity public bindings
        [ReadOnly]
        public float EntertainmentMood;
        [ReadOnly]
        public float HungerMood;
        [ReadOnly]
        public float RestroomMood;
        [ReadOnly]
        public float ShowerMood;
        [ReadOnly]
        public float SleepMood;
        [ReadOnly]
        public float HealthMood;
#endregion


    }

}