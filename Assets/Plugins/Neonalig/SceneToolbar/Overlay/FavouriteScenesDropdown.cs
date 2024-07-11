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
    /// <summary> The favourite scenes dropdown. </summary>
    [EditorToolbarElement(ID, typeof(SceneView))]
    internal sealed class FavouriteScenesDropdown : EditorToolbarDropdown {
        /// <summary> The ID. </summary>
        internal const string ID = SceneToolbar.ID + ".FavouriteScenesDropdown";
        
        public FavouriteScenesDropdown() {
            text    =  string.Empty;//"Favourites";
            icon    =  GetIcon("Favorite Icon");
            clicked += ShowMenu;
            
            // Listen to scene changes.
            EditorSceneManager.activeSceneChangedInEditMode += OnSceneChanged;
        }
        
        ~FavouriteScenesDropdown() {
            // Stop listening to scene changes.
            EditorSceneManager.activeSceneChangedInEditMode -= OnSceneChanged;
        }
        
        void OnSceneChanged( Scene Current, Scene Next ) => UpdateIcon();
        
        void UpdateIcon( bool Mark = true ) {
            icon = GetIcon(SceneToolbarSettings.GetIsFavourite(CurrentScene));
            if (Mark) {
                MarkDirtyRepaint();
            }
        }

        static string CurrentScene {
            get {
                string Current = SceneManager.GetActiveScene().path;
                return Current;
            }
        }
        
        void UpdateView() {
            UpdateIcon(false);
            MarkDirtyRepaint();
        }

        GenericMenu GetMenu() {
            GenericMenu Menu = new();
            
            bool CurrentIsFav = SceneToolbarSettings.GetIsFavourite(CurrentScene);
            if (!CurrentIsFav) {
                Menu.AddItem(
                    EditorGUIUtility.TrTextContent("Add to Favourites"),
                    false,
                    MakeCurrentFav
                );
                void MakeCurrentFav() {
                    SceneToolbarSettings.SetIsFavourite(CurrentScene, true);
                    UpdateView();
                }
            } else {
                Menu.AddItem(
                    EditorGUIUtility.TrTextContent("Remove from Favourites"),
                    false,
                    MakeCurrentNotFav
                );
                void MakeCurrentNotFav() {
                    SceneToolbarSettings.SetIsFavourite(CurrentScene, false);
                    UpdateView();
                }
            }
            
            Menu.AddItem(
                EditorGUIUtility.TrTextContent("Open Additive"), SceneToolbarSettings.OpenAdditive, () => {
                    SceneToolbarSettings.OpenAdditive = !SceneToolbarSettings.OpenAdditive;
                    UpdateView();
                }
            );
            Menu.AddSeparator(string.Empty);
            
            string[] Paths = GetScenes().ToArray();
            if (Paths.Length == 0) {
                Menu.AddDisabledItem(EditorGUIUtility.TrTextContent("No scenes found."));
                return Menu;
            }

            bool DuplicateNames = Paths.GroupBy(SysPath.GetFileNameWithoutExtension).Any(Group => Group.Count() > 1);

            ISet<string> ActivePaths = SceneDropdown.GetActiveScenes();
            int          ActiveLn    = ActivePaths.Count;
            
            foreach (string Path in SceneToolbarSettings.Favourites.Select(Entry => Entry.Path)) {
                string Name = SysPath.GetFileNameWithoutExtension(Path);
                if (DuplicateNames) {
                    Name = SysPath.GetFileName(Path);
                }

                bool IsActive = ActivePaths.Contains(Path);
                GUIContent Content = EditorGUIUtility.TrTextContent(Name, Path);

                Menu.AddItem(Content, IsActive, OnClick);
                void OnClick() {
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

        void ShowMenu() => GetMenu().ShowAsContext();
        
        static IEnumerable<string> GetScenes() {
            IEnumerable<string> Paths = AssetDatabase.FindAssets("t:SceneAsset")
                .Select(AssetDatabase.GUIDToAssetPath);
            return Paths;
        }

        static Texture2D GetIcon( [LocalizationRequired(false)] string Name ) =>
            (Texture2D)EditorGUIUtility.IconContent(Name).image;

        static Texture2D GetIcon( bool IsFav ) =>
            IsFav
                ? GetIcon("Favorite On Icon")
                : GetIcon("Favorite Icon");

    }
}
