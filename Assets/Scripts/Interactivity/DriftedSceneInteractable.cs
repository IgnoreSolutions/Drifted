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
using System.ArrayExtensions;
using System.Collections;
using System.Collections.Generic;
using Drifted;
using Drifted.Interactivity;
using Drifted.UI;
using UnityEngine;
using UnityEngine.Events;

namespace Drifted.Interactivity
{
    public abstract class DriftedSceneInteractable : MonoBehaviour, ISceneInteractable
    {
        CullingGroup cullingGroup;

        [Header("Environment")]
        [SerializeField]
        string FriendlyName = "Nameless Object";

        [Header("New Systems")]
        [SerializeField]
        internal ActivityConsoleManager Console;
        [SerializeField]
        internal NextGen.Inventory.Inventory playerInventory;
        [SerializeField]
        internal SkillController Skills;
        [SerializeField]
        internal MenuManager menuManager;
        [SerializeField]
        internal NotificationManager notificationManager;
        internal bool ShouldBeVisible = false;


        [Header("Culling Group Properties", order = 50)]
        [SerializeField]
        float CullingBoundRadius = 3.0f;
        [SerializeField]
        UnityEvent HasBecomeVisible;
        [SerializeField]
        UnityEvent HasBecomeInvisible;
        [SerializeField]
        float DisappearDelayTime = 1.0f;
        [Space]
        private object lol = null;

        protected virtual void Awake()
        {
            if (cullingGroup == null)
            {
                cullingGroup = new CullingGroup();
            }

            cullingGroup.targetCamera = Camera.main;

            BoundingSphere[] spheres = new BoundingSphere[1000];
            spheres[0] = new BoundingSphere(transform.position, CullingBoundRadius);
            cullingGroup.SetBoundingSpheres(spheres);
            cullingGroup.SetBoundingSphereCount(1);

            cullingGroup.onStateChanged += CullingGroup_OnStateChanged;
        }

        protected virtual void OnDisable()
        {
            if(cullingGroup != null)
            {
                cullingGroup.Dispose();
                cullingGroup = null;
            }
        }

        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, CullingBoundRadius);
        }

        [ReadOnly]
        [SerializeField]
        private bool dAllowCulling = true;
        public void d_SetCullingAllowed(bool allowed)
        {
            dAllowCulling = allowed;
        }

        void CullingGroup_OnStateChanged(CullingGroupEvent groupEvent)
        {
            if(!dAllowCulling) return;

            if(groupEvent.hasBecomeVisible)
            {
                if(HasBecomeVisible != null && HasBecomeVisible.GetPersistentEventCount() > 0)
                {
                    HasBecomeVisible.Invoke();
                    ShouldBeVisible = true;
                }

                //SetMeshRenderers(true);
                StartCoroutine("SetMeshRenderersCoroutine", true);
            }
            else if(groupEvent.hasBecomeInvisible)
            {
                if(HasBecomeInvisible != null && HasBecomeInvisible.GetPersistentEventCount() > 0)
                {
                    HasBecomeInvisible.Invoke();
                    ShouldBeVisible = false;
                }

                //SetMeshRenderers(false);
                StartCoroutine("SetMeshRenderersCoroutine", false);
            }
        }

        private IEnumerator SetMeshRenderersCoroutine(bool shouldBeVisible)
        {
            var meshRenderers = GetComponentsInChildren<MeshRenderer>(false);

            foreach(MeshRenderer mesh in meshRenderers)
            {
                if(mesh == null || mesh.gameObject == null) continue;

                if (mesh.gameObject.layer == LayerDefinitions.DriftedMinimap)
                {
                    continue;
                }
                else
                {
                    mesh.enabled = shouldBeVisible;
                }
                yield return new WaitForSeconds(DisappearDelayTime);
            }
        }

        private void SetMeshRenderers(bool shouldBeVisible)
        {
            var meshRenderers = GetComponentsInChildren<MeshRenderer>(false);

            foreach(MeshRenderer mesh in meshRenderers)
            {
                if (mesh.gameObject.layer == LayerDefinitions.DriftedMinimap)
                {
                    continue;
                }
                else
                {
                    mesh.enabled = shouldBeVisible;
                }
            }
        }

        internal string GetFriendlyName() => FriendlyName;
        public void SetFriendlyName(string name) => FriendlyName = name;

        /// This method is called whenever the object is clicked or interacted with in the scene.
        public virtual void Interact(MonoBehaviour sender)
        {}

        public virtual void OnMouseDown()
        {
            if (DriftedConstants.IsPointerOverUIElement() || CustomInputManager.IsController) return;

            Interact(this);
        }
    }
}