/// Copyright 2019 Mike Santiago (admin@ignoresolutions.xyz)

using UnityEditor;
using Drifted.Items.Next;
using UnityEngine;

namespace Drifted.Editor
{
    [CustomPropertyDrawer(typeof(ItemContainer), true)]
    public class ItemContainerPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(UnityEngine.Rect position, SerializedProperty property, UnityEngine.GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty item = property.FindPropertyRelative("_item");
            SerializedProperty itemName = property.FindPropertyRelative("_itemName");
            SerializedProperty quantity = property.FindPropertyRelative("Quantity");
            
            string prefixLabel = "Item";

            bool itemIsntNull = false;
            try
            {
                if(label.text.Contains("Element") == false) prefixLabel = label.text;
                else
                {
                    Item ob = item.objectReferenceValue as Item;
                    if(ob != null)
                    { 
                        prefixLabel = ob.ItemName;
                        itemIsntNull = true;
                    }
                }
            }
            catch
            {
                Debug.Log("Oops");
            }

            if(itemIsntNull && quantity.intValue == 0) quantity.intValue = 1;

            label.text = prefixLabel;
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            Rect quantityRect = new Rect(position.x, position.y, position.width * .15f, position.height);
            Rect itemRect = new Rect(position.x + (position.width * .15f), position.y, position.width * .85f, position.height);

            EditorGUI.PropertyField(quantityRect, quantity, GUIContent.none);
            EditorGUI.PropertyField(itemRect, item, GUIContent.none);

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }
    }
}