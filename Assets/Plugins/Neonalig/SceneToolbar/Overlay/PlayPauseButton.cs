using System;
using System.Diagnostics;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

namespace Plugins.Neonalig.SceneToolbar.Overlay {
    /// <summary> The play/pause button. </summary>
    [EditorToolbarElement(ID, typeof(SceneView))]
    internal sealed class PlayPauseButton : EditorToolbarButton {
        /// <summary> The ID. </summary>
        internal const string ID = SceneToolbar.ID + ".PlayPause";

        public PlayPauseButton() {
            UpdateIcon();
            clicked += OnClick;
            
            // Register context menu items.
            RegisterCallback<ContextClickEvent>(ContextMenuCallback);
            
            // Listen for changes that make us need to swap to play/pause icons.
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        static void ContextMenuCallback( ContextClickEvent Evt ) {
            GenericMenu Menu = new();
            Menu.AddItem(EditorGUIUtility.TrTextContent("Plays Build Index 0"), SceneToolbarSettings.PlayButtonMethod == PlayButtonMethod.PlayBuild, PlayBuild_Clicked);
            Menu.AddItem(EditorGUIUtility.TrTextContent("Plays the Current Scene"), SceneToolbarSettings.PlayButtonMethod == PlayButtonMethod.PlayCurrent, PlayCurrent_Clicked);
            Menu.ShowAsContext();

            void PlayBuild_Clicked()   => SceneToolbarSettings.PlayButtonMethod = PlayButtonMethod.PlayBuild;
            void PlayCurrent_Clicked() => SceneToolbarSettings.PlayButtonMethod = PlayButtonMethod.PlayCurrent;
        }

        ~PlayPauseButton() {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }
        
        void OnPlayModeStateChanged( PlayModeStateChange State ) {
            UpdateIcon();
        }

        static Texture2D GetIcon( [LocalizationRequired(false)] string Name ) => (Texture2D)EditorGUIUtility.IconContent(Name).image;

        void UpdateIcon() {
            if (IsPlaying) {
                icon    = GetIcon("PlayButton On");
                tooltip = "Stops the main scene.";
                MarkDirtyRepaint();
            } else {
                icon = GetIcon("PlayButton");
                tooltip = SceneToolbarSettings.PlayButtonMethod switch {
                    PlayButtonMethod.PlayBuild   => "Plays the main scene.",
                    PlayButtonMethod.PlayCurrent => "Plays the current scene.",
                    _                            => throw new NotImplementedException()
                };
                MarkDirtyRepaint();
            }
        }

        void OnClick() {
            if (IsPlaying) {
                Stop();
                return;
            }
            
            switch (SceneToolbarSettings.PlayButtonMethod) {
                case PlayButtonMethod.PlayBuild:
                    PlayBuild();
                    break;
                case PlayButtonMethod.PlayCurrent:
                    PlayCurrent();
                    break;
                default:
                    Debug.LogError("Unknown " + nameof(PlayButtonMethod) + " value: " + SceneToolbarSettings.PlayButtonMethod);
                    break;
            }
        }
        
        void PlayBuild() {
            if (EditorBuildSettings.scenes.Length == 0) {
                PlayCurrent();
                return;
            }
            SwapToScene(0, true);
        }

        static int CurrentSceneIndex => SceneManager.GetActiveScene().buildIndex;

        void SwapToScene( int Index, bool Play, OpenSceneMode Mode = OpenSceneMode.Single ) {
            if (Index < 0 || Index >= EditorBuildSettings.scenes.Length) {
                Debug.LogError("Invalid scene index: " + Index);
                return;
            }
            int CurrentIndex = CurrentSceneIndex;
            if (CurrentIndex != Index) {
                EditorSceneManager.OpenScene(EditorBuildSettings.scenes[Index].path, Mode);
            }
            
            if (Play) {
                PlayCurrent();
            }
        }
        
        static bool IsPlaying => EditorApplication.isPlaying;
        
        void PlayCurrent() {
            EditorApplication.isPlaying = true;
            UpdateIcon();
        }
        
        void Stop() {
            EditorApplication.isPlaying = false;
            UpdateIcon();
        }
    }

    internal enum PlayButtonMethod {
        /// <summary> Plays the scene at build index 0. </summary>
        PlayBuild,
        /// <summary> Plays the currently open scene. </summary>
        PlayCurrent
    }
}
