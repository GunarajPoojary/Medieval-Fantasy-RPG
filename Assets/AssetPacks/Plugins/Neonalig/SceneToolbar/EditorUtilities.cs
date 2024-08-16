using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using SysPath = System.IO.Path;

// ReSharper disable once RedundantNullableDirective
#nullable enable

namespace Plugins.Neonalig.SceneToolbar {
    internal static class EditorUtilities {
        static class Enum<TEnum> where TEnum : Enum {
            /// <summary> The values of the enum. </summary>
            static readonly TEnum[] _Values;
            
            /// <summary> The labels of the enum. </summary>
            // ReSharper disable once StaticMemberInGenericType
            internal static readonly GUIContent[] Labels; // Ideally we wouldn't expose an array here, and instead use IReadOnlyList<T>, but Unity editor methods only support arrays.
            
            /// <summary> Gets the index of the specified value. </summary>
            /// <param name="Value"> The value. </param>
            /// <returns> The index of the specified value. <c>-1</c> if the value is not found. </returns>
            [Pure]
            internal static int IndexOf( TEnum Value ) => Array.IndexOf(_Values, Value);

            /// <summary> Gets the value at the specified index. </summary>
            /// <param name="Idx"> The index. </param>
            /// <returns> The value at the specified index. </returns>
            /// <exception cref="IndexOutOfRangeException"> Thrown if the index is out of range. </exception>
            [Pure]
            internal static TEnum ElementAt( int Idx ) => _Values[Idx];
            
            /// <summary> Gets the number of elements in the enum. </summary>
            [Pure]
            internal static int Count => _Values.Length;

            static Enum() {
                Array EnumValues = Enum.GetValues(typeof(TEnum));
                int Ln = EnumValues.Length;
                _Values = new TEnum[Ln];
                Labels = new GUIContent[Ln];
                for (int I = 0; I < Ln; I++) {
                    // 1. Find member via reflection
                    string     MemberName = EnumValues.GetValue(I).ToString();
                    MemberInfo Member     = typeof(TEnum).GetMember(MemberName).FirstOrDefault() ?? throw new MissingMemberException(typeof(TEnum).Name, MemberName);
                    // 2. Get any custom display attributes (InspectorName, Tooltip, etc.)
                    string   Name, Tooltip;
                    Texture? Icon;
                    if (Member.GetCustomAttribute<InspectorNameAttribute>() is { } InspectorName) {
                        Name = InspectorName.displayName;
                    } else {
                        Name = ObjectNames.NicifyVariableName(MemberName);
                    }
                    if (Member.GetCustomAttribute<TooltipAttribute>() is { } TooltipAttribute) {
                        Tooltip = TooltipAttribute.tooltip;
                    } else {
                        Tooltip = string.Empty;
                    }
                    if (Member.GetCustomAttribute<IconAttribute>() is { } IconAttribute) {
                        string Path = IconAttribute.path;
                        Icon = AssetDatabase.LoadAssetAtPath<Texture>(Path);
                        if (Icon == null) {
                            Debug.LogWarning($"Icon not found at path \"{Path}\".");
                        }
                    } else {
                        Icon = null;
                    }
                    // 3. Create GUIContent
                    _Values[I] = (TEnum)EnumValues.GetValue(I);
                    Labels[I] = EditorGUIUtility.TrTextContentWithIcon(Name, Tooltip, Icon);
                }
            }
        }
        
        /// <summary> Draws a horizontally-aligned collection of buttons for the specified enum. </summary>
        /// <typeparam name="TEnum"> The type of the enum. </typeparam>
        /// <param name="R"> The rect to draw the buttons in. </param>
        /// <param name="Lbl"> The label of the buttons. Value may be <see langword="null"/>. </param>
        /// <param name="Value"> The value of the enum. </param>
        /// <param name="Changed"> [out] Whether the value was changed. </param>
        /// <returns> The new value of the enum. </returns>
        internal static TEnum DrawEnumToolbar<TEnum>( Rect R, GUIContent? Lbl, TEnum Value, out bool Changed ) where TEnum : Enum {
            bool HasLbl = Lbl is not null;
            if (HasLbl) {
                R = EditorGUI.PrefixLabel(R, Lbl);
            }
            
            int OldIdx = Enum<TEnum>.IndexOf(Value);
            int NewIdx = GUI.Toolbar(R, OldIdx, Enum<TEnum>.Labels/*, EditorStyles.miniButton*/);
            if (NewIdx != OldIdx) {
                Changed = true;
                return Enum<TEnum>.ElementAt(NewIdx);
            }
            Changed = false;
            return Value;
        }
        
