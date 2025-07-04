using UnityEngine;
using UnityEditor;
using System.IO;

// Editor window for creating ScriptableObjects
public class ScriptableObjectCreationWindow : EditorWindow
{
    private SerializedProperty targetProperty;
    private System.Type scriptableObjectType;
    private string assetName = "New ScriptableObject";
    private string selectedPath = "Assets/Game";

    public static void ShowWindow(SerializedProperty property, System.Type soType)
    {
        ScriptableObjectCreationWindow window = GetWindow<ScriptableObjectCreationWindow>(true, "Create ScriptableObject", true);
        window.targetProperty = property;
        window.scriptableObjectType = soType;
        window.assetName = soType.Name;
        window.selectedPath = "Assets/Game";
        window.minSize = new Vector2(400, 150);
        window.maxSize = new Vector2(400, 150);
        window.Show();
    }

    void OnGUI()
    {
        if (targetProperty == null || scriptableObjectType == null)
        {
            Close();
            return;
        }

        GUILayout.Space(10);
        
        // Title
        GUILayout.Label($"Create {scriptableObjectType.Name}", EditorStyles.boldLabel);
        
        GUILayout.Space(10);
        
        // Asset name field
        GUILayout.BeginHorizontal();
        GUILayout.Label("Name:", GUILayout.Width(80));
        assetName = EditorGUILayout.TextField(assetName);
        GUILayout.EndHorizontal();
        
        GUILayout.Space(5);
        
        // Directory selection
        GUILayout.BeginHorizontal();
        GUILayout.Label("Directory:", GUILayout.Width(80));
        EditorGUILayout.TextField(selectedPath);
        if (GUILayout.Button("Browse", GUILayout.Width(60)))
        {
            string newPath = EditorUtility.OpenFolderPanel("Select Directory", selectedPath, "");
            if (!string.IsNullOrEmpty(newPath))
            {
                // Convert absolute path to relative path
                if (newPath.StartsWith(Application.dataPath))
                {
                    selectedPath = "Assets" + newPath.Substring(Application.dataPath.Length);
                }
                else
                {
                    selectedPath = newPath;
                }
            }
        }
        GUILayout.EndHorizontal();
        
        GUILayout.Space(15);
        
        // Buttons
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        
        if (GUILayout.Button("Cancel", GUILayout.Width(80)))
        {
            Close();
        }
        
        if (GUILayout.Button("Create", GUILayout.Width(80)))
        {
            CreateScriptableObject();
        }
        
        GUILayout.EndHorizontal();
        
        GUILayout.Space(10);
    }

    private void CreateScriptableObject()
    {
        if (string.IsNullOrEmpty(assetName))
        {
            EditorUtility.DisplayDialog("Error", "Please provide a name for the ScriptableObject.", "OK");
            return;
        }

        // Ensure the directory exists
        if (!Directory.Exists(selectedPath))
        {
            Directory.CreateDirectory(selectedPath);
            AssetDatabase.Refresh();
        }

        // Create the ScriptableObject instance
        ScriptableObject instance = ScriptableObject.CreateInstance(scriptableObjectType);
        
        // Generate unique asset path
        string assetPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(selectedPath, assetName + ".asset"));
        
        // Create the asset
        AssetDatabase.CreateAsset(instance, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        // Assign the created asset to the property
        targetProperty.objectReferenceValue = instance;
        targetProperty.serializedObject.ApplyModifiedProperties();
        
        // Select the created asset in the project window
        EditorGUIUtility.PingObject(instance);
        Selection.activeObject = instance;
        
        Close();
    }
}