// Copyright 2019 Mike Santiago (admin@ignoresolutions.xyz)

using UnityEditor;
using Drifted.Interactivity;

/*
[CustomPropertyDrawer(typeof(DriftedSceneInteractable))]
public class DriftedSceneInteractablePropertyDrawer : PropertyDrawer
{
    public VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        // Create property container element.
        var container = new VisualElement();

        // Create property fields.
        var consoleField = new PropertyField(property.FindPropertyRelative("consoleManager"));
        var playerInventoryField = new PropertyField(property.FindPropertyRelative("playerInventory"));
        var fancySkillsField = new PropertyField(property.FindPropertyRelative("Skills"), "Fancy skills");
        
        // Add fields to the container.
        container.Add(consoleField);
        container.Add(playerInventoryField);
        container.Add(fancySkillsField);

        return container;
    }
}
*/