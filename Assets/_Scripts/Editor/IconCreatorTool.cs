using UnityEngine;
using UnityEditor;
using System.IO;

public class CustomToolsEditorWindow : EditorWindow
{
    [Header("Icon Settings")]
    public GameObject targetObject;
    public int iconSize = 256;
    public string fileName = "icon";
    public string savePath = "Assets/Icons/";

    [Header("Camera Settings")]
    public Vector3 cameraPosition = new Vector3(0, 0, -5);
    public Vector3 cameraRotation = new Vector3(0, 0, 0);
    public float fieldOfView = 60f;
    public Color backgroundColor = Color.clear;

    [Header("Lighting")]
    public bool useCustomLighting = true;
    public Color lightColor = Color.white;
    public float lightIntensity = 1f;
    public Vector3 lightDirection = new Vector3(-30, 30, 0);

    private Camera iconCamera;
    private Light iconLight;
    private RenderTexture renderTexture;

    [MenuItem("Tools/Custom/Icon Creator")]
    public static void ShowWindow()
    {
        GetWindow<CustomToolsEditorWindow>("Icon Creator");
    }

    void OnGUI()
    {
        GUILayout.Label("Unity Icon Creator", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Icon Settings Section
        EditorGUILayout.LabelField("Icon Settings", EditorStyles.boldLabel);
        targetObject = (GameObject)EditorGUILayout.ObjectField("Target Object", targetObject, typeof(GameObject), true);
        iconSize = EditorGUILayout.IntSlider("Icon Size", iconSize, 64, 1024);
        fileName = EditorGUILayout.TextField("File Name", fileName);
        savePath = EditorGUILayout.TextField("Save Path", savePath);

        EditorGUILayout.Space();

        // Camera Settings Section
        EditorGUILayout.LabelField("Camera Settings", EditorStyles.boldLabel);
        cameraPosition = EditorGUILayout.Vector3Field("Camera Position", cameraPosition);
        cameraRotation = EditorGUILayout.Vector3Field("Camera Rotation", cameraRotation);
        fieldOfView = EditorGUILayout.Slider("Field of View", fieldOfView, 10f, 120f);
        backgroundColor = EditorGUILayout.ColorField("Background Color", backgroundColor);

        EditorGUILayout.Space();

        // Lighting Settings Section
        EditorGUILayout.LabelField("Lighting Settings", EditorStyles.boldLabel);
        useCustomLighting = EditorGUILayout.Toggle("Use Custom Lighting", useCustomLighting);

        if (useCustomLighting)
        {
            lightColor = EditorGUILayout.ColorField("Light Color", lightColor);
            lightIntensity = EditorGUILayout.Slider("Light Intensity", lightIntensity, 0f, 3f);
            lightDirection = EditorGUILayout.Vector3Field("Light Direction", lightDirection);
        }

        EditorGUILayout.Space();

        // Action Buttons
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Setup Preview"))
        {
            SetupPreviewCamera();
        }
        if (GUILayout.Button("Generate Icon"))
        {
            GenerateIcon();
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Cleanup Preview"))
        {
            CleanupPreview();
        }

        EditorGUILayout.Space();

        // Help Section
        EditorGUILayout.HelpBox("1. Select a GameObject from the scene\n2. Adjust camera and lighting settings\n3. Use 'Setup Preview' to position the camera\n4. Click 'Generate Icon' to create the PNG", MessageType.Info);
    }

    void SetupPreviewCamera()
    {
        if (targetObject == null)
        {
            EditorUtility.DisplayDialog("Error", "Please select a target object first.", "OK");
            return;
        }

        CleanupPreview();

        // Create camera for preview
        GameObject cameraObj = new GameObject("Icon Preview Camera");
        iconCamera = cameraObj.AddComponent<Camera>();
        iconCamera.transform.position = cameraPosition;
        iconCamera.transform.eulerAngles = cameraRotation; // Preserve user-defined rotation
        iconCamera.fieldOfView = fieldOfView;
        iconCamera.backgroundColor = backgroundColor;
        iconCamera.clearFlags = CameraClearFlags.SolidColor;

        // Setup lighting if enabled
        if (useCustomLighting)
        {
            GameObject lightObj = new GameObject("Icon Light");
            iconLight = lightObj.AddComponent<Light>();
            iconLight.type = LightType.Directional;
            iconLight.color = lightColor;
            iconLight.intensity = lightIntensity;
            iconLight.transform.eulerAngles = lightDirection;
        }

        // Focus scene view on the camera
        Selection.activeGameObject = cameraObj;
        SceneView.FrameLastActiveSceneView();

        Debug.Log("Preview camera setup complete. Adjust position as needed, then generate the icon.");
    }

    void GenerateIcon()
    {
        if (targetObject == null)
        {
            EditorUtility.DisplayDialog("Error", "Please select a target object first.", "OK");
            return;
        }

        if (iconCamera == null)
        {
            EditorUtility.DisplayDialog("Error", "Please setup preview camera first.", "OK");
            return;
        }

        // Create directory if it doesn't exist
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        // Create render texture
        renderTexture = new RenderTexture(iconSize, iconSize, 24, RenderTextureFormat.ARGB32);
        renderTexture.antiAliasing = 4;

        // Configure camera for final render
        Camera tempCamera = iconCamera;
        tempCamera.targetTexture = renderTexture;
        tempCamera.backgroundColor = Color.clear;
        tempCamera.clearFlags = CameraClearFlags.SolidColor;

        // Render the icon
        tempCamera.Render();

        // Convert to Texture2D
        RenderTexture.active = renderTexture;
        Texture2D iconTexture = new Texture2D(iconSize, iconSize, TextureFormat.ARGB32, false);
        iconTexture.ReadPixels(new Rect(0, 0, iconSize, iconSize), 0, 0);
        iconTexture.Apply();

        // Save as PNG
        byte[] pngData = iconTexture.EncodeToPNG();
        string fullPath = Path.Combine(savePath, fileName + ".png");
        File.WriteAllBytes(fullPath, pngData);

        // Cleanup
        RenderTexture.active = null;
        tempCamera.targetTexture = null;
        DestroyImmediate(renderTexture);
        DestroyImmediate(iconTexture);

        // Refresh asset database
        AssetDatabase.Refresh();

        // Configure import settings for transparency
        ConfigureTextureImportSettings(fullPath);

        EditorUtility.DisplayDialog("Success", $"Icon saved to: {fullPath}", "OK");

        // Ping the created asset
        Object createdAsset = AssetDatabase.LoadAssetAtPath<Texture2D>(fullPath);
        EditorGUIUtility.PingObject(createdAsset);
    }

    void ConfigureTextureImportSettings(string assetPath)
    {
        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.alphaSource = TextureImporterAlphaSource.FromInput;
            importer.alphaIsTransparency = true;
            importer.npotScale = TextureImporterNPOTScale.None;
            importer.SaveAndReimport();
        }
    }

    Vector3 GetObjectCenter(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            return renderer.bounds.center;
        }

        // If no renderer, use transform position
        return obj.transform.position;
    }

