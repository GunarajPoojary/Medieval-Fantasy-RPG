using RPG.Gameplay.Inventories;
using UnityEditor;
using UnityEngine;

namespace RPG.Editor
{
    [CustomEditor(typeof(ItemSO), true)]
    [CanEditMultipleObjects]
    public class ItemGUIDGenerate : UnityEditor.Editor
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
                    if (target is ItemSO itemObject)
                    {
                        itemObject.GenerateGuid();
                        EditorUtility.SetDirty(itemObject); // Mark the object as dirty to save changes
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
