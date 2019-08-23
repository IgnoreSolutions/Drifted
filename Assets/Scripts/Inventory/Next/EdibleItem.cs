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
using Drifted.Items.Next;
using UnityEditor;
using UnityEngine;

namespace Drifted.Items.Next
{
    [CreateAssetMenu(menuName = "Drifted/Items/Edible Item")]
    public class EdibleItem : Item
    {
        public float GoodnessFactor;
        public bool ImmediateHealthGain;

        public bool Eat()
        {
            //DriftedConstants.Instance.Player().EnqueueAction(() => DriftedConstants.Instance.Player().Movement.PlayEatAnimation(true));
            //DriftedConstants.Instance.Player().EnqueueWait(3.0f);
            //DriftedConstants.Instance.Player().EnqueueAction(() => DriftedConstants.Instance.Player().Movement.PlayEatAnimation(false));
            //DriftedConstants.Instance.Player().PlayerMoods.Eat(this);
            // TODO: get this out too.

            return true;
        }
    }

    [CustomEditor(typeof(EdibleItem))]
    public class EdibleFoodEditor : AbsCustomItemEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EdibleItem actionTarget = target as EdibleItem;

            actionTarget.ItemName = EditorGUILayout.TextField("Name", actionTarget.ItemName);
            actionTarget.Description = EditorGUILayout.TextField ("Description", actionTarget.Description);
            actionTarget.Stackable = EditorGUILayout.Toggle("Stackable", actionTarget.Stackable);

            // Complicated stuff
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Icon"), new GUIContent("Sprite Object"));

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
            actionTarget.GoodnessFactor = EditorGUILayout.FloatField("Goodness", actionTarget.GoodnessFactor);
            actionTarget.ImmediateHealthGain = EditorGUILayout.Toggle("Restore Health Immediately", actionTarget.ImmediateHealthGain);

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(actionTarget);
            }
        }
    }
}