        static readonly Stack<bool> _EnabledStack = new();
        /// <summary> Begins a GUI enabled group. </summary>
        /// <param name="Enabled"> Whether the group should be enabled. </param>
        internal static void BeginEnabled( bool Enabled = true ) {
            _EnabledStack.Push(GUI.enabled);
            GUI.enabled = Enabled;
        }
        /// <summary> Begins a GUI disabled group. </summary>
        /// <param name="Disabled"> Whether the group should be disabled. </param>
        internal static void BeginDisabled( bool Disabled = true ) => BeginEnabled(!Disabled);
        
        /// <summary> Ends a GUI enabled group. </summary>
        internal static void EndEnabled() => GUI.enabled = _EnabledStack.Pop();
        /// <summary> Ends a GUI disabled group. </summary>
        internal static void EndDisabled() => EndEnabled();

        /// <summary> Draws an object field with a dropdown menu. </summary>
        /// <typeparam name="TObject"> The type of the object. </typeparam>
        /// <param name="R"> The rect to draw the field in. </param>
        /// <param name="Lbl"> The label of the field. Value may be <see langword="null"/>. </param>
        /// <param name="Value"> The value of the field. </param>
        /// <param name="OnSelection"> The action to perform when an item is selected. </param>
        /// <param name="AllowAssets"> Whether to allow assets to be selected. </param>
        /// <param name="AllowSceneObjects"> Whether to allow scene objects to be selected. </param>
        internal static void DrawObjectFieldWithDropdown<TObject>( Rect R, GUIContent? Lbl, TObject? Value, Action<TObject?> OnSelection, bool AllowAssets = true, bool AllowSceneObjects = true ) where TObject : Object {
            DrawFieldWithDropdown(R, Lbl, Value, OnSelection, DrawField, GetItems, GetObjectLabel, AllowNone: true);

            IEnumerable<TObject> GetItems() {
                if (AllowAssets) {
                    foreach (TObject Item in AssetDatabase.FindAssets($"t:{typeof(TObject).Name}").Select(AssetDatabase.GUIDToAssetPath).Select(AssetDatabase.LoadAssetAtPath<TObject>)) {
                        yield return Item;
                    }
                }

                if (AllowSceneObjects) {
                    foreach (TObject Item in Resources.FindObjectsOfTypeAll<TObject>()) {
                        yield return Item;
                    }
                }
            }

            TObject? DrawField( Rect R, TObject? V ) {
                TObject? NewV = (TObject?)EditorGUI.ObjectField(R, V, typeof(TObject), AllowSceneObjects);
                if (NewV != V && NewV != null && !AllowAssets && AssetDatabase.Contains(NewV)) {
                    return null;
                }
                return NewV;
            }
        }
        
        /// <summary> Gets the label for the specified object. </summary>
        /// <typeparam name="TObject"> The type of the object. </typeparam>
        /// <param name="Item"> The item. </param>
        /// <returns> The label for the specified object. </returns>
        internal static GUIContent GetObjectLabel<TObject>( TObject Item ) where TObject : Object =>
            EditorGUIUtility.ObjectContent(Item, typeof(TObject));
        
        /// <summary> Draws a scene field with a dropdown menu. </summary>
        /// <param name="R"> The rect to draw the field in. </param>
        /// <param name="Lbl"> The label of the field. Value may be <see langword="null"/>. </param>
        /// <param name="Value"> The value of the field. </param>
        /// <param name="OnSelection"> The action to perform when an item is selected. </param>
        internal static void DrawSceneFieldWithDropdown( Rect R, GUIContent? Lbl, SceneAsset? Value, Action<SceneAsset?> OnSelection ) {
            DrawFieldWithDropdown(R, Lbl, Value, OnSelection, DrawField, GetItems, GetSceneLabel, AllowNone: true);
            
            IEnumerable<SceneAsset> GetItems() {
                foreach (string Path in AssetDatabase.FindAssets("t:SceneAsset").Select(AssetDatabase.GUIDToAssetPath)) {
                    yield return AssetDatabase.LoadAssetAtPath<SceneAsset>(Path);
                }
            }
            
            SceneAsset? DrawField( Rect R, SceneAsset? V ) => (SceneAsset?)EditorGUI.ObjectField(R, V, typeof(SceneAsset), false);
        }
        
