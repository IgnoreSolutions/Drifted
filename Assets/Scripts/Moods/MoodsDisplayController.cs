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

using UnityEngine;
using Drifted.Moods;
using UnityEngine.UI;
using Drifted.Moods.MoodDefinitions;
using System;
using Drifted;

[Serializable]
public struct MoodsProgressBars
{
    public Image HungerMood;
    public Image SleepMood;
    public Image RestroomMood;
    public Image ShowerMood;
    public Image EntertainmentMood;
    public Image HealthMood;

    public Image GetMoodProgressBar(DriftedMoods mood)
    {
        switch(mood)
        {
            case DriftedMoods.Hunger: return HungerMood;
            case DriftedMoods.Sleep: return SleepMood;
            case DriftedMoods.Restroom: return RestroomMood;
            case DriftedMoods.Shower: return ShowerMood;
            case DriftedMoods.Entertainment: return EntertainmentMood;
            case DriftedMoods.Health: return HealthMood;
        }
        return null;
    }
}

public class MoodsDisplayController : MonoBehaviour
{
    public MoodsProgressBars ProgressBars = new MoodsProgressBars();

    [SerializeField]
    private MoodsManager moodController;
    private EzTimer keyDelayTimer;

    private bool Display = false, AllowedToToggle = true;

    // Use this for initialization
    void Start()
    {
        keyDelayTimer = new EzTimer(0.18f, () => { AllowedToToggle = true; }, false);
    }

    private void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (moodController == null) return;

        float overallMoodTotal = 0;
        for (DriftedMoods mood = DriftedMoods.Hunger; mood < DriftedMoods.DONTUSE_Total; mood++)
        {
            AutoDecayMood thisMood = moodController.GetMood(mood);
            if (mood != DriftedMoods.Health) thisMood.Update(Time.deltaTime);
            overallMoodTotal += thisMood.CurrentMoodLevel;
            Image thisProgressBar = ProgressBars.GetMoodProgressBar(mood);

            if (thisMood != null && thisProgressBar != null)
            {
                float moodLevel = (thisMood.CurrentMoodLevel / thisMood.MaxMoodLevel);
                thisProgressBar.fillAmount = moodLevel;

                if (moodLevel > .70f) thisProgressBar.color = Color.green;
                else if (moodLevel < .70f && moodLevel > .30f) thisProgressBar.color = Color.yellow;
                else if (moodLevel < .30f) thisProgressBar.color = Color.red;
            }
        }

        moodController.MoodAverage = (overallMoodTotal / (int)DriftedMoods.DONTUSE_Total);
    }



}
