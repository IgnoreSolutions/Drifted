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

using System.Collections;
using System.Collections.Generic;
using Drifted.Items.Next;
using Drifted.Moods;
using Drifted.Moods.MoodDefinitions;
using UnityEngine;

[CreateAssetMenu]
public class MoodsManager : ScriptableObject
{
    [SerializeField]
    private AutoDecayMood[] Moods;

    public float MoodAverage = 0f;

    public ActivityConsoleManager Console;

    // Use this for initialization
    void Start()
    {

    }

    private void OnEnable()
    {
        Moods = new AutoDecayMood[(int)DriftedMoods.DONTUSE_Total];
        Moods[(int)DriftedMoods.Entertainment] = new BasicMood("Entertainment", "");
        Moods[(int)DriftedMoods.Hunger] = new HungerMood();
        Moods[(int)DriftedMoods.Restroom] = new BasicMood("Restroom", "");
        Moods[(int)DriftedMoods.Shower] = new BasicMood("Shower", "");
        Moods[(int)DriftedMoods.Sleep] = new BasicMood("Sleep", "");
        Moods[(int)DriftedMoods.Health] = new HealthMood();
    }

    public AutoDecayMood GetMood(DriftedMoods mood) => Moods[(int)mood];
    public void HealEntity(int amount) => (Moods[(int)DriftedMoods.Health] as HealthMood).Heal(amount);
    public void DamageEntity(int amount) => (Moods[(int)DriftedMoods.Health] as HealthMood).Damage(amount);

    public void Eat(Item item)
    {
        float goodnessFactor = 0;

        if (item is EdibleItem) goodnessFactor = (item as EdibleItem).GoodnessFactor;
        if (item is PlantableFood) goodnessFactor = (item as PlantableFood).GoodnessFactor;

        if (goodnessFactor > 0f)
        {
            HungerMood hunger = Moods[(int)DriftedMoods.Hunger] as HungerMood;
            hunger.Eat(goodnessFactor);
            if(Console != null)Console.AddLine($"Mmmmm, that was yummy!");
        }
    }

    /*
    void Update()
    {
        Debug.Log("Scriptable update");
        int overallMoodTotal = 0;
        if (Moods == null) return;
        for (DriftedMoods mood = 0; mood < DriftedMoods.DONTUSE_Total; mood++)
        {
            AutoDecayMood thisMood = Moods[(int)mood];
            if (mood != DriftedMoods.Health)
            {
                thisMood.Update(Time.deltaTime);
                overallMoodTotal += Mathf.CeilToInt(thisMood.CurrentMoodLevel);
            }
        }
    }
    */
}
