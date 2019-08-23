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
using UnityEditor;
using UnityEngine;

namespace Drifted.Items.Next
{
    [CreateAssetMenu(menuName = "Drifted/Items/Cookable Item")]
    public class CookableItem : Item
    {
        public Item CookResult;
        public float CookTime;

        // TODO: Should it spoil?? Raw meat definitely should.
    }

    [CustomEditor(typeof(CookableItem))]
    public class CookableItemEditor : AbsCustomItemEditor
    {

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            CookableItem actionTarget = target as CookableItem;

            actionTarget.ItemName = EditorGUILayout.TextField("Name", actionTarget.ItemName);
            actionTarget.Description = EditorGUILayout.TextField("Description", actionTarget.Description, GUILayout.Height(45f));
            actionTarget.Stackable = EditorGUILayout.Toggle("Stackable", actionTarget.Stackable);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Icon"), new GUIContent("Sprite Object"));

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("CookResult"));
            actionTarget.CookTime = EditorGUILayout.FloatField("Cook Time", actionTarget.CookTime);

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(actionTarget);
            }
        }
    }
}
