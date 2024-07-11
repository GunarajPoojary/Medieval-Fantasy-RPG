using System.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace Plugins.Neonalig.SceneToolbar.Attributes {
    [CustomPropertyDrawer(typeof(SceneDropdownAttribute))]
    internal sealed class SceneDropdownAttributeDrawer : PropertyDrawer {

        #region Overrides of PropertyDrawer

        /// <inheritdoc />
        public override void OnGUI( Rect Pos, SerializedProperty Prop, GUIContent Lbl ) {
            if (Prop.propertyType != SerializedPropertyType.String) {
                EditorGUI.LabelField(Pos, Lbl.text, "Use [SceneDropdown] with strings.");
                return;
            }

            string Path = Prop.stringValue;
            EditorUtilities.DrawSceneFieldWithDropdown(Pos, Lbl, Path, OnSelection);

            void OnSelection( string NewPath ) {
                Prop.stringValue = NewPath;
                Prop.serializedObject.ApplyModifiedProperties();
            }
        }

        #endregion

    }
}
