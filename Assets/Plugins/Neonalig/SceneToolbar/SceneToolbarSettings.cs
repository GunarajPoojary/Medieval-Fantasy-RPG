using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Plugins.Neonalig.SceneToolbar.Attributes;
using Plugins.Neonalig.SceneToolbar.Overlay;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

// ReSharper disable once RedundantNullableDirective
#nullable enable

namespace Plugins.Neonalig.SceneToolbar {
    /// <summary> Contains the settings for the scene toolbar. </summary>
    [FilePath(SettingsPath, FilePathAttribute.Location.PreferencesFolder)]
    internal sealed class SceneToolbarSettings : ScriptableSingleton<SceneToolbarSettings> {
        [SerializeField, Tooltip("The entries of the scene toolbar.")]
        List<SceneEntry> _Entries = new();

        /// <summary> Gets the entries of the scene toolbar. </summary>
        /// <returns> The entries of the scene toolbar. </returns>
        internal static IReadOnlyList<SceneEntry> Entries => instance._Entries;
        
        [SerializeField, Tooltip("The play/pause button method.")]
        PlayButtonMethod _PlayButtonMethod = PlayButtonMethod.PlayBuild;
        
        /// <summary> Gets or sets the play/pause button method. </summary>
        internal static PlayButtonMethod PlayButtonMethod {
            get => instance._PlayButtonMethod;
            set {
                if (instance._PlayButtonMethod == value) { return; }
                instance._PlayButtonMethod = value;
                Save();
            }
        }
        
        [SerializeField, Tooltip("Whether the dropdown opens the scene additively (appends to the current scene) or not (replaces the current scene).")]
        bool _OpenAdditive = false;

        /// <summary> Gets or sets whether the dropdown opens the scene additively (appends to the current scene) or not (replaces the current scene). </summary>
        internal static bool OpenAdditive {
            get => instance._OpenAdditive;
            set {
                if (instance._OpenAdditive == value) { return; }
                instance._OpenAdditive = value;
                Save();
            }
        }
        
        [SerializeField, Tooltip("Whether to include scenes not present in the build settings when the dropdown is opened.")]
        bool _IncludeNotInBuild = false;
        
        /// <summary> Gets or sets whether to include scenes not present in the build settings when the dropdown is opened. </summary>
        internal static bool IncludeNotInBuild {
            get => instance._IncludeNotInBuild;
            set {
                if (instance._IncludeNotInBuild == value) { return; }
                instance._IncludeNotInBuild = value;
                Save();
            }
        }

        /// <summary> The path to the settings file. </summary>
        internal const string SettingsPath = "Neonalig/SceneToolbar/Settings.yaml";

        static bool SaveAsText => EditorSettings.serializationMode == SerializationMode.ForceText;

        /// <inheritdoc cref="ScriptableSingleton{T}.Save"/>
        internal static void Save() => instance.Save(SaveAsText);

        /// <summary> Adds the specified entry to the scene toolbar. </summary>
        /// <param name="Entry"> The entry to add. </param>
        internal static void AddEntry( SceneEntry Entry ) {
            instance._Entries.Add(Entry);
            Save();
        }

        /// <summary> Removes the specified entry from the scene toolbar. </summary>
        /// <param name="Entry"> The entry to remove. </param>
        /// <returns> <see langword="true"/> if the entry was removed; otherwise, <see langword="false"/>. </returns>
        internal static bool RemoveEntry( SceneEntry Entry ) {
            if (instance._Entries.Remove(Entry)) {
                Save();
                return true;
            }

            return false;
        }

        /// <summary> Gets the index of the specified entry. </summary>
        /// <param name="Entry"> The entry. </param>
        /// <returns> The index of the specified entry. <c>-1</c> if the entry is not found. </returns>
        internal static int IndexOf( SceneEntry Entry ) => instance._Entries.IndexOf(Entry);

        /// <summary> Gets the entry at the specified index. </summary>
        /// <param name="Idx"> The index. </param>
        /// <returns> The entry at the specified index. </returns>
        /// <exception cref="System.IndexOutOfRangeException"> Thrown if the index is out of range. </exception>
        internal static SceneEntry ElementAt( int Idx ) => instance._Entries[Idx];

        /// <summary> Gets the number of entries in the scene toolbar. </summary>
        /// <returns> The number of entries in the scene toolbar. </returns>
        internal static int Count => instance._Entries.Count;

