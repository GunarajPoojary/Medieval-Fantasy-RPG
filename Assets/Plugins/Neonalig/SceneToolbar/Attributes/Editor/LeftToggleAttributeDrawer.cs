using System.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace Plugins.Neonalig.SceneToolbar.Attributes {
    [CustomPropertyDrawer(typeof(LeftToggleAttribute))]
    internal sealed class LeftToggleAttributeDrawer : PropertyDrawer {

        #region Overrides of PropertyDrawer

        /// <inheritdoc />
        public override void OnGUI( Rect Pos, SerializedProperty Prop, GUIContent Lbl ) {
            if (Prop.propertyType != SerializedPropertyType.Boolean) {
                EditorGUI.HelpBox(Pos, "[LeftToggle] is only supported on booleans.", MessageType.Error);
                return;
            }
            
            Prop.boolValue = EditorGUI.ToggleLeft(Pos, Lbl, Prop.boolValue);
        }

        #endregion

    }
}