    void CleanupPreview()
    {
        if (iconCamera != null)
        {
            DestroyImmediate(iconCamera.gameObject);
            iconCamera = null;
        }

        if (iconLight != null)
        {
            DestroyImmediate(iconLight.gameObject);
            iconLight = null;
        }

        if (renderTexture != null)
        {
            DestroyImmediate(renderTexture);
            renderTexture = null;
        }
    }

    void OnDestroy()
    {
        CleanupPreview();
    }
}

// Additional utility class for batch icon generation
public static class IconBatchProcessor
{
    [MenuItem("Tools/Generate Icons for Selected Objects")]
    public static void GenerateIconsForSelection()
    {
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length == 0)
        {
            EditorUtility.DisplayDialog("Error", "Please select one or more GameObjects first.", "OK");
            return;
        }

        string savePath = EditorUtility.SaveFolderPanel("Select Save Folder", "Assets", "");
        if (string.IsNullOrEmpty(savePath))
            return;

        // Convert absolute path to relative path
        savePath = "Assets" + savePath.Substring(Application.dataPath.Length);

        for (int i = 0; i < selectedObjects.Length; i++)
        {
            GameObject obj = selectedObjects[i];
            string progress = $"Generating icon {i + 1}/{selectedObjects.Length}: {obj.name}";
            EditorUtility.DisplayProgressBar("Generating Icons", progress, (float)i / selectedObjects.Length);

            GenerateSingleIcon(obj, savePath);
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Complete", $"Generated {selectedObjects.Length} icons successfully.", "OK");
    }

    private static void GenerateSingleIcon(GameObject targetObject, string savePath)
    {
        // Create temporary camera
        GameObject cameraObj = new GameObject("Temp Icon Camera");
        Camera iconCamera = cameraObj.AddComponent<Camera>();

        // Position camera to frame object
        Vector3 objectCenter = GetObjectCenter(targetObject);
        Vector3 cameraPos = objectCenter + new Vector3(0, 0, -5);
        iconCamera.transform.position = cameraPos;
        iconCamera.transform.LookAt(objectCenter);

        iconCamera.fieldOfView = 60f;
        iconCamera.backgroundColor = Color.clear;
        iconCamera.clearFlags = CameraClearFlags.SolidColor;

        // Create render texture
        RenderTexture renderTexture = new RenderTexture(256, 256, 24, RenderTextureFormat.ARGB32);
        renderTexture.antiAliasing = 4;
        iconCamera.targetTexture = renderTexture;

        // Render
        iconCamera.Render();

        // Convert to Texture2D
        RenderTexture.active = renderTexture;
        Texture2D iconTexture = new Texture2D(256, 256, TextureFormat.ARGB32, false);
        iconTexture.ReadPixels(new Rect(0, 0, 256, 256), 0, 0);
        iconTexture.Apply();

        // Save PNG
        byte[] pngData = iconTexture.EncodeToPNG();
        string fileName = targetObject.name + "_icon.png";
        string fullPath = Path.Combine(savePath, fileName);
        File.WriteAllBytes(fullPath, pngData);

        // Cleanup
        RenderTexture.active = null;
        Object.DestroyImmediate(renderTexture);
        Object.DestroyImmediate(iconTexture);
        Object.DestroyImmediate(cameraObj);
    }

    private static Vector3 GetObjectCenter(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            return renderer.bounds.center;
        }
        return obj.transform.position;
    }
}