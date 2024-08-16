using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

// ReSharper disable once RedundantNullableDirective
#nullable enable

namespace Plugins.Neonalig.SceneToolbar.Attributes {
    [CustomPropertyDrawer(typeof(SceneToolbarSettings.SceneTagCollection))]
    internal sealed class SceneTagCollectionDrawer : PropertyDrawer {

        /// <inheritdoc />
        public override float GetPropertyHeight( SerializedProperty Prop, GUIContent Lbl ) => base.GetPropertyHeight(Prop, Lbl);

        #region Overrides of PropertyDrawer

        /// <inheritdoc />
        public override void OnGUI( Rect Pos, SerializedProperty Prop, GUIContent Lbl ) {
            Lbl = EditorGUIUtility.TrTextContent(Lbl.text, Lbl.tooltip, "FilterByLabel");
            
            // Iterate properties as usual, but if we reach _Tags, we'll draw it differently.
            SerializedProperty Iterator      = Prop.Copy();
            SerializedProperty End           = Prop.GetEndProperty();
            bool               EnterChildren = true;
            while (Iterator.NextVisible(EnterChildren) && !SerializedProperty.EqualContents(Iterator, End)) {
                if (Iterator.name == "m_Script") { continue; }
                EnterChildren = false;
                if (Iterator.name == "_Tags") {
                    DrawTags(ref Pos, Iterator, Lbl);
                } else {
                    EditorGUI.PropertyField(Pos, Iterator, true);
                }
                Pos.y += EditorGUI.GetPropertyHeight(Iterator, true);
            }
        }

        #endregion

        static void DrawTags( ref Rect Pos, SerializedProperty Prop, GUIContent Lbl ) {
            string PropPath = Prop.propertyPath; // Store the property path, to re-fetch the property later.

            string[] Tags = IterateStrings(Prop).ToArray();
            EditorUtilities.DrawChipBar(Pos, Lbl, Tags, OnRemove, OnAdd, GetAvailableValues, GetLabel, GetMenu);

            void OnRemove( string Tag, int Idx ) {
                SerializedProperty? FreshProp = GetRefreshedProperty(Prop.serializedObject, PropPath);
                if (FreshProp != null) {
                    FreshProp.serializedObject.Update();

                    // Decrease the size of the array
                    FreshProp.DeleteArrayElementAtIndex(Idx);

                    FreshProp.serializedObject.ApplyModifiedProperties();
                    FreshProp.serializedObject.Update();
                } else {
                    Debug.LogError($"Failed to remove tag \"{Tag}\" from property \"{PropPath}\" (could not refresh property).");
                }
            }
            void OnAdd( string Tag ) {
                SerializedProperty? FreshProp = GetRefreshedProperty(Prop.serializedObject, PropPath);
                if (FreshProp != null) {
                    FreshProp.serializedObject.Update();

                    // Increase the size of the array
                    FreshProp.arraySize++;
                    int NewIndex = FreshProp.arraySize - 1;

                    // Set the value of the new element
                    SerializedProperty NewElement = FreshProp.GetArrayElementAtIndex(NewIndex);
                    NewElement.stringValue = Tag;

                    FreshProp.serializedObject.ApplyModifiedProperties();
                    FreshProp.serializedObject.Update();
                } else {
                    Debug.LogError($"Failed to add tag \"{Tag}\" to property \"{PropPath}\" (could not refresh property).");
                }
            }
            IEnumerable<string> GetAvailableValues()   => SceneToolbarSettings.Tags.Except(Tags);
            GUIContent          GetLabel( string Tag ) => EditorGUIUtility.TrTextContent(Tag);
            GenericMenu GetMenu() {
                GenericMenu Menu = new();
                Menu.AddItem(EditorGUIUtility.TrTextContent("New Tag"), false, BeginTagCreation);
                if (GetAvailableValues().Any()) {
                    Menu.AddSeparator(string.Empty);
                }
                return Menu;
            }

            void BeginTagCreation() {
                TagCreatorWindow.Show(OnCreate, Tags);
                void OnCreate( string Tag ) => OnAdd(Tag);
            }
        }

