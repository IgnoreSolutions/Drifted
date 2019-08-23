using System;
using Drifted.UI;
using UnityEngine;

namespace Drifted.Interactivity
{
    public interface ISceneInteractable
    {
        /// <summary>
        /// What should happen when the player presses the interact button near 
        /// this object.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="position"></param>
        void Interact(MonoBehaviour sender);
    }
}