        /// <summary> Draws a scene field with a dropdown menu. </summary>
        /// <param name="R"> The rect to draw the field in. </param>
        /// <param name="Lbl"> The label of the field. Value may be <see langword="null"/>. </param>
        /// <param name="Value"> The value of the field. </param>
        /// <param name="OnSelection"> The action to perform when an item is selected. </param>
        internal static void DrawSceneFieldWithDropdown( Rect R, GUIContent? Lbl, string Value, Action<string> OnSelection ) {
            bool? _DuplicateNames = null;
            DrawFieldWithDropdown(R, Lbl, Value, OnSelection!, DrawField!, GetItems, GetSceneLabel, AllowNone: true);
            
            IEnumerable<string> GetItems() {
                foreach (string Path in AssetDatabase.FindAssets("t:SceneAsset").Select(AssetDatabase.GUIDToAssetPath)) {
                    yield return Path;
                }
            }

            static string DrawField( Rect R, string V ) => DrawFilePathPicker(R, null, V, "unity", "Assets");
            
            bool DuplicateNames() => _DuplicateNames ??= GetItems().GroupBy(SysPath.GetFileNameWithoutExtension).Any(Group => Group.Count() > 1);
            
            GUIContent GetSceneLabel( string Path ) {
                GUIContent Lbl = EditorUtilities.GetSceneLabel(Path, NameIsPath: DuplicateNames());
                return Lbl;
            }
        }

        /// <summary> Gets the label for the specified scene. </summary>
        /// <param name="Scene"> The scene. </param>
        /// <returns> The label for the specified scene. </returns>
        internal static GUIContent GetSceneLabel( SceneAsset Scene ) =>
            EditorGUIUtility.TrTextContentWithIcon(Scene.name,  AssetDatabase.GetAssetPath(Scene), AssetPreview.GetMiniTypeThumbnail(typeof(SceneAsset)));

        /// <summary> Gets the label for the specified scene. </summary>
        /// <param name="Path"> The path to the scene. </param>
        /// <param name="NameIsPath"> Whether the name of the scene is the path (<see langword="true"/>) or the file name (<see langword="false"/>). </param>
        /// <returns> The label for the specified scene. </returns>
        internal static GUIContent GetSceneLabel( string Path, bool NameIsPath = true ) =>
            EditorGUIUtility.TrTextContentWithIcon(NameIsPath ? Path : SysPath.GetFileNameWithoutExtension(Path), Path, AssetPreview.GetMiniTypeThumbnail(typeof(SceneAsset)));

        static void DrawFieldWithDropdown<T>(
            Rect                   R,
            GUIContent?            Lbl,
            T?                     Value,
            Action<T?>             OnSelection,
            Func<Rect, T?, T?>     DrawField,
            Func<IEnumerable<T>>   GetItems,
            Func<T, GUIContent>    GetLabel,
            bool                   AllowNone = true,
            IEqualityComparer<T?>? Comparer  = null
        ) {
            const float BtnWidth = 20f;
            Rect        BtnR     = new(R.xMax - BtnWidth, R.y, BtnWidth, R.height);
            R.width -= BtnWidth;

            bool HasLbl = Lbl is not null;
            if (HasLbl) {
                R = EditorGUI.PrefixLabel(R, Lbl);
            }

            Comparer ??= EqualityComparer<T?>.Default;

            T? NewValue = DrawField(R, Value);
            if (!Comparer.Equals(NewValue, Value)) {
                OnSelection(NewValue);
            }

            if (GUI.Button(BtnR, GUIContent.none, EditorStyles.popup)) {
                GenericMenu Menu = new();
                if (AllowNone) {
                    Menu.AddItem(EditorGUIUtility.TrTextContent("None"), Value == null, () => OnSelection(default));
                    Menu.AddSeparator(string.Empty);
                }

                foreach (T Item in GetItems()) {
                    Menu.AddItem(GetLabel(Item), Comparer.Equals(Item, Value), () => OnSelection(Item));
                }

                Menu.DropDown(BtnR);
            }
        }

        /// <summary> Draws a file path picker. </summary>
        /// <param name="R"> The rect to draw the field in. </param>
        /// <param name="Lbl"> The label of the field. Value may be <see langword="null"/>. </param>
        /// <param name="Value"> The value of the field. </param>
        /// <param name="Extension"> The extension of the file. </param>
        /// <param name="Location"> The location of the file. </param>
        /// <returns> The new value of the field. </returns>
        internal static string DrawFilePathPicker( Rect R, GUIContent? Lbl, string Value, string Extension, string Location ) {
            bool HasLbl = Lbl is not null;
            if (HasLbl) {
                R = EditorGUI.PrefixLabel(R, Lbl);
            }

            const float BtnWd = 20f;
            Rect        BtnR  = new(R.xMax - BtnWd, R.y, BtnWd, R.height);
            R.width -= BtnWd;

            string NewValue = EditorGUI.TextField(R, Value);
            if (GUI.Button(BtnR, EditorGUIUtility.TrTextContent("..."), EditorStyles.miniButtonRight)) {
                string Path = EditorUtility.SaveFilePanel("Select File", Location, string.Empty, Extension);
                if (!string.IsNullOrEmpty(Path)) {
                    // Make path relative to project folder
                    Path = Path.Replace(Application.dataPath, "Assets");
                    if (Path.StartsWith("Assets/")) {
                        NewValue = Path;
                    } else {
                        Debug.LogWarning($"Path \"{Path}\" is not in the project folder.");
                    }
                }
            }
            return NewValue;
        }

