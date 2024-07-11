using System.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace Plugins.Neonalig.SceneToolbar.Attributes {
    [CustomPropertyDrawer(typeof(EnumToolbarAttribute))]
    internal sealed class EnumToolbarAttributeDrawer : PropertyDrawer {

        #region Overrides of PropertyDrawer

        /// <inheritdoc />
        public override bool CanCacheInspectorGUI( SerializedProperty Property ) => true;

        /// <inheritdoc />
        public override void OnGUI( Rect Pos, SerializedProperty Prop, GUIContent Lbl ) {
            Lbl = EditorGUI.BeginProperty(Pos, Lbl, Prop);
            Pos = EditorGUI.PrefixLabel(Pos, Lbl);
            EditorGUI.BeginChangeCheck();
            int NewValue = GUI.Toolbar(Pos, Prop.enumValueIndex, Prop.enumDisplayNames);
            if (EditorGUI.EndChangeCheck()) {
                Prop.enumValueIndex = NewValue;
            }
            EditorGUI.EndProperty();
        }

        #endregion

    }
}