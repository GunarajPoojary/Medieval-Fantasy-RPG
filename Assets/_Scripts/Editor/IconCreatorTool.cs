using UnityEngine;
using UnityEditor;
using System.IO;

public class IconCreatorTool : EditorWindow
{
    [Header("Icon Settings")]
    public GameObject targetObject;
    public int iconSize = 2048;
    public string fileName = "Icon";
    public string savePath = "Assets/Game/Sprites/Icons/Items/";

    [Header("Camera Settings")]
    public bool useOrthographicCamera = false;
    public Vector3 cameraPosition = new Vector3(0, 0, -5);
    public Vector3 cameraRotation = new Vector3(0, 0, 0);
    public float fieldOfView = 60f;
    public float orthographicSize = 5f;
    public Color backgroundColor = Color.clear;

    [Header("Lighting")]
    public bool useCustomLighting = true;
    public Color lightColor = Color.white;
    public float lightIntensity = 1f;
    public Vector3 lightDirection = new Vector3(-30, 30, 0);

    private Camera iconCamera;
    private Light iconLight;
    private RenderTexture renderTexture;
    private Vector2 scrollPosition;
    private Texture2D previewTexture;
    private bool showPreview = false;
    private RenderTexture previewRenderTexture;


    [MenuItem("Tools/Custom/Icon Creator")]
    public static void ShowWindow()
    {
        GetWindow<IconCreatorTool>("Icon Creator");
    }

    private void OnEnable()
    {
        minSize = new Vector2(350, 500);
        maxSize = new Vector2(400, 800);
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        GUILayout.Label("Unity Icon Creator", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Icon Settings Section
        EditorGUILayout.LabelField("Icon Settings", EditorStyles.boldLabel);
        targetObject = (GameObject)EditorGUILayout.ObjectField("Target Object", targetObject, typeof(GameObject), true);
        iconSize = EditorGUILayout.IntSlider("Icon Size", iconSize, 64, 2048);
        fileName = EditorGUILayout.TextField("File Name", fileName);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Save Path", GUILayout.Width(EditorGUIUtility.labelWidth));
        savePath = EditorGUILayout.TextField(savePath);
        if (GUILayout.Button("Browse", GUILayout.Width(60)))
        {
            string selectedPath = EditorUtility.OpenFolderPanel("Select Save Directory", savePath, "");
            if (!string.IsNullOrEmpty(selectedPath))
            {
                savePath = "Assets" + selectedPath.Substring(Application.dataPath.Length);
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // Camera Settings Section
        EditorGUILayout.LabelField("Camera Settings", EditorStyles.boldLabel);
        useOrthographicCamera = EditorGUILayout.Toggle("Use Orthographic Camera", useOrthographicCamera);
        cameraPosition = EditorGUILayout.Vector3Field("Camera Position", cameraPosition);
        cameraRotation = EditorGUILayout.Vector3Field("Camera Rotation", cameraRotation);

        if (useOrthographicCamera)
        {
            orthographicSize = EditorGUILayout.Slider("Orthographic Size", orthographicSize, 0.1f, 20f);
        }
        else
        {
            fieldOfView = EditorGUILayout.Slider("Field of View", fieldOfView, 10f, 120f);
        }

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

        // Action Buttons with improved layout
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Setup Preview", GUILayout.Height(25)))
        {
            SetupPreviewCamera();
        }
        if (GUILayout.Button("Generate Icon", GUILayout.Height(25)))
        {
            GenerateIcon();
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Cleanup Preview", GUILayout.Height(25)))
        {
            CleanupPreview();
        }

        EditorGUILayout.Space();
        // Preview Section (add this after Action Buttons, before Help Section)
        if (showPreview && previewTexture != null)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);

            Rect previewRect = GUILayoutUtility.GetRect(256, 256, GUILayout.ExpandWidth(false));
            previewRect.width = 256;
            previewRect.height = 256;

            GUI.DrawTexture(previewRect, previewTexture, ScaleMode.ScaleToFit, true);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Refresh Preview"))
            {
                UpdatePreview();
            }
            if (GUILayout.Button("Hide Preview"))
            {
                showPreview = false;
            }
            EditorGUILayout.EndHorizontal();
        }
        // Help Section with improved formatting
        string helpText = useOrthographicCamera
            ? "1. Select a GameObject from the scene\n2. Adjust camera and lighting settings\n3. Use 'Setup Preview' to position the camera\n4. Adjust Orthographic Size to frame the object\n5. Click 'Generate Icon' to create the PNG"
            : "1. Select a GameObject from the scene\n2. Adjust camera and lighting settings\n3. Use 'Setup Preview' to position the camera\n4. Click 'Generate Icon' to create the PNG";

        EditorGUILayout.HelpBox(helpText, MessageType.Info);

        EditorGUILayout.EndScrollView();
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

        // Configure camera projection
        iconCamera.orthographic = useOrthographicCamera;
        if (useOrthographicCamera)
        {
            iconCamera.orthographicSize = orthographicSize;
        }
        else
        {
            iconCamera.fieldOfView = fieldOfView;
        }

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
        // Add this line before the Debug.Log
        UpdatePreview();
        Debug.Log("Preview camera setup complete. Adjust position as needed, then generate the icon.");
    }

    void UpdatePreview()
    {
        if (iconCamera == null || targetObject == null) return;

        // Create or update preview render texture
        if (previewRenderTexture == null)
        {
            previewRenderTexture = new RenderTexture(256, 256, 24, RenderTextureFormat.ARGB32);
        }

        // Store original camera settings
        RenderTexture originalTarget = iconCamera.targetTexture;
        Color originalBackground = iconCamera.backgroundColor;

        // Configure camera for preview
        iconCamera.targetTexture = previewRenderTexture;
        iconCamera.backgroundColor = backgroundColor;
        iconCamera.Render();

        // Convert to Texture2D for display
        RenderTexture.active = previewRenderTexture;
        if (previewTexture == null)
        {
            previewTexture = new Texture2D(256, 256, TextureFormat.RGBA32, false);
        }
        previewTexture.ReadPixels(new Rect(0, 0, 256, 256), 0, 0);
        previewTexture.Apply();

        // Restore camera settings
        iconCamera.targetTexture = originalTarget;
        iconCamera.backgroundColor = originalBackground;
        RenderTexture.active = null;

        showPreview = true;
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

        // Add these cleanup lines
        if (previewRenderTexture != null)
        {
            DestroyImmediate(previewRenderTexture);
            previewRenderTexture = null;
        }

        if (previewTexture != null)
        {
            DestroyImmediate(previewTexture);
            previewTexture = null;
        }

        showPreview = false;
    }

    void OnDestroy()
    {
        CleanupPreview();
    }
}