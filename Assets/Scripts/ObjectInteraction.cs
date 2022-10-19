using UnityEditor;
using UnityEngine;
using System;

public enum Reaction
{
    Shake,
    AnimationTrigger,
    Other
}

[Serializable]
public class ObjectInteraction
{
    public PickUpItem pickUpItem;
    public string itemName;
    public Reaction reaction;
    public string animationTrigger;
}


// IngredientDrawer
[CustomPropertyDrawer(typeof(ObjectInteraction))]
public class ObjectInteractionDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 2* base.GetPropertyHeight(property, label);
    }
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        //position.height = 2 * position.height;
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rectss
        float height = position.height/2;
        var itemRect            = new Rect(position.x,                      position.y, position.width * 0.5f, height);
        var nameRect            = new Rect(position.x + position.width / 2, position.y+height, position.width * 0.5f, height);

        var reactionRect        = new Rect(position.x,                      position.y+height, position.width * 0.5f, height);
        var reactionVarRect     = new Rect(position.x + position.width / 2, position.y+height, position.width * 0.5f, height);

        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        int reaction = property.FindPropertyRelative("reaction").intValue;
        EditorGUI.PropertyField(itemRect, property.FindPropertyRelative("pickUpItem"), GUIContent.none);
        if (reaction == (int)Reaction.Other)
        {
            EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("itemName"), GUIContent.none);
        }
        EditorGUI.PropertyField(reactionRect, property.FindPropertyRelative("reaction"), GUIContent.none);

        if (reaction == (int)Reaction.AnimationTrigger)
        {
            EditorGUI.PropertyField(reactionVarRect, property.FindPropertyRelative("animationTrigger"), GUIContent.none);
        }
        

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}