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
using System.Linq;
using Drifted.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Drifted.Items.Next
{
    public class EnumFlagsAttribute : PropertyAttribute
    {
        public EnumFlagsAttribute() { }
    }

    [Flags]
    public enum ItemFlags
    {
        Food = (1 << 0),
        Tool = (1 << 1),
        Weapon = (1 << 2),
        Armor = (1 << 3)
    }

    [CreateAssetMenu(menuName = "Drifted/Items/Item")]
    [Serializable]
    public class Item : ScriptableObject
    {
        public string ItemName;
        public bool Stackable = true;
        public Sprite Icon;
        public string Description;

        public ItemFlags Flags;

        public event EventHandler QuantityChanged;

        // Overriding what menu options are available for an item, just in case ;)
        public bool UseCustomMenu = false;
        [SerializeField]
        public CustomInventoryMenu BuildCustomMenuAction;

        public Item()
        {
            ItemName = "New Item";
            Description = "It's so new I don't even know what to say about it yet.";
        }

        protected virtual void OnValidate()
        {
            if (Icon == null)
            {
                Icon = Resources.LoadAll<Sprite>("Sprites/items").Single((x) =>
                {
                    if (x.name == "item_unknown") return x;
                    return false;
                });
            }
        }

        //public OverrideMenuAction OverrideDefaultMenu;
        public virtual AbstractMenuItem[] MakeInventoryPopup()
        {
            return null;
            /*
            if (UseCustomMenu)
            {
                return BuildCustomMenuAction.MakeInventoryMenu();
            }

            List<AbstractMenuItem> menuItems = new List<AbstractMenuItem>();
            if (this is UsableItem)
            {
                UsableItem asUsable = (this as UsableItem);
                // TODO: Consume item on use?
                menuItems.Add(PopUpMenu.MakeMenuItem("Use", () =>
                {
                    asUsable.Use();
                    return true;
                }));
            }

            if(this is PlantableFood)
            {
                PlantableFood asPlantable = (this as PlantableFood);
                if (asPlantable.IsFood)
                {
                    menuItems.Add(PopUpMenu.MakeMenuItem("Eat", () =>
                    {
                        if (asPlantable.Eat())
                        {
                            return true;
                        }
                        return false;
                    }));
                }
                menuItems.Add(PopUpMenu.MakeMenuItem("Plant", () => 
                {
                    if (asPlantable.Plant())
                    {
                        return true;
                    }
                    return false;
                }));
            }

            if(this is EdibleItem)
            {
                EdibleItem edible = (this as EdibleItem);
                menuItems.Add(PopUpMenu.MakeMenuItem("Eat", () =>
                {
                    if (edible.Eat())
                    {
                        return true;
                    }
                    return false;
                }));
            }

            return menuItems.ToArray();
        }
        */
        }

        public override bool Equals(object other)
        {
            if(other is Item)
            {
                bool result = ((Item)other).ItemName == ItemName;
                return result;
            }

            return false;
        }

    }

    [Serializable]
    public class OverrideMenuAction : ScriptableObject
    {
        public bool CustomMenu = false;
        public UnityEvent MakeMenuAction = null;
    }
    
    public abstract class CustomInventoryMenu : ScriptableObject
    {
        public abstract AbstractMenuItem[] MakeInventoryMenu();
    }

    public abstract class AbsCustomItemEditor : Editor
    {
        private Texture2D _tex;
        public override Texture2D RenderStaticPreview(string assetPath, UnityEngine.Object[] subAssets, int width, int height)
        {
            Item _target = target as Item;
            if (_target.Icon == null)
            {
                return null;
            }
            Texture2D preview = AssetPreview.GetAssetPreview(_target.Icon);
            if (preview == null) _tex = null;
            else
            {
                _tex = new Texture2D(preview.width, preview.height);
                _tex.SetPixels(preview.GetPixels());
                _tex.Apply();
            }

            return _tex;
        }
    }

    [CustomEditor(typeof(Item))]
    public class CustomItemEditor : AbsCustomItemEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            Item actionTarget = target as Item;

            actionTarget.ItemName = EditorGUILayout.TextField("Name", actionTarget.ItemName);
            actionTarget.Description = EditorGUILayout.TextField("Description", actionTarget.Description);
            actionTarget.Stackable = EditorGUILayout.Toggle("Stackable", actionTarget.Stackable);

            // Complicated stuff
            //this.serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Icon"));

            actionTarget.UseCustomMenu = EditorGUILayout.Toggle("Override menu creation", actionTarget.UseCustomMenu);
            using (var group = new EditorGUILayout.FadeGroupScope(Convert.ToSingle(actionTarget.UseCustomMenu)))
            {
                if (group.visible == true)
                {
                    EditorGUI.indentLevel++;
                    //EditorGUILayout.PrefixLabel("On Make Menu");
                    //this.serializedObject.Update();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("BuildCustomMenuAction"));
                }
            }

            this.serializedObject.ApplyModifiedProperties();
            if (GUI.changed)
            {
                // TODO: ?
                EditorUtility.SetDirty(actionTarget);
            }
        }
    }
}
