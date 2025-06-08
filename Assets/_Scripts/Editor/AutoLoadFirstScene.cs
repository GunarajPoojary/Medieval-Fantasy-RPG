using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace EditorTools
{
    /// <summary>
    /// Editor script that automatically loads the first scene in Build Settings when entering Play Mode
    /// and restores the previously opened scene when exiting Play Mode.
    /// </summary>
    [InitializeOnLoad]
    public static class AutoLoadFirstScene
    {
        private const string MENU_PATH = "Tools/Auto Load First Scene";
        private const string ENABLED_PREF_KEY = "AutoLoadFirstScene_Enabled";
        private const string STORED_SCENE_PATH_KEY = "AutoLoadFirstScene_StoredPath";
        
        static AutoLoadFirstScene()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }
        
        /// <summary>
        /// Menu item to toggle the Auto Load First Scene feature.
        /// </summary>
        [MenuItem(MENU_PATH)]
        private static void ToggleAutoLoadFirstScene()
        {
            bool isEnabled = EditorPrefs.GetBool(ENABLED_PREF_KEY, false);
            EditorPrefs.SetBool(ENABLED_PREF_KEY, !isEnabled);
            
            string status = !isEnabled ? "enabled" : "disabled";
            Debug.Log($"Auto Load First Scene {status}.");
        }
        
        /// <summary>
        /// Validates the menu item to show a checkmark when the feature is enabled.
        /// </summary>
        [MenuItem(MENU_PATH, true)]
        private static bool ValidateToggleAutoLoadFirstScene()
        {
            bool isEnabled = EditorPrefs.GetBool(ENABLED_PREF_KEY, false);
            Menu.SetChecked(MENU_PATH, isEnabled);
            return true;
        }
        
        /// <summary>
        /// Handles Play Mode state changes to load/restore scenes appropriately.
        /// </summary>
        /// <param name="state">The current Play Mode state</param>
        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            bool isEnabled = EditorPrefs.GetBool(ENABLED_PREF_KEY, false);
            if (!isEnabled) return;
            
            switch (state)
            {
                case PlayModeStateChange.ExitingEditMode:
                    HandleExitingEditMode();
                    break;
                    
                case PlayModeStateChange.EnteredEditMode:
                    HandleEnteredEditMode();
                    break;
            }
        }
        
        /// <summary>
        /// Handles the transition from Edit Mode to Play Mode.
        /// Stores the current scene path and loads the first scene from Build Settings.
        /// </summary>
        private static void HandleExitingEditMode()
        {
            // Store the currently active scene's path
            Scene currentScene = SceneManager.GetActiveScene();
            string currentScenePath = currentScene.path;
            
            // Handle case where scene might not be saved or might be a new scene
            if (string.IsNullOrEmpty(currentScenePath))
            {
                // For unsaved scenes, we cannot restore them after Play Mode
                if (currentScene.isDirty)
                {
                    Debug.LogWarning("Auto Load First Scene: Current scene has unsaved changes and no saved path. Cannot restore after Play Mode.");
                }
                else
                {
                    Debug.Log("Auto Load First Scene: Current scene is unsaved. Cannot restore after Play Mode.");
                }
                currentScenePath = ""; // Store empty path to indicate no restoration needed
            }
            else
            {
                // Log information about the scene being stored, including whether it's in Build Settings
                bool isInBuildSettings = IsSceneInBuildSettings(currentScenePath);
                string sceneType = isInBuildSettings ? "Build Settings scene" : "external scene (e.g., Addressable)";
                Debug.Log($"Auto Load First Scene: Storing {sceneType}: {currentScenePath}");
            }
            
            EditorPrefs.SetString(STORED_SCENE_PATH_KEY, currentScenePath);
            
            // Get the first scene from Build Settings
            string firstScenePath = GetFirstSceneInBuildSettings();
            if (string.IsNullOrEmpty(firstScenePath))
            {
                Debug.LogWarning("Auto Load First Scene: No scenes found in Build Settings. Cannot load first scene.");
                return;
            }
            
            // Only switch scenes if we're not already in the first scene
            if (currentScenePath != firstScenePath)
            {
                // Check if current scene needs saving
                if (currentScene.isDirty)
                {
                    bool shouldSave = EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                    if (!shouldSave)
                    {
                        // User cancelled the save dialog, prevent entering Play Mode
                        Debug.Log("Auto Load First Scene: Scene change cancelled. Preventing Play Mode entry.");
                        EditorApplication.isPlaying = false;
                        return;
                    }
                }
                
                // Load the first scene
                try
                {
                    EditorSceneManager.OpenScene(firstScenePath);
                    Debug.Log($"Auto Load First Scene: Loaded first scene from Build Settings: {firstScenePath}");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Auto Load First Scene: Failed to load first scene '{firstScenePath}'. Error: {e.Message}");
                }
            }
            else
            {
                Debug.Log("Auto Load First Scene: Already in first scene from Build Settings.");
            }
        }
        
        /// <summary>
        /// Handles when Edit Mode is fully entered after exiting Play Mode.
        /// Restores the previously stored scene if available.
        /// </summary>
        private static void HandleEnteredEditMode()
        {
            string storedScenePath = EditorPrefs.GetString(STORED_SCENE_PATH_KEY, "");
            
            if (string.IsNullOrEmpty(storedScenePath))
            {
                Debug.Log("Auto Load First Scene: No previous scene to restore.");
                return;
            }
            
            // Use EditorApplication.delayCall to ensure scene restoration happens after Edit Mode is fully established
            EditorApplication.delayCall += () => RestoreStoredScene(storedScenePath);
        }
        
        /// <summary>
        /// Restores the previously stored scene.
        /// Works with any scene file, including those not in Build Settings (e.g., Addressable scenes).
        /// </summary>
        /// <param name="scenePath">The path of the scene to restore</param>
        private static void RestoreStoredScene(string scenePath)
        {
            try
            {
                // Check if the scene file still exists
                if (!System.IO.File.Exists(scenePath))
                {
                    Debug.LogWarning($"Auto Load First Scene: Previously opened scene no longer exists: {scenePath}");
                    return;
                }
                
                // Check if we're already in the target scene
                Scene currentScene = SceneManager.GetActiveScene();
                if (currentScene.path == scenePath)
                {
                    Debug.Log("Auto Load First Scene: Already in the target scene.");
                    return;
                }
                
                // Determine scene type for logging
                bool isInBuildSettings = IsSceneInBuildSettings(scenePath);
                string sceneType = isInBuildSettings ? "Build Settings scene" : "external scene ";
                
                // Load the scene - this works for any .unity file regardless of Build Settings inclusion
                EditorSceneManager.OpenScene(scenePath);
                Debug.Log($"Auto Load First Scene: Restored {sceneType}: {scenePath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Auto Load First Scene: Failed to restore scene '{scenePath}'. Error: {e.Message}");
            }
            finally
            {
                // Clear the stored path regardless of success or failure
                EditorPrefs.DeleteKey(STORED_SCENE_PATH_KEY);
            }
        }
        
        /// <summary>
        /// Checks if a scene path exists in the current Build Settings.
        /// </summary>
        /// <param name="scenePath">The scene path to check</param>
        /// <returns>True if the scene is included in Build Settings, false otherwise</returns>
        private static bool IsSceneInBuildSettings(string scenePath)
        {
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            if (scenes == null || scenes.Length == 0)
                return false;
                
            for (int i = 0; i < scenes.Length; i++)
            {
                if (scenes[i].path == scenePath)
                    return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Gets the path of the first scene in the Build Settings.
        /// </summary>
        /// <returns>The path of the first scene, or null if no scenes are configured</returns>
        private static string GetFirstSceneInBuildSettings()
        {
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            
            if (scenes == null || scenes.Length == 0)
            {
                return null;
            }
            
            // Find the first enabled scene
            for (int i = 0; i < scenes.Length; i++)
            {
                if (scenes[i].enabled)
                {
                    return scenes[i].path;
                }
            }
            
            // If no enabled scenes found, return the first scene regardless of enabled state
            return scenes[0].path;
        }
    }
}