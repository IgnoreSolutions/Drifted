// /**
// HealthController
// Created 4/8/2019 10:06 PM
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
using System;
using UnityEngine.Events;
using Drifted;
namespace Drifted.Entities
{
    /// <summary>
    /// Attach this to an entity to support in-game
    /// Health bar rendering.
    /// </summary>
    public class HealthController : MonoBehaviour
    {
        public UnityEvent HealthChanged;
        public UnityEvent EntityDied;

        public FloatReference CurrentHealth;
        public float MaxHealth;

        // Detecting
        public float Radius = 1.1f;
        public bool ShowIfFull = false;
        //

        public EntityHealthBar HealthBar;

        public float CurrentHealthNormalized => CurrentHealth.Value / MaxHealth;

        private bool hitPlayer = false;
        private bool RaycastPlayer()
        {
            int layerMask = (1 << LayerDefinitions.DriftedPlayer) | (1 << LayerDefinitions.DriftedEntities);


            Collider[] hit = Physics.OverlapSphere(transform.position, Radius, layerMask);
            hitPlayer = false;

            if (hit.Length > 0)
            {
                for (int i = 0; i < hit.Length; i++)
                {
                    if (hit[i].tag == "Player") { hitPlayer = true; return hitPlayer; }
                }
            }

            return hitPlayer;
        }

        private void OnValidate()
        {
            if(CurrentHealth == null) CurrentHealth = new FloatReference();
            if(HealthBar != null)
                HealthBar.SetFill(CurrentHealthNormalized);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, Radius);
        }

        public void Heal(int amount)
        {
            CurrentHealth.Value = Mathf.Clamp(CurrentHealth.Value + amount, 0, MaxHealth);
            UpdateHealthBar();
            //WorldSpaceHealthbar
            HealthChanged.Invoke();
        }
        public void Damage(int amount)
        {
            CurrentHealth.Value = Mathf.Clamp(CurrentHealth.Value - amount, 0, MaxHealth);
            UpdateHealthBar();
            if (CurrentHealth.Value <= 0) EntityDied.Invoke();
        }

        private void UpdateHealthBar()
        {
            if (HealthBar != null) HealthBar.SetFill(CurrentHealth.Value / MaxHealth);
        }

        bool showing = false;

        private void Update()
        {
            bool showHealthbar = false;
            if (HealthIsFull())
            {
                if (ShowIfFull) showHealthbar = RaycastPlayer();
                else showHealthbar = false;
            }
            else showHealthbar = RaycastPlayer();

            if(showing && showHealthbar) return;
            if (!showing && !showHealthbar) return; // Return if we're already set.

            if (showHealthbar) ShowHealthbar();
            else HideHealthbar();
        }

        private void Start()
        {
            HideHealthbar();
        }

        private void ShowHealthbar()
        {
            HealthBar.SetVisibility(true);
            UpdateHealthBar();
            showing = true;
        }

        private void HideHealthbar()
        {
            HealthBar.SetVisibility(false);
            UpdateHealthBar();
            showing = false;
        }

        private bool HealthIsFull() => MaxHealth - CurrentHealth.Value <= 0.3f;

        // TODO: raycast
    }
}