        const  float    _ChipPadding = 4f;
        static GUIStyle ChipLabelStyle   => EditorStyles.miniButtonLeft;
        static GUIStyle ChipDismissStyle => EditorStyles.miniButtonRight;

        static float CalculateChipWidth( GUIContent Label ) =>
            ChipLabelStyle.CalcSize(Label).x + _ChipPadding * 2f
            + ChipDismissStyle.CalcSize(EditorGUIUtility.TrIconContent("Toolbar Minus")).x;

        /// <summary> Draws a chip. </summary>
        /// <param name="R"> The rect to draw the chip in. </param>
        /// <param name="Label"> The label of the chip. </param>
        /// <returns> <see langword="true"/> if the chip remove button was clicked; otherwise, <see langword="false"/>. </returns>
        internal static bool DrawChip( Rect R, GUIContent Label ) {
            const float BtnWd = 20f;
            Rect        BtnR  = new(R.xMax - BtnWd, R.y, BtnWd - _ChipPadding, R.height);
            R.width -= BtnWd;

            BeginDisabled();
            GUI.Label(R, Label, ChipLabelStyle);
            EndDisabled();
            return GUI.Button(BtnR, EditorGUIUtility.TrIconContent("Toolbar Minus"), ChipDismissStyle);
        }

        /// <summary> Draws a chip bar. </summary>
        /// <typeparam name="T"> The type of the values. </typeparam>
        /// <param name="R"> The rect to draw the chip bar in. </param>
        /// <param name="Lbl"> The label of the chip bar. Value may be <see langword="null"/>. </param>
        /// <param name="Values"> The values of the chip bar. </param>
        /// <param name="OnRemove"> The action to perform when a chip is removed. </param>
        /// <param name="OnAdd"> The action to perform when a chip is added. </param>
        /// <param name="GetAvailableValues"> The function to get the available values for the chip bar. </param>
        /// <param name="GetLabel"> The function to get the label for a value. </param>
        /// <param name="GetMenu"> The function to generate a generic menu for the chip bar. Can be overridden to add custom menu items. </param>
        /// <param name="Comparer"> The comparer to use for the values. </param>
        internal static void DrawChipBar<T>(
            Rect                  R,
            GUIContent?           Lbl,
            IReadOnlyList<T>      Values,
            Action<T, int>        OnRemove,
            Action<T>             OnAdd,
            Func<IEnumerable<T>>  GetAvailableValues,
            Func<T, GUIContent>   GetLabel,
            Func<GenericMenu>?    GetMenu  = null,
            IEqualityComparer<T>? Comparer = null
        ) {
            const float BtnWd = 20f;
            Rect        BtnR  = new(R.xMax - BtnWd, R.y, BtnWd, R.height);
            R.width -= BtnWd;

            bool HasLbl = Lbl is not null;
            if (HasLbl) {
                R = EditorGUI.PrefixLabel(R, Lbl);
            }

            Comparer ??= EqualityComparer<T>.Default;

            float[] Widths = Values.Select(GetLabel).Select(CalculateChipWidth).ToArray();
            const float BetweenPadding = 2f;
            
            int Idx = 0;
            foreach (T Value in Values) {
                float Wd    = Widths[Idx];
                Rect  ChipR = new(R.x, R.y, Wd, R.height);
                if (DrawChip(ChipR, GetLabel(Value))) {
                    OnRemove(Value, Idx);
                }

                R.x += Wd + BetweenPadding;
                Idx++;
            }

            if (GUI.Button(BtnR, EditorGUIUtility.TrIconContent("Toolbar Plus"), EditorStyles.iconButton)) {
                GenericMenu Menu = GetMenu?.Invoke() ?? new();
                foreach (T Value in GetAvailableValues()) {
                    if (Values.Contains(Value, Comparer)) {
                        continue;
                    }

                    Menu.AddItem(GetLabel(Value), false, () => OnAdd(Value));
                }
                Menu.DropDown(BtnR);
            }
        }

        static readonly Stack<float> _LabelWidthStack = new();
        /// <summary> Begins a label width group. </summary>
        /// <param name="F"> The label width. </param>
        internal static void BeginLabelWidth( float F ) {
            _LabelWidthStack.Push(EditorGUIUtility.labelWidth);
            EditorGUIUtility.labelWidth = F;
        }
        
        /// <summary> Ends a label width group. </summary>
        internal static void EndLabelWidth() => EditorGUIUtility.labelWidth = _LabelWidthStack.Pop();
    }
}
