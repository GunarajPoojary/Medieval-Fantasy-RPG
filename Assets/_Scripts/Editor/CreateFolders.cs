using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateFolders : EditorWindow
{
    private static readonly List<string> MainFolders = new List<string>
    {
        "_Scripts",
        "Animations",
        "Asset Store",
        "Audio",
        "Fonts",
        "Materials",
        "Models",
        "Particles",
        "Prefabs",
        "Resources",
        "Scenes",
        "Shaders",
        "Sprites",
        "Textures"
    };

    private static readonly List<string> ScriptsSubFolders = new List<string>
    {
        "UI",
        "Gameplay",
        "Managers",
        "Utilities",
        "Editor"
    };

    private static readonly List<string> AnimationsSubFolders = new List<string>
    {
        "Controllers",
        "Clips"
    };

    [MenuItem("Assets/Create/Default Folders", priority = 1)]
    private static void SetUpFolders()
    {
        CreateFolders window = CreateInstance<CreateFolders>();
        window.position = new Rect(Screen.width / 2, Screen.height / 2, 400, 100);
        window.ShowPopup();
    }

    private static void CreateAllFolders()
    {
        try
        {
            CreateFoldersInDirectory("Assets", MainFolders);
            CreateFoldersInDirectory("Assets/_Scripts", ScriptsSubFolders);
            CreateFoldersInDirectory("Assets/Animations", AnimationsSubFolders);

            AssetDatabase.Refresh();
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to create folders: " + ex.Message);
        }
    }

    // Method to create directories
    private static void CreateFoldersInDirectory(string parentDirectory, List<string> folders)
    {
        foreach (string folder in folders)
        {
            string path = Path.Combine(parentDirectory, folder);
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception ex)
                {
                    Debug.LogError("Failed to create folder at " + path + ": " + ex.Message);
                }
            }
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Create Default Folders");
        GUILayout.Space(10);

        if (GUILayout.Button("Generate!"))
        {
            CreateAllFolders();
            Close();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Close"))
        {
            Close();
        }
    }
}
