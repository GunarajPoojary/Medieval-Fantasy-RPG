using GunarajCode.ScriptableObjects;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemObject), true)]
[CanEditMultipleObjects]
public class ItemObjectGUIGenerate : Editor
{
    SerializedProperty idProperty;

    private void OnEnable()
    {
        idProperty = serializedObject.FindProperty("ID");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawPropertiesExcluding(serializedObject, "ID");

        EditorGUILayout.PropertyField(idProperty);

        if (GUILayout.Button("Generate GUID"))
        {
            foreach (var target in targets)
            {
                if (target is ItemObject itemObject)
                {
                    itemObject.GenerateGuid();
                    EditorUtility.SetDirty(itemObject); // Mark the object as dirty to save changes
                }
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
