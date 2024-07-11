using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.SceneManagement;
using SysPath = System.IO.Path;

namespace Plugins.Neonalig.SceneToolbar.Overlay {
    /// <summary> The scene dropdown. </summary>
    [EditorToolbarElement(ID, typeof(SceneView))]
    internal sealed class SceneDropdown : EditorToolbarDropdown {
        /// <summary> The ID. </summary>
        internal const string ID = SceneToolbar.ID + ".SceneDropdown";

        public SceneDropdown() {
            UpdateView();
            clicked += ShowMenu;
            
            // Listen to scene changes.
            EditorSceneManager.activeSceneChangedInEditMode += OnSceneChanged;
        }
        
        ~SceneDropdown() {
            // Stop listening to scene changes.
            EditorSceneManager.activeSceneChangedInEditMode -= OnSceneChanged;
        }
        
        void OnSceneChanged( Scene Current, Scene Next ) => UpdateView();

        void UpdateLabel( bool Mark = true ) {
            text = SceneManager.GetActiveScene().name;
            if (Mark) {
                MarkDirtyRepaint();
            }
        }

        void UpdateIcon( bool Mark = true ) {
            icon = GetIcon("SceneAsset Icon");
            if (Mark) {
                MarkDirtyRepaint();
            }
        }

        void UpdateView() {
            UpdateLabel(false);
            UpdateIcon(false);
            MarkDirtyRepaint();
        }

        GenericMenu GetMenu() {
            GenericMenu Menu = new();
            Menu.AddItem(
                EditorGUIUtility.TrTextContent("Open Additive"), SceneToolbarSettings.OpenAdditive, () => {
                    SceneToolbarSettings.OpenAdditive = !SceneToolbarSettings.OpenAdditive;
                }
            );
            Menu.AddItem(
                EditorGUIUtility.TrTextContent("Include not in Build"), SceneToolbarSettings.IncludeNotInBuild, () => {
                    SceneToolbarSettings.IncludeNotInBuild = !SceneToolbarSettings.IncludeNotInBuild;
                }
            );
            Menu.AddSeparator(string.Empty);

            string[] Paths = GetScenes(SceneToolbarSettings.IncludeNotInBuild).ToArray();
            if (Paths.Length == 0 || Paths.All(string.IsNullOrEmpty)) {
                Menu.AddDisabledItem(EditorGUIUtility.TrTextContent("No scenes found."));
                return Menu;
            }

            bool DuplicateNames = Paths.GroupBy(SysPath.GetFileNameWithoutExtension).Any(Group => Group.Count() > 1);

            ISet<string> ActivePaths = GetActiveScenes();
            int          ActiveLn    = ActivePaths.Count;

            foreach (string Path in Paths) {
                bool IsActive = ActivePaths.Contains(Path);
                Menu.AddItem(EditorUtilities.GetSceneLabel(Path, DuplicateNames), IsActive, OnSelection);

                void OnSelection() {
                    if (IsActive) {
                        if (ActiveLn > 1) {
                            if (SceneToolbarSettings.OpenAdditive) {
                                CloseScene(Path);
                                return;
                            }
                        } else {
                            return;
                        }
                    }

                    OpenScene(Path);
                }
            }

            return Menu;
        }

        internal static ISet<string> GetActiveScenes() {
            HashSet<string> Paths = new();
            int             Ln    = SceneManager.sceneCount;
            for (int I = 0; I < Ln; I++) {
                Scene Scene = SceneManager.GetSceneAt(I);
                if (Scene.path != string.Empty) {
                    Paths.Add(Scene.path);
                }
            }
            return Paths;
        }

        void ShowMenu() => GetMenu().ShowAsContext();

        void OpenScene( string Path ) {
            // If we aren't in play mode, and the current scene has changes, prompt the user to save.
            if (!EditorApplication.isPlaying && SceneManager.GetActiveScene().isDirty) {
                if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
                    return;
                }
            }

            EditorSceneManager.OpenScene(Path, SceneToolbarSettings.OpenAdditive ? OpenSceneMode.Additive : OpenSceneMode.Single);
            UpdateView();
        }

        void CloseScene( string Path ) {
            EditorSceneManager.CloseScene(SceneManager.GetSceneByPath(Path), true);
            UpdateView();
        }

        internal static IEnumerable<string> GetScenes( bool IncludeNotInBuildSettings ) {
            HashSet<string> Paths = new();

            int BuildScenes = SceneManager.sceneCount;
            for (int I = 0; I < BuildScenes; I++) {
                Scene Scene = SceneManager.GetSceneAt(I);
                if (Scene.path != string.Empty) {
                    Paths.Add(Scene.path);
                }
            }

            if (IncludeNotInBuildSettings) {
                SceneAsset[] Assets = AssetDatabase.FindAssets("t:SceneAsset")
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .Select(AssetDatabase.LoadAssetAtPath<SceneAsset>)
                    .ToArray();
                foreach (SceneAsset Asset in Assets) {
                    if (Asset == null) { continue; }

                    string Path = AssetDatabase.GetAssetPath(Asset);
                    if (Path == string.Empty) { continue; }

                    if (Paths.Contains(Path)) { continue; }

                    Paths.Add(Path);
                }
            }

            return Paths;
        }

        static Texture2D GetIcon( [LocalizationRequired(false)] string Name ) => (Texture2D)EditorGUIUtility.IconContent(Name).image;

    }
}
