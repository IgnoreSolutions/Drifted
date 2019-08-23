// /**
// WeightedRandom.cs
// Created 3/24/2019 5:51 PM
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
using System;
using System.Collections.Generic;

namespace MikeSantiago.Extensions
{
    public class RandomChoice<T>
    {
        public T Choice;
        public float Weight;

        public RandomChoice(T choice, float w)
        {
            Choice = choice;
            Weight = w;
        }
    }

    public class WeightedRandom<T>
    {
        private List<RandomChoice<T>> Choices = new List<RandomChoice<T>>();
        private Random rng = new Random((int)DateTime.Now.Ticks);

        // TODO: Can i pre-calculate total weights so I don't have to iterate twice to do this?

        public WeightedRandom(params RandomChoice<T>[] choices)
        {
            Choices.AddRange(choices);
        }

        public WeightedRandom(List<T> choices) => choices.ForEach((x) => Choices.Add(new RandomChoice<T>(x, 1.0f)));

        public void AddChoice(RandomChoice<T> choice) => Choices.Add(choice);

        public void AddChoices(params RandomChoice<T>[] choices) => Choices.AddRange(choices);

        public void ClearChoices() => Choices.Clear();

        private float GetTotalWeights()
        {
            float result = 0f;
            Choices.ForEach((x) => result += x.Weight);
            return result;
        }

        public RandomChoice<T> GetRandomChoice()
        {
            float totalWeights = GetTotalWeights();
            int randomChoice = rng.Next(0, (int)Math.Ceiling(totalWeights));

            RandomChoice<T> selectedChoice = null;
            foreach(var choice in Choices)
            {
                if(randomChoice < choice.Weight)
                {
                    selectedChoice = choice;
                    break;
                }

                randomChoice = randomChoice - (int)choice.Weight;
            }

            return selectedChoice;
        }

    }
}