        /// <summary> Gets all currently used tags. </summary>
        /// <returns> All currently used tags. </returns>
        internal static IEnumerable<string> Tags => instance._Entries.SelectMany(Entry => Entry.Tags).Distinct();
        
        /// <summary> Gets the entries of the scene toolbar that are favourited. </summary>
        /// <returns> The entries of the scene toolbar that are favourited. </returns>
        internal static IEnumerable<SceneEntry> Favourites => instance._Entries.Where(Entry => Entry.IsFavourite);
        
        /// <summary> Gets whether the specified scene is favourited. </summary>
        /// <param name="Path"> The path to the scene. </param>
        /// <returns> <see langword="true"/> if the specified scene is favourited; otherwise, <see langword="false"/>. </returns>
        internal static bool GetIsFavourite( [LocalizationRequired(false)] string Path ) {
            if (string.IsNullOrEmpty(Path)) { return false; }
            return instance._Entries.Any(Entry => Entry.Path == Path && Entry.IsFavourite);
        }
        
        /// <summary> Sets whether the specified scene is favourited. </summary>
        /// <param name="Path"> The path to the scene. </param>
        /// <param name="IsFavourite"> Whether the scene is favourited. </param>
        internal static void SetIsFavourite( [LocalizationRequired(false)] string Path, bool IsFavourite ) {
            Debug.Assert(!string.IsNullOrEmpty(Path), "Path must not be null or empty.");
            SceneEntry? Entry = instance._Entries.FirstOrDefault(Entry => Entry.Path == Path);
            if (Entry == null) {
                Entry = new(Path, IsFavourite);
                instance._Entries.Add(Entry);
            } else {
                Entry.IsFavourite = IsFavourite;
            }
        }

        /// <summary> A scene entry. </summary>
        [Serializable]
        internal sealed class SceneEntry {
            [SerializeField, Tooltip("The path to the scene."), LocalizationRequired(false), SceneDropdown]
            string _Path;

            [SerializeField, Tooltip("Whether the scene is favourited."), LeftToggle]
            bool _IsFavourite = false;

            [SerializeField, Tooltip("The tags of the scene.")]
            SceneTagCollection _Tags = new();

            /// <summary> Gets or sets the path to the scene. </summary>
            [LocalizationRequired(false)]
            internal string Path {
                get => _Path;
                set {
                    if (_Path == value) { return; }
                    _Path = value;
                    Save();
                }
            }

            /// <summary> Gets or sets whether the scene is favourited. </summary>
            internal bool IsFavourite {
                get => _IsFavourite;
                set {
                    if (_IsFavourite == value) { return; }
                    _IsFavourite = value;
                    Save();
                }
            }

            /// <summary> Gets the tag(s) of the scene. </summary>
            internal SceneTagCollection Tags => _Tags;

            /// <summary> Gets whether the scene has the specified tag. </summary>
            /// <param name="Tag"> The tag. </param>
            /// <returns> <see langword="true"/> if the scene has the specified tag; otherwise, <see langword="false"/>. </returns>
            internal bool HasTag( [LocalizationRequired(false)] string Tag ) => _Tags.HasTag(Tag);

            /// <summary> Gets whether the scene has the specified tag(s). </summary>
            /// <param name="Tags"> The tag(s). If multiple tags are specified, all must be present for this to return <see langword="true"/>. </param>
            /// <returns> <see langword="true"/> if the scene has all of the specified tag(s); otherwise, <see langword="false"/>. </returns>
            internal bool HasTags( [LocalizationRequired(false)] params string[] Tags ) => _Tags.HasTags(Tags);
            
            /// <summary> Creates a new scene entry. </summary>
            /// <param name="Path"> The path to the scene. </param>
            /// <param name="IsFavourite"> Whether the scene is favourited. </param>
            /// <param name="Tags"> The tags of the scene. </param>
            internal SceneEntry( [LocalizationRequired(false)] string Path, bool IsFavourite = false, params string[] Tags ) {
                _Path        = Path;
                _IsFavourite = IsFavourite;
                foreach (string Tag in Tags) {
                    _Tags.AddTag(Tag);
                }
            }
        }

        [Serializable]
        internal sealed class SceneTagCollection : IReadOnlyList<string> {
            [SerializeField, Tooltip("The tags."), LocalizationRequired(false)]
            string[] _Tags = Array.Empty<string>();

            /// <summary> Gets the tags. </summary>
            [LocalizationRequired(false)]
            internal IReadOnlyList<string> Tags => _Tags;

            /// <summary> Normalises the specified tag. </summary>
            /// <param name="Tag"> The tag. </param>
            /// <returns> The normalised tag. </returns>
            [Pure]
            internal static string Normalise( [LocalizationRequired(false)] string Tag ) {
                // Tags must be in lower kebab-case.
                Tag = Tag.ToLowerInvariant();
                Tag = Tag.Replace(' ', '-');
                return Tag;
            }

            /// <summary> Gets whether the specified tag is normalised. </summary>
            /// <param name="Tag"> The tag. </param>
            /// <returns> <see langword="true"/> if the specified tag is normalised; otherwise, <see langword="false"/>. </returns>
            internal static bool IsNormalised( [LocalizationRequired(false)] string Tag ) => Tag.Any(C => char.IsUpper(C) || char.IsWhiteSpace(C));

            /// <summary> Gets whether the scene has the specified tag. </summary>
            /// <param name="Tag"> The tag. </param>
            /// <returns> <see langword="true"/> if the scene has the specified tag; otherwise, <see langword="false"/>. </returns>
            internal bool HasTag( [LocalizationRequired(false)] string Tag ) {
                Debug.Assert(!IsNormalised(Tag), "Tag must be normalised before checking if it is present.");
                return Array.Exists(_Tags, EntryTag => string.Equals(EntryTag, Tag, StringComparison.OrdinalIgnoreCase));
            }

            /// <summary> Gets whether the scene has the specified tag(s). </summary>
            /// <param name="Tags"> The tag(s). If multiple tags are specified, all must be present for this to return <see langword="true"/>. </param>
            /// <returns> <see langword="true"/> if the scene has all of the specified tag(s); otherwise, <see langword="false"/>. </returns>
            internal bool HasTags( [LocalizationRequired(false)] params string[] Tags ) {
                foreach (string Tag in Tags) {
                    if (!HasTag(Tag)) {
                        return false;
                    }
                }

                return true;
            }

            /// <summary> Adds the specified tag, if it is not already present. </summary>
            /// <param name="Tag"> The tag to add. </param>
            /// <returns> <see langword="true"/> if the tag was added; otherwise, <see langword="false"/>. </returns>
            internal bool AddTag( [LocalizationRequired(false)] string Tag ) {
                Debug.Assert(!IsNormalised(Tag), "Tag must be normalised before adding.");
                if (HasTag(Tag)) {
                    return false;
                }

                Array.Resize(ref _Tags, _Tags.Length + 1);
                _Tags[^1] = Tag;
                Save();
                return true;
            }

            /// <summary> Removes the specified tag, if it is present. </summary>
            /// <param name="Tag"> The tag to remove. </param>
            /// <returns> <see langword="true"/> if the tag was removed; otherwise, <see langword="false"/>. </returns>
            internal bool RemoveTag( [LocalizationRequired(false)] string Tag ) {
                Debug.Assert(!IsNormalised(Tag), "Tag must be normalised before removing.");
                int Idx = Array.FindIndex(_Tags, EntryTag => string.Equals(EntryTag, Tag, StringComparison.OrdinalIgnoreCase));
                if (Idx == -1) {
                    return false;
                }

                Array.Copy(_Tags, Idx + 1, _Tags, Idx, _Tags.Length - Idx - 1);
                Array.Resize(ref _Tags, _Tags.Length - 1);
                Save();
                return true;
            }

            #region Implementation of IEnumerable

            /// <inheritdoc />
            IEnumerator<string> IEnumerable<string>.GetEnumerator() => ((IEnumerable<string>)_Tags).GetEnumerator();

            /// <inheritdoc />
            IEnumerator IEnumerable.GetEnumerator() => _Tags.GetEnumerator();

            #endregion

            #region Implementation of IReadOnlyCollection<out string>

            /// <inheritdoc />
            int IReadOnlyCollection<string>.Count => _Tags.Length;

            #endregion

            #region Implementation of IReadOnlyList<out string>

            /// <inheritdoc />
            string IReadOnlyList<string>.this[ int Index ] => _Tags[Index];

            #endregion

        }

        /// <summary> Gets whether this is the first run of the plugin. </summary>
        /// <returns> <see langword="true"/> if this is the first run of the plugin; otherwise, <see langword="false"/>. </returns>
        internal static bool IsFirstRun => !File.Exists(GetFilePath());
    }
}
