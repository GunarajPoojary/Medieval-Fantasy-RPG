using RPG;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(TabGroup))]
public class TabEditor : Editor
{
    private SerializedProperty _tabsProperty;
    private SerializedProperty _hoverSoundProperty;
    private SerializedProperty _selectSoundProperty;
    private SerializedProperty _audioSourceProperty;
    private SerializedProperty _hoverColorProperty;
    private SerializedProperty _selectedColorProperty;
    private SerializedProperty _defaultColorProperty;
    private ReorderableList _tabList;

    private void OnEnable()
    {
        _tabsProperty = serializedObject.FindProperty("_tabs");
        _hoverSoundProperty = serializedObject.FindProperty("_hoverSound");
        _selectSoundProperty = serializedObject.FindProperty("_selectSound");
        _audioSourceProperty = serializedObject.FindProperty("_audioSource");
        _hoverColorProperty = serializedObject.FindProperty("_hoverColor");
        _selectedColorProperty = serializedObject.FindProperty("_selectedColor");
        _defaultColorProperty = serializedObject.FindProperty("_defaultColor");

        _tabList = new ReorderableList(serializedObject, _tabsProperty, true, true, true, true);

        _tabList.drawHeaderCallback = rect => {
            EditorGUI.LabelField(rect, "Tabs (Button + Content)");
        };

        _tabList.elementHeight = EditorGUIUtility.singleLineHeight + 10;

        _tabList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            SerializedProperty element = _tabsProperty.GetArrayElementAtIndex(index);
            SerializedProperty buttonProp = element.FindPropertyRelative("tabButton");
            SerializedProperty contentProp = element.FindPropertyRelative("tabContent");

            float halfWidth = (rect.width - 10) / 2;

            rect.y += 2;

            // Draw TabButton field
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, halfWidth, EditorGUIUtility.singleLineHeight),
                buttonProp, GUIContent.none);

            // Draw TabContent field
            EditorGUI.PropertyField(
                new Rect(rect.x + halfWidth + 10, rect.y, halfWidth, EditorGUIUtility.singleLineHeight),
                contentProp, GUIContent.none);
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Sound Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_hoverSoundProperty);
        EditorGUILayout.PropertyField(_selectSoundProperty);
        EditorGUILayout.PropertyField(_audioSourceProperty);
        EditorGUILayout.PropertyField(_selectedColorProperty);
        EditorGUILayout.PropertyField(_hoverColorProperty);
        EditorGUILayout.PropertyField(_defaultColorProperty);
        EditorGUILayout.Space();

        _tabList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }
}