        static SerializedProperty? GetRefreshedProperty( SerializedObject SerializedObject, string PropertyPath ) =>
            SerializedObject.FindProperty(PropertyPath);

        static IEnumerable<string> IterateStrings( SerializedProperty Prop ) {
            Debug.Assert(Prop.isArray, $"{Prop.name} ({Prop.propertyPath}) is not an array; actual type: {Prop.propertyType}");
            SerializedProperty Iterator = Prop.Copy();
            int                Ln       = Iterator.arraySize;
            
            for (int Idx = 0; Idx < Ln; Idx++) {
                SerializedProperty Element = Iterator.GetArrayElementAtIndex(Idx);
                if (Element.propertyType != SerializedPropertyType.String) { continue; }
                yield return Element.stringValue;
            }
        }

    }

    [Icon(_IconName)]
    internal sealed class TagCreatorWindow : EditorWindow {
        const string _IconName = "FilterByLabel";

        string[] _CurrentTags = Array.Empty<string>();
        
        string         _Tag      = string.Empty;
        Action<string> _OnCreate = _ => { };

        bool _ShouldFocus, _TagExistsWarning;

        const string _TagFieldID = "TagField";
        
        void OnGUI() {
            if (_ShouldFocus) {
                _ShouldFocus             =  false;
                EditorApplication.update += SetFocus;
            }

            GUI.SetNextControlName(_TagFieldID);
            EditorGUILayout.BeginHorizontal();
            {
                EditorUtilities.BeginLabelWidth(50f);
                _Tag = SceneToolbarSettings.SceneTagCollection.Normalise(
                    EditorGUILayout.TextField(EditorGUIUtility.TrTextContent("Tag", "The tag to create."), _Tag)
                );
                EditorUtilities.EndLabelWidth();

                _TagExistsWarning = _CurrentTags.Contains(_Tag);
                if (_TagExistsWarning) {
                    EditorGUILayout.LabelField(EditorGUIUtility.TrIconContent("console.warnicon.sml", "This tag is already in use."), GUILayout.Width(20f));
                }
            }
            EditorGUILayout.EndHorizontal();
        
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                const float BtnW = 100f;
                EditorUtilities.BeginDisabled(_TagExistsWarning);
                if (GUILayout.Button(EditorGUIUtility.TrTextContent("Create", "Create the tag."), GUILayout.Width(BtnW))) {
                    CreateTag(_Tag);
                }
                EditorUtilities.EndDisabled();
            }
            EditorGUILayout.EndHorizontal();
        
            HandleEnterKeyPress();
        }

        static void SetFocus() {
            GUI.FocusControl(_TagFieldID);
            EditorApplication.update -= SetFocus;
        }

        void HandleEnterKeyPress() {
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return) {
                if (GUI.GetNameOfFocusedControl() == _TagFieldID && !_TagExistsWarning) {
                    CreateTag(_Tag);
                }
            }
        }

        void CreateTag( string Tag ) {
            _OnCreate(Tag);
            Close();
        }

        /// <summary> Shows the tag creator window. </summary>
        /// <param name="OnCreate"> The callback to invoke when a tag is created. </param>
        /// <param name="CurrentTags"> The currently used tags. </param>
        public static void Show( Action<string> OnCreate, IEnumerable<string> CurrentTags ) {
            TagCreatorWindow Window = CreateInstance<TagCreatorWindow>();
            Window._OnCreate    = OnCreate;
            Window._CurrentTags = CurrentTags.ToArray();
            Window.titleContent = EditorGUIUtility.TrTextContent("Create Tag", "Create a new tag.", _IconName);

            Vector2 Size = new(300f, EditorGUIUtility.singleLineHeight * 2f + EditorGUIUtility.standardVerticalSpacing * 3f);
            Window.minSize = Size;
            Window.maxSize = Size;

            Window.ShowModal();
        }
    }
}