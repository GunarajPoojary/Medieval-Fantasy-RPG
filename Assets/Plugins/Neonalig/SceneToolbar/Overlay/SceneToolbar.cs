using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;

namespace Plugins.Neonalig.SceneToolbar.Overlay {

    /// <summary> Provides an overlay for quick scene switching. </summary>
    [Overlay(typeof(SceneView), id: ID, displayName: DisplayTitle, defaultDisplay: true)]
    [Icon(IconName)]
    internal sealed class SceneToolbar : ToolbarOverlay {
        /// <summary> The unique ID of this overlay. </summary>
        internal const string ID = "Neonalig.SceneToolbar";
        
        /// <summary> The display title of this overlay. </summary>
        internal const string DisplayTitle = "Scene Toolbar";
        
        /// <summary> The title of the package. </summary>
        internal const string PackageTitle = "Scene Toolbar";

        /// <summary> The icon name of this overlay. </summary>
        internal const string IconName = "UnityEditor.SceneHierarchyWindow";
        
        /// <summary> The version of this overlay. </summary>
        internal static readonly Version Version = new(1, 0);
        
        /// <summary> Creates a new instance of <see cref="SceneToolbar"/>. </summary>
        SceneToolbar() : base(
            PlayPauseButton.ID,
            SceneDropdown.ID,
            FavouriteScenesDropdown.ID
        ) { }

        #region Overrides of Overlay

        /// <inheritdoc />
        public override void OnCreated() {
            base.OnCreated();
            if (SceneToolbarSettings.IsFirstRun) {
                SceneToolbarSettings.Save();
                // Debug.Log($"Scene Toolbar v{Version} was successfully installed!\nGood on ya!");
                // SettingsService.OpenUserPreferences(SceneToolbarSettingsProvider.Path);
                FirstTimeWindow.ShowAsUtility();
            }
        }

        #endregion

    }

}
