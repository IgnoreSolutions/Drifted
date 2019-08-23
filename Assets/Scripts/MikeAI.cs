/**

MikeAI.cs

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
using System.Collections.Generic;
using MikeSantiago.Extensions;
using UnityEngine;
/**
* I'm eventually abstracting all the AI stuff so that its easy to extend and implement for different types of mobs.
*/
public class MikeAI
{
    /// <summary>
    /// A literal list of functions. These are all the possible behaviours that the AI backend can execute.
    /// </summary>
    public List<Action> PossibleBehaviors;

    /// <summary>
    /// The index in the list of the CurrentBehaviour we're working with.
    /// </summary>
    internal int CurrentBehaviour = 0;

    /// <summary>
    /// The behaviour change time, in seconds.
    /// </summary>
    public float BehaviourChangeTime = 3.0f;

    /**
        Event that fires when the event changes.
        If no event change happens, this event does not fire. This ensures
        animation sequences run smoothly.
     */
    public event OnBehaviourChange BehaviourChange;
    public delegate void OnBehaviourChange(EventArgs e);
    /**
        Spoiler alert: I hate the way events are declared in C#.
     */

    /**
        Timer object that handles the event changes.
     */
    private EzTimer behaviourChangeTimer;

    /**
        RNG.
     */
    private System.Random BetterRandomGenerator;

    private WeightedRandom<Action> WeightedRNG;

    /// <summary>
    /// Constructs an AI routine based on the Actions given.
    /// For smaller AI logic, you can use lambda functions but if you want
    /// to use their method names as part of logic, I'd suggest defining them
    /// as separate functions so that way the Method.Name is logically determined.
    /// </summary>
    public MikeAI(params Action[] behaviours) /** params is C#'s keyword for variadic arguments. uh, yuck. */
    {
        if(behaviours != null && behaviours.Length > 0) // Make sure behaviours are passed and that null wasn't just passed in instead.
        {
            PossibleBehaviors = new List<Action>(behaviours); // TODO: Is it actually this easy...       
            behaviourChangeTimer = new EzTimer(BehaviourChangeTime, OnTimerComplete);
            BetterRandomGenerator = new System.Random();
            WeightedRNG = new WeightedRandom<Action>(PossibleBehaviors);

            behaviourChangeTimer.Start();
        }
        else
        {
            // Y'all mind if i FLEX
            string message = behaviours != null ? (behaviours.Length <= 0 ? "No behaviours were passed!" : "This shouldn't throw! Wtf Unity.") : "Passed behaviours were null.";
            throw new Exception(message);
        }
    }

    private void OnTimerComplete()
    {
        int behaviourCount = PossibleBehaviors.Count;
        //int newBehaviour = BetterRandomGenerator.Next(0, behaviourCount), oldBehaviour = this.CurrentBehaviour;
        //CurrentBehaviour = newBehaviour;


        //if(newBehaviour != oldBehaviour) BehaviourChange(new EventArgs());
        
        behaviourChangeTimer = new EzTimer(BehaviourChangeTime, OnTimerComplete);
    }

    public void AddBehaviour(Action action, float weight)
    {
        PossibleBehaviors.Add(action);
        WeightedRNG.AddChoice(new RandomChoice<Action>(action, weight));
    }

    /// <summary>
    /// Call this method in your Unity update to make sure everything gets updated properly!!
    /// </summary>
    public void UpdateAI(float dt)
    {
        if(behaviourChangeTimer.enabled)
            behaviourChangeTimer.Tick(dt);

        // Get the current behaviour from the list and then call it like a function! Ez! I'm reffing it for optimization sake.
        var newBehaviour = PossibleBehaviors[CurrentBehaviour];
        ref Action currentBehaviour = ref newBehaviour;
        currentBehaviour();
    }

    /// <summary>
    /// Gets the current behaviour name that's being ran. 
    /// This is NON-LOGICAL for lambda functions.
    /// </summary>
    public string CurrentBehaviourName()
    {
        var currentBehaviour = PossibleBehaviors[CurrentBehaviour];
        return currentBehaviour.Method.Name;
    }

    /// <summary>
    /// Manually set the current behaviour index. Up to you to figure out which one you want!
    /// </summary>
    public void SetCurrentBehaviourIndex(int index) => CurrentBehaviour = index;
}