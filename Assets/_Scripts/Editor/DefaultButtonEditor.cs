using RPG;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(DefaultButton))]
public class DefaultButtonEditor : ButtonEditor
{
    SerializedProperty _pressedScale;
    SerializedProperty _animationDuration;

    protected override void OnEnable()
    {
        base.OnEnable();

        _pressedScale = serializedObject.FindProperty("_pressedScale");
        _animationDuration = serializedObject.FindProperty("_animationDuration");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Animation Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_pressedScale, new GUIContent("Pressed Scale"));
        EditorGUILayout.PropertyField(_animationDuration, new GUIContent("Animation Duration"));

        serializedObject.ApplyModifiedProperties();
    }
}