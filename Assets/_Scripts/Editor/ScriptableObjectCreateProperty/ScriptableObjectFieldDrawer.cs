using UnityEngine;
using UnityEditor;

// Property drawer for ScriptableObject fields with creation button
[CustomPropertyDrawer(typeof(CreateScriptableObjectAttribute))]
public class ScriptableObjectFieldDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Ensure we're dealing with an object reference
        if (property.propertyType != SerializedPropertyType.ObjectReference)
        {
            EditorGUI.LabelField(position, label.text, "Use [ScriptableObjectField] with Object references only.");
            return;
        }

        // Check if we need to show the create button
        bool shouldShowCreateButton = property.objectReferenceValue == null && IsScriptableObjectType(property);
        
        if (shouldShowCreateButton)
        {
            // Calculate positions for the property field and button
            float buttonWidth = 60f;
            float spacing = 5f;
            
            Rect propertyRect = new Rect(position.x, position.y, position.width - buttonWidth - spacing, position.height);
            Rect buttonRect = new Rect(position.x + position.width - buttonWidth, position.y, buttonWidth, position.height);

            // Draw the original property field
            EditorGUI.PropertyField(propertyRect, property, label);

            // Draw the create button
            if (GUI.Button(buttonRect, "Create"))
            {
                ScriptableObjectCreationWindow.ShowWindow(property, GetScriptableObjectType(property));
            }
        }
        else
        {
            // Use the full width for the property field when no button is needed
            EditorGUI.PropertyField(position, property, label);
        }
    }

    private bool IsScriptableObjectType(SerializedProperty property)
    {
        System.Type fieldType = GetScriptableObjectType(property);
        return fieldType != null && (fieldType == typeof(ScriptableObject) || fieldType.IsSubclassOf(typeof(ScriptableObject)));
    }

    private System.Type GetScriptableObjectType(SerializedProperty property)
    {
        // Get the type of the field
        string[] pathSegments = property.propertyPath.Split('.');
        System.Type targetType = property.serializedObject.targetObject.GetType();
        
        foreach (string segment in pathSegments)
        {
            var field = targetType.GetField(segment, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                targetType = field.FieldType;
            }
        }
        
        return targetType;
    }
}