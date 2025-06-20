using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class RemoveMissingScripts : EditorWindow
{
    [MenuItem("Tools/Remove All Missing Scripts in Scene")]
    static void RemoveInCurrentScene()
    {
        int count = 0;
        foreach (GameObject go in FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            int removed = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
            if (removed > 0)
                count += removed;
        }
        // Force refresh after removing missing scripts
        AssetDatabase.Refresh();
        EditorSceneManager.MarkAllScenesDirty();

        Debug.Log($"Removed {count} missing script(s) in the current scene.");
    }

    [MenuItem("Tools/Remove All Missing Scripts in Prefabs (Project)")]
    static void RemoveInPrefabs()
    {
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
        int totalRemoved = 0;

        foreach (string guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            int removed = 0;

            foreach (Transform go in instance.GetComponentsInChildren<Transform>(true))
            {
                removed += GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go.gameObject);
            }

            if (removed > 0)
            {
                totalRemoved += removed;
                PrefabUtility.SaveAsPrefabAsset(instance, path);
            }

            GameObject.DestroyImmediate(instance);
        }
        // Force refresh after removing missing scripts
        AssetDatabase.Refresh();
        EditorSceneManager.MarkAllScenesDirty();

        Debug.Log($"Removed {totalRemoved} missing script(s) from prefabs.");
    }
}
