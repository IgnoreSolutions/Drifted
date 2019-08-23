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
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Drifted.Items.Next
{
    [CreateAssetMenu(menuName = "Drifted/Items/Plantable Food")]
    public class PlantableFood : Item
    {
        [SerializeField]
        GameObject SeedlingPrefab;

        public int PlantingExperienceGain;

        public bool IsFood = false;
        public float GoodnessFactor;
        public bool ImmediateHealthRestoration;

        public bool Eat()
        {
            // TODO: get this out
            if (!IsFood) return false;

            DriftedConstants.Instance.Player().EnqueueAction(() => DriftedConstants.Instance.Player().Movement.PlayEatAnimation(true));
            DriftedConstants.Instance.Player().EnqueueWait(3.0f);
            DriftedConstants.Instance.Player().EnqueueAction(() => DriftedConstants.Instance.Player().Movement.PlayEatAnimation(false));
            //DriftedConstants.Instance.Player().PlayerMoods.Eat(this);

            return true;
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if(!Flags.HasFlag(ItemFlags.Food))
            {
                Flags |= ItemFlags.Food;
            }
        }

        private Vector3 GetPlantingPosition()
        {
            // TODO: raycast
            GameObject player = GameObject.FindWithTag("Player");
            if (player == null) Debug.LogError("Couldn't find a player");
            Vector3 plantingPosition = player.transform.position + (player.transform.forward * 2);
            plantingPosition.y += 1.0f;

            int layerMask = (1 << LayerDefinitions.DriftedEnvironment);
            Collider[] hit = Physics.OverlapSphere(plantingPosition, 2.5f, layerMask);

            if (hit.Length > 0)
            {
                int i = 0;
                string collidres = "";
                for(; i < hit.Length; i++)
                {
                    collidres += $"{hit[i].name},";
                }
                return Vector3.zero;
            }

            return plantingPosition;
        }

        public bool Plant()
        {
            if (SeedlingPrefab == null) return false;

            GameObject player = GameObject.FindWithTag("Player");
            GameObject environmentalLayer = GameObject.FindWithTag("Environment");
            if (environmentalLayer == null)
            {
                Debug.LogError("Environmental group not tagged!");
                return false;
            }

            if (player.GetComponent<PlayerMovement>().PlayerIsMoving)
            {
                return false; // Can't plant while moving.
            }

            Vector3 plantPosition = GetPlantingPosition();

            GameObject planted = (GameObject)UnityEngine.Object.Instantiate(SeedlingPrefab);
            planted.tag = "Environment";
            planted.layer = 10;
            planted.transform.SetParent(environmentalLayer.transform);
            planted.transform.position = plantPosition + new Vector3(0, 5f, 0);
            if (planted.GetComponent<SnapToGround>() == null) planted.AddComponent<SnapToGround>();

            if (DriftedConstants.Instance.UI().Skills != null && plantPosition != Vector3.zero)
            {
                //Debug.Log("Experience gain");
                DriftedConstants.Instance.UI().Skills.Farming.GainExp(PlantingExperienceGain);
                return true;
            }
            else
            {
                DriftedConstants.Instance.UI().Console.AddLine("<color=orange>You can't plant here!</color>");
            }

            return false;
        }
    }

    [CustomEditor(typeof(PlantableFood))]
    public class PlantableFoodEditor : AbsCustomItemEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            PlantableFood actionTarget = target as PlantableFood;

            actionTarget.ItemName = EditorGUILayout.TextField("Name", actionTarget.ItemName);
            actionTarget.Description = EditorGUILayout.TextField("Description", actionTarget.Description);
            actionTarget.Stackable = EditorGUILayout.Toggle("Stackable", actionTarget.Stackable);

            // Complicated stuff
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Icon"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("SeedlingPrefab"));

            actionTarget.UseCustomMenu = EditorGUILayout.Toggle("Override menu creation", actionTarget.UseCustomMenu);
            using (var group = new EditorGUILayout.FadeGroupScope(Convert.ToSingle(actionTarget.UseCustomMenu)))
            {
                if (group.visible == true)
                {
                    EditorGUI.indentLevel++;

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("BuildCustomMenuAction"));
                }
            }
            EditorGUILayout.Separator();
            actionTarget.IsFood = EditorGUILayout.Toggle("Is Food", actionTarget.IsFood);
            using(var group = new EditorGUILayout.FadeGroupScope(Convert.ToSingle(actionTarget.IsFood)))
            {
                if(group.visible)
                {
                    EditorGUI.indentLevel++;
                    actionTarget.GoodnessFactor = EditorGUILayout.FloatField("Goodness", actionTarget.GoodnessFactor);
                    actionTarget.ImmediateHealthRestoration = EditorGUILayout.Toggle("Restore Health Immediately", actionTarget.ImmediateHealthRestoration);
                }
            }

            this.serializedObject.ApplyModifiedProperties();
            if(GUI.changed)
            {
                EditorUtility.SetDirty(actionTarget);
            }
        }
    }
}