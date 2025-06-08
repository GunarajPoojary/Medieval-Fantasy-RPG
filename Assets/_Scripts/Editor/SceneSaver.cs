#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSaver
{
    [MenuItem("Tools/Save Active Scene %#s")] // Ctrl+Shift+S
    public static void SaveCurrentScene()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        if (activeScene.isDirty)
        {
            EditorSceneManager.SaveScene(activeScene);
            Debug.Log($"Scene saved: {activeScene.name}");
        }
        else
        {
            Debug.Log("Scene is already saved (no changes).");
        }
    }
}
#endif