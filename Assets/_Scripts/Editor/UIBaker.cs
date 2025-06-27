using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Editor tool for consolidating multiple UI elements into a single optimized sprite and GameObject
/// Enhanced with pixel-perfect dimension calculation based on actual UI element bounds
/// </summary>
public class UIBaker : EditorWindow
{
    private GameObject targetUIParent;
    private string outputPath = "Assets/Generated/ConsolidatedUI/";
    private string assetName = "ConsolidatedUI";
    private int textureWidth = 2048;
    private int textureHeight = 2048;
    private bool includeInactiveElements = false;
    private bool preserveOriginalHierarchy = true;
    private bool useNativeResolution = false;

    private Vector2 scrollPosition;
    private Vector2 elementListScrollPosition;
    private List<UIElementData> detectedElements = new List<UIElementData>();
    private bool showPreview = true;
    private Texture2D previewTexture;
    private Rect originalCanvasBounds;
    private Vector2 calculatedPixelDimensions; // Store calculated pixel dimensions

    [System.Serializable]
    private class UIElementData
    {
        public GameObject gameObject;
        public Component component;
        public Rect screenRect;
        public bool includeInConsolidation = true;
        public UIElementType elementType;

        public enum UIElementType
        {
            Image,
            Text,
            RawImage,
            Button,
            Other
        }
    }

    [MenuItem("Tools/Custom/UGUI/UIBaker")]
    public static void ShowWindow()
    {
        UIBaker window = GetWindow<UIBaker>("UI Baker");
        window.minSize = new Vector2(400, 600);
        window.Show();
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        EditorGUILayout.LabelField("UI Consolidation Tool", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        DrawConfigurationSection();
        EditorGUILayout.Space();

        DrawDetectionSection();
        EditorGUILayout.Space();

        DrawElementListSection();
        EditorGUILayout.Space();

        DrawPreviewSection();
        EditorGUILayout.Space();

        DrawConsolidationSection();

        EditorGUILayout.EndScrollView();
    }

    private void OnInspectorUpdate()
    {
        // Force repaint when window is resized to ensure proper scaling
        if (Event.current != null && Event.current.type == EventType.Layout)
        {
            Repaint();
        }
    }

    private void DrawConfigurationSection()
    {
        EditorGUILayout.LabelField("Configuration", EditorStyles.boldLabel);

        targetUIParent = (GameObject)EditorGUILayout.ObjectField(
            "Target UI Parent", targetUIParent, typeof(GameObject), true);

        outputPath = EditorGUILayout.TextField("Output Path", outputPath);
        assetName = EditorGUILayout.TextField("Asset Name", assetName);

        EditorGUILayout.BeginHorizontal();
        textureWidth = EditorGUILayout.IntField("Texture Width", textureWidth, GUILayout.MinWidth(100));
        textureHeight = EditorGUILayout.IntField("Texture Height", textureHeight, GUILayout.MinWidth(100));
        EditorGUILayout.EndHorizontal();

        useNativeResolution = EditorGUILayout.Toggle("Use Pixel-Perfect Dimensions", useNativeResolution);
        if (useNativeResolution)
        {
            EditorGUILayout.HelpBox("Pixel-perfect mode calculates texture dimensions based on actual UI element pixel bounds for precise scaling.", MessageType.Info);

            if (calculatedPixelDimensions.x > 0 && calculatedPixelDimensions.y > 0)
            {
                EditorGUILayout.LabelField($"Calculated Dimensions: {calculatedPixelDimensions.x:F0} x {calculatedPixelDimensions.y:F0} pixels");
            }
        }

        includeInactiveElements = EditorGUILayout.Toggle("Include Inactive Elements", includeInactiveElements);
        preserveOriginalHierarchy = EditorGUILayout.Toggle("Preserve Original Hierarchy", preserveOriginalHierarchy);
    }

    private void DrawDetectionSection()
    {
        EditorGUILayout.LabelField("Element Detection", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Scan UI Elements", GUILayout.MinWidth(100), GUILayout.ExpandWidth(true)))
        {
            ScanUIElements();
        }

        if (GUILayout.Button("Clear List", GUILayout.MinWidth(80), GUILayout.ExpandWidth(true)))
        {
            detectedElements.Clear();
            ClearPreview();
            calculatedPixelDimensions = Vector2.zero;
        }
        EditorGUILayout.EndHorizontal();

        if (detectedElements.Count > 0)
        {
            EditorGUILayout.LabelField($"Detected {detectedElements.Count} UI elements");
        }
    }

    private void DrawElementListSection()
    {
        if (detectedElements.Count == 0) return;

        EditorGUILayout.LabelField("Detected Elements", EditorStyles.boldLabel);

        elementListScrollPosition = EditorGUILayout.BeginScrollView(elementListScrollPosition, GUILayout.Height(120));

        for (int i = 0; i < detectedElements.Count; i++)
        {
            UIElementData element = detectedElements[i];

            EditorGUILayout.BeginHorizontal();

            element.includeInConsolidation = EditorGUILayout.Toggle(element.includeInConsolidation, GUILayout.Width(20));

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(element.gameObject, typeof(GameObject), true, GUILayout.MinWidth(100), GUILayout.ExpandWidth(true));
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.LabelField(element.elementType.ToString(), GUILayout.Width(60));
            EditorGUILayout.LabelField($"{element.screenRect.width:F0}x{element.screenRect.height:F0}", GUILayout.Width(80));

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Select All"))
        {
            foreach (UIElementData element in detectedElements)
                element.includeInConsolidation = true;
        }

        if (GUILayout.Button("Select None"))
        {
            foreach (UIElementData element in detectedElements)
                element.includeInConsolidation = false;
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawPreviewSection()
    {
        EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);

        showPreview = EditorGUILayout.Toggle("Show Preview", showPreview);

        EditorGUILayout.BeginHorizontal();
        if (previewTexture != null && showPreview)
        {
            float aspectRatio = (float)previewTexture.width / previewTexture.height;
            float availableWidth = EditorGUIUtility.currentViewWidth - 40;
            float maxPreviewWidth = Mathf.Min(400, availableWidth);
            float previewWidth = Mathf.Max(200, maxPreviewWidth);
            float previewHeight = previewWidth / aspectRatio;

            GUILayout.Space(5);
            EditorGUILayout.LabelField("Preview Texture:", EditorStyles.boldLabel);

            Rect previewRect = EditorGUILayout.GetControlRect(false, previewHeight);

            // Center the preview if the window is wider than the preview
            if (previewRect.width > previewWidth)
            {
                float offset = (previewRect.width - previewWidth) * 0.5f;
                previewRect.x += offset;
                previewRect.width = previewWidth;
            }

            EditorGUI.DrawPreviewTexture(previewRect, previewTexture);
        }
        EditorGUILayout.EndHorizontal();

        if (previewTexture != null && showPreview)
        {
            float aspectRatio = (float)previewTexture.width / previewTexture.height;
            float previewWidth = Mathf.Min(300, EditorGUIUtility.currentViewWidth - 60);
            float previewHeight = previewWidth / aspectRatio;

            GUILayout.Space(5);
            EditorGUILayout.LabelField("Preview Texture:", EditorStyles.boldLabel);

            Rect previewRect = EditorGUILayout.GetControlRect(false, previewHeight);
            EditorGUI.DrawPreviewTexture(previewRect, previewTexture);
        }
    }

    private void DrawConsolidationSection()
    {
        EditorGUILayout.LabelField("Consolidation", EditorStyles.boldLabel);

        EditorGUI.BeginDisabledGroup(detectedElements.Count == 0 || targetUIParent == null);

        if (GUILayout.Button("Consolidate UI Elements", GUILayout.Height(30)))
        {
            ConsolidateUIElements();
        }

        EditorGUI.EndDisabledGroup();

        if (targetUIParent == null)
        {
            EditorGUILayout.HelpBox("Please assign a Target UI Parent to proceed with consolidation.", MessageType.Warning);
        }
        else if (detectedElements.Count == 0)
        {
            EditorGUILayout.HelpBox("Please scan for UI elements first.", MessageType.Warning);
        }
    }

    private void ScanUIElements()
    {
        if (targetUIParent == null)
        {
            EditorUtility.DisplayDialog("Error", "Please assign a Target UI Parent first.", "OK");
            return;
        }

        detectedElements.Clear();

        Component[] components = targetUIParent.GetComponentsInChildren<Component>(includeInactiveElements);
        foreach (Component comp in components)
        {
            if (comp == null || comp.gameObject == null)
                continue;

            if (IsUIElement(comp))
            {
                UIElementData elementData = CreateElementData(comp);
                if (elementData != null)
                {
                    detectedElements.Add(elementData);
                }
            }
        }

        // Calculate pixel dimensions after scanning
        CalculatePixelDimensions();

        Debug.Log($"Scanned and found {detectedElements.Count} UI elements.");
        if (useNativeResolution)
        {
            Debug.Log($"Calculated pixel dimensions: {calculatedPixelDimensions.x} x {calculatedPixelDimensions.y}");
        }
    }

    private bool IsUIElement(Component component)
    {
        return component is Image || component is Text || component is RawImage ||
               component is Button || component is Slider || component is Toggle;
    }

    private UIElementData CreateElementData(Component component)
    {
        RectTransform rectTransform = component.GetComponent<RectTransform>();
        if (rectTransform == null) return null;

        UIElementData elementData = new UIElementData
        {
            gameObject = component.gameObject,
            component = component,
            screenRect = GetScreenRect(rectTransform)
        };

        if (component is Image) elementData.elementType = UIElementData.UIElementType.Image;
        else if (component is Text) elementData.elementType = UIElementData.UIElementType.Text;
        else if (component is RawImage) elementData.elementType = UIElementData.UIElementType.RawImage;
        else if (component is Button) elementData.elementType = UIElementData.UIElementType.Button;
        else elementData.elementType = UIElementData.UIElementType.Other;

        return elementData;
    }

    private Rect GetScreenRect(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        Vector2 min = corners[0];
        Vector2 max = corners[2];

        return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
    }

    /// <summary>
    /// Calculate pixel-perfect dimensions based on actual UI element bounds
    /// This mimics Unity's Sprite Editor automatic slice functionality
    /// </summary>
    private void CalculatePixelDimensions()
    {
        if (detectedElements.Count == 0)
        {
            calculatedPixelDimensions = Vector2.zero;
            return;
        }

        Canvas canvas = targetUIParent.GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            calculatedPixelDimensions = Vector2.zero;
            return;
        }

        // Calculate the actual content bounds in canvas space
        Rect contentBounds = CalculateContentBounds();

        if (contentBounds.width <= 0 || contentBounds.height <= 0)
        {
            calculatedPixelDimensions = Vector2.zero;
            return;
        }

        // Get the canvas scale factor for pixel conversion
        CanvasScaler canvasScaler = canvas.GetComponent<CanvasScaler>();
        float scaleFactor = GetCanvasScaleFactor(canvas, canvasScaler);

        // Convert canvas units to actual pixels
        float pixelWidth = contentBounds.width * scaleFactor;
        float pixelHeight = contentBounds.height * scaleFactor;

        // Round to nearest integer pixels and ensure minimum size
        calculatedPixelDimensions = new Vector2(
            Mathf.Max(1, Mathf.RoundToInt(pixelWidth)),
            Mathf.Max(1, Mathf.RoundToInt(pixelHeight))
        );

        Debug.Log($"Content bounds: {contentBounds}, Scale factor: {scaleFactor}, Final pixel dimensions: {calculatedPixelDimensions}");
    }

    /// <summary>
    /// Calculate tight bounds around actual UI content
    /// </summary>
    private Rect CalculateContentBounds()
    {
        float minX = float.MaxValue;
        float minY = float.MaxValue;
        float maxX = float.MinValue;
        float maxY = float.MinValue;

        bool boundsFound = false;
        Canvas canvas = targetUIParent.GetComponentInParent<Canvas>();
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        foreach (UIElementData element in detectedElements)
        {
            if (element.includeInConsolidation && element.gameObject.activeInHierarchy)
            {
                RectTransform rectTransform = element.gameObject.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    // Get the actual rendered bounds of the element
                    Bounds renderedBounds = GetRenderedBounds(rectTransform);

                    Vector2 min = new Vector2(renderedBounds.min.x, renderedBounds.min.y);
                    Vector2 max = new Vector2(renderedBounds.max.x, renderedBounds.max.y);

                    minX = Mathf.Min(minX, min.x);
                    minY = Mathf.Min(minY, min.y);
                    maxX = Mathf.Max(maxX, max.x);
                    maxY = Mathf.Max(maxY, max.y);

                    boundsFound = true;
                }
            }
        }

        if (!boundsFound)
        {
            return new Rect(0, 0, 100, 100); // Fallback size
        }

        // Add minimal padding for edge precision
        float padding = 1f;
        return new Rect(
            minX - padding,
            minY - padding,
            (maxX - minX) + (padding * 2),
            (maxY - minY) + (padding * 2)
        );
    }

    /// <summary>
    /// Get the actual rendered bounds of a UI element
    /// </summary>
    private Bounds GetRenderedBounds(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        Vector3 min = corners[0];
        Vector3 max = corners[0];

        for (int i = 1; i < 4; i++)
        {
            min = Vector3.Min(min, corners[i]);
            max = Vector3.Max(max, corners[i]);
        }

        return new Bounds((min + max) * 0.5f, max - min);
    }

    /// <summary>
    /// Get the effective canvas scale factor for pixel calculations
    /// </summary>
    private float GetCanvasScaleFactor(Canvas canvas, CanvasScaler canvasScaler)
    {
        if (canvasScaler == null)
        {
            return 1f; // No scaling
        }

        switch (canvasScaler.uiScaleMode)
        {
            case CanvasScaler.ScaleMode.ConstantPixelSize:
                return canvasScaler.scaleFactor;

            case CanvasScaler.ScaleMode.ScaleWithScreenSize:
                Vector2 referenceResolution = canvasScaler.referenceResolution;
                Vector2 screenSize = new Vector2(Screen.width, Screen.height);

                float logWidth = Mathf.Log(screenSize.x / referenceResolution.x, 2);
                float logHeight = Mathf.Log(screenSize.y / referenceResolution.y, 2);
                float logWeightedAverage = Mathf.Lerp(logWidth, logHeight, canvasScaler.matchWidthOrHeight);

                return Mathf.Pow(2, logWeightedAverage);

            case CanvasScaler.ScaleMode.ConstantPhysicalSize:
                return Screen.dpi / canvasScaler.fallbackScreenDPI;

            default:
                return 1f;
        }
    }

    private void GeneratePreview()
    {
        if (detectedElements.Count == 0) return;

        ClearPreview();
        previewTexture = GenerateConsolidatedTexture();
        Repaint();
    }

    /// <summary>
    /// Calculate canvas-relative bounds for all selected UI elements
    /// </summary>
    private Rect CalculateCanvasBounds()
    {
        float minX = float.MaxValue;
        float minY = float.MaxValue;
        float maxX = float.MinValue;
        float maxY = float.MinValue;

        bool boundsFound = false;

        Canvas canvas = targetUIParent.GetComponentInParent<Canvas>();
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        foreach (UIElementData element in detectedElements)
        {
            if (element.includeInConsolidation && element.gameObject.activeInHierarchy)
            {
                RectTransform rectTransform = element.gameObject.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    Vector3[] corners = new Vector3[4];
                    rectTransform.GetWorldCorners(corners);

                    foreach (Vector3 corner in corners)
                    {
                        Vector2 localPoint;
                        RectTransformUtility.ScreenPointToLocalPointInRectangle(
                            canvasRect,
                            RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, corner),
                            canvas.worldCamera,
                            out localPoint
                        );

                        minX = Mathf.Min(minX, localPoint.x);
                        minY = Mathf.Min(minY, localPoint.y);
                        maxX = Mathf.Max(maxX, localPoint.x);
                        maxY = Mathf.Max(maxY, localPoint.y);

                        boundsFound = true;
                    }
                }
            }
        }

        if (!boundsFound)
        {
            Vector2 canvasSize = canvasRect.sizeDelta;
            return new Rect(-canvasSize.x * 0.5f, -canvasSize.y * 0.5f, canvasSize.x, canvasSize.y);
        }

        float padding = 1f;
        Rect bounds = new Rect(
            minX - padding,
            minY - padding,
            (maxX - minX) + (padding * 2),
            (maxY - minY) + (padding * 2)
        );

        originalCanvasBounds = bounds;
        return bounds;
    }

    /// <summary>
    /// Prepare UI elements for screen space capture while preserving original alpha values
    /// </summary>
    private void PrepareUIElementsForCapture()
    {
        Canvas canvas = targetUIParent.GetComponentInParent<Canvas>();
        if (canvas == null) return;

        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        Canvas.ForceUpdateCanvases();

        foreach (UIElementData element in detectedElements)
        {
            if (element.includeInConsolidation)
            {
                element.gameObject.SetActive(true);
                // Note: We no longer force alpha to 1.0f - preserve original alpha values
            }
            else
            {
                element.gameObject.SetActive(false);
            }
        }

        Canvas.ForceUpdateCanvases();
    }

    /// <summary>
    /// Generate consolidated texture using pixel-perfect dimensions
    /// </summary>
    private Texture2D GenerateConsolidatedTexture()
    {
        Canvas canvas = targetUIParent.GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("No Canvas found in parent hierarchy!");
            return null;
        }

        // Store original canvas state
        RenderMode originalRenderMode = canvas.renderMode;
        Camera originalCamera = canvas.worldCamera;
        int originalSortingOrder = canvas.sortingOrder;

        // Store original element states
        Dictionary<GameObject, bool> originalActiveStates = new Dictionary<GameObject, bool>();
        // Remove the originalColors dictionary since we're no longer modifying colors

        foreach (UIElementData element in detectedElements)
        {
            originalActiveStates[element.gameObject] = element.gameObject.activeSelf;
            // Remove the color storage logic
        }

        try
        {
            PrepareUIElementsForCapture();

            Rect canvasBounds = CalculateCanvasBounds();

            // Use pixel-perfect dimensions if enabled, otherwise use specified dimensions
            int renderWidth, renderHeight;
            if (useNativeResolution && calculatedPixelDimensions.x > 0 && calculatedPixelDimensions.y > 0)
            {
                renderWidth = (int)calculatedPixelDimensions.x;
                renderHeight = (int)calculatedPixelDimensions.y;
            }
            else
            {
                renderWidth = textureWidth;
                renderHeight = textureHeight;
            }

            RenderTexture renderTexture = new RenderTexture(renderWidth, renderHeight, 24, RenderTextureFormat.ARGB32);
            RenderTexture previousActive = RenderTexture.active;

            GameObject tempCameraObj = new GameObject("TempUICamera");
            Camera tempCamera = tempCameraObj.AddComponent<Camera>();

            tempCamera.clearFlags = CameraClearFlags.SolidColor;
            tempCamera.backgroundColor = Color.clear;
            tempCamera.orthographic = true;
            tempCamera.cullingMask = 1 << 5; // UI layer
            tempCamera.targetTexture = renderTexture;
            tempCamera.nearClipPlane = -1000f;
            tempCamera.farClipPlane = 1000f;

            // Enhanced settings for better alpha capture
            tempCamera.allowMSAA = false;
            tempCamera.allowHDR = false;
            tempCamera.useOcclusionCulling = false;

            tempCamera.transform.position = new Vector3(
                canvasBounds.center.x,
                canvasBounds.center.y,
                -100f
            );
            tempCamera.orthographicSize = canvasBounds.height * 0.5f;

            // Calculate aspect ratio for proper viewport
            float boundsAspect = canvasBounds.width / canvasBounds.height;
            float renderAspect = (float)renderWidth / renderHeight;

            if (boundsAspect > renderAspect)
            {
                float viewportHeight = renderAspect / boundsAspect;
                tempCamera.rect = new Rect(0, (1f - viewportHeight) * 0.5f, 1f, viewportHeight);
            }
            else
            {
                float viewportWidth = boundsAspect / renderAspect;
                tempCamera.rect = new Rect((1f - viewportWidth) * 0.5f, 0, viewportWidth, 1f);
            }

            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = tempCamera;
            canvas.planeDistance = 100f;

            Canvas.ForceUpdateCanvases();

            RenderTexture.active = renderTexture;
            GL.Clear(true, true, Color.clear);

            tempCamera.Render();

            Texture2D rawTexture = new Texture2D(renderWidth, renderHeight, TextureFormat.RGBA32, false);
            rawTexture.ReadPixels(new Rect(0, 0, renderWidth, renderHeight), 0, 0);

            // Process pixels to enhance alpha values before applying
            Color[] capturedPixels = rawTexture.GetPixels();
            Color[] processedPixels = ProcessCapturedPixels(capturedPixels, renderWidth, renderHeight);
            rawTexture.SetPixels(processedPixels);
            rawTexture.Apply();

            // Crop texture to actual content bounds for pixel-perfect dimensions
            Texture2D consolidatedTexture = CropTextureToContent(rawTexture);

            // Clean up the raw texture
            if (rawTexture != consolidatedTexture)
            {
                DestroyImmediate(rawTexture);
            }

            // Clean up
            RenderTexture.active = previousActive;
            DestroyImmediate(tempCameraObj);
            renderTexture.Release();

            SaveTextureAsset(consolidatedTexture);

            ClearPreview();
            previewTexture = consolidatedTexture;

            return consolidatedTexture;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error during texture generation: {ex.Message}");
            return null;
        }
        finally
        {
            RestoreOriginalStates(canvas, originalRenderMode, originalCamera, originalSortingOrder,
                                originalActiveStates);
        }
    }

    /// <summary>
    /// Process captured pixels to enhance alpha values and maintain visual consistency
    /// </summary>
    private Color[] ProcessCapturedPixels(Color[] pixels, int width, int height)
    {
        Color[] processedPixels = new Color[pixels.Length];

        for (int i = 0; i < pixels.Length; i++)
        {
            Color originalPixel = pixels[i];

            // Skip fully transparent pixels
            if (originalPixel.a <= 0.01f)
            {
                processedPixels[i] = originalPixel;
                continue;
            }

            // Enhance alpha visibility while preserving the original alpha relationship
            // This compensates for the rendering pipeline differences
            float enhancedAlpha = Mathf.Pow(originalPixel.a, 0.7f); // Gamma-like correction for alpha

            // Preserve the color intensity relative to alpha
            float alphaRatio = originalPixel.a > 0 ? enhancedAlpha / originalPixel.a : 1f;

            processedPixels[i] = new Color(
                originalPixel.r * alphaRatio,
                originalPixel.g * alphaRatio,
                originalPixel.b * alphaRatio,
                enhancedAlpha
            );
        }

        return processedPixels;
    }

    /// <summary>
    /// Crop texture to actual content bounds, ensuring dimensions are multiples of 4 for compression compatibility
    /// </summary>
    private Texture2D CropTextureToContent(Texture2D sourceTexture)
    {
        Color[] pixels = sourceTexture.GetPixels();
        int width = sourceTexture.width;
        int height = sourceTexture.height;

        int minX = width, maxX = -1;
        int minY = height, maxY = -1;

        // Find actual content bounds by scanning for non-transparent pixels
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color pixel = pixels[y * width + x];
                if (pixel.a > 0.01f) // Consider nearly transparent pixels as empty
                {
                    minX = Mathf.Min(minX, x);
                    maxX = Mathf.Max(maxX, x);
                    minY = Mathf.Min(minY, y);
                    maxY = Mathf.Max(maxY, y);
                }
            }
        }

        // If no content found, return original texture
        if (maxX == -1 || maxY == -1)
        {
            Debug.LogWarning("No visible content found in texture");
            return sourceTexture;
        }

        // Add small padding to prevent edge artifacts
        int padding = 2;
        minX = Mathf.Max(0, minX - padding);
        minY = Mathf.Max(0, minY - padding);
        maxX = Mathf.Min(width - 1, maxX + padding);
        maxY = Mathf.Min(height - 1, maxY + padding);

        int croppedWidth = maxX - minX + 1;
        int croppedHeight = maxY - minY + 1;

        // Ensure dimensions are multiples of 4 for compression compatibility
        croppedWidth = RoundToMultipleOf4(croppedWidth);
        croppedHeight = RoundToMultipleOf4(croppedHeight);

        // Recalculate bounds to center the content within the adjusted dimensions
        int widthAdjustment = croppedWidth - (maxX - minX + 1);
        int heightAdjustment = croppedHeight - (maxY - minY + 1);

        minX = Mathf.Max(0, minX - widthAdjustment / 2);
        minY = Mathf.Max(0, minY - heightAdjustment / 2);

        // Ensure we don't exceed source texture bounds
        if (minX + croppedWidth > width)
        {
            minX = width - croppedWidth;
        }
        if (minY + croppedHeight > height)
        {
            minY = height - croppedHeight;
        }

        // Create cropped texture with compression-compatible dimensions
        Texture2D croppedTexture = new Texture2D(croppedWidth, croppedHeight, TextureFormat.RGBA32, false);
        Color[] croppedPixels = new Color[croppedWidth * croppedHeight];

        // Fill with transparent pixels first
        for (int i = 0; i < croppedPixels.Length; i++)
        {
            croppedPixels[i] = Color.clear;
        }

        // Copy source pixels to the cropped texture
        for (int y = 0; y < croppedHeight; y++)
        {
            for (int x = 0; x < croppedWidth; x++)
            {
                int sourceX = minX + x;
                int sourceY = minY + y;

                if (sourceX >= 0 && sourceX < width && sourceY >= 0 && sourceY < height)
                {
                    croppedPixels[y * croppedWidth + x] = pixels[sourceY * width + sourceX];
                }
            }
        }

        croppedTexture.SetPixels(croppedPixels);
        croppedTexture.Apply();

        Debug.Log($"Texture cropped from {width}x{height} to {croppedWidth}x{croppedHeight} (compression-compatible)");

        return croppedTexture;
    }

    /// <summary>
    /// Round dimension to the nearest multiple of 4, ensuring minimum size of 4
    /// </summary>
    private int RoundToMultipleOf4(int value)
    {
        return Mathf.Max(4, ((value + 3) / 4) * 4);
    }

    /// <summary>
    /// Restore original canvas and element states after texture generation
    /// </summary>
    private void RestoreOriginalStates(Canvas canvas, RenderMode originalRenderMode, Camera originalCamera,
                                     int originalSortingOrder, Dictionary<GameObject, bool> originalActiveStates)
    {
        canvas.renderMode = originalRenderMode;
        canvas.worldCamera = originalCamera;
        canvas.sortingOrder = originalSortingOrder;

        foreach (KeyValuePair<GameObject, bool> kvp in originalActiveStates)
        {
            if (kvp.Key != null)
            {
                kvp.Key.SetActive(kvp.Value);
            }
        }

        Canvas.ForceUpdateCanvases();
    }

    private void SaveTextureAsset(Texture2D texture)
    {
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }

        byte[] textureBytes = texture.EncodeToPNG();
        string texturePath = Path.Combine(outputPath, $"{assetName}_Texture.png");

        File.WriteAllBytes(texturePath, textureBytes);
        AssetDatabase.Refresh();

        TextureImporter importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.isReadable = false;
            importer.filterMode = FilterMode.Bilinear;

            // Set compression settings based on texture properties
            if (HasAlphaChannel(texture))
            {
                // For textures with alpha, use appropriate compression
                if (IsDimensionMultipleOf4(texture.width) && IsDimensionMultipleOf4(texture.height))
                {
                    importer.textureCompression = TextureImporterCompression.Compressed;
                    importer.compressionQuality = 100; // High quality for UI elements
                }
                else
                {
                    // Fallback to uncompressed for non-multiple-of-4 dimensions
                    importer.textureCompression = TextureImporterCompression.Uncompressed;
                    Debug.LogWarning($"Texture dimensions ({texture.width}x{texture.height}) are not multiples of 4. Using uncompressed format to avoid compression warnings.");
                }
            }
            else
            {
                importer.textureCompression = TextureImporterCompression.Compressed;
            }

            // Set pixels per unit based on canvas scale for proper sizing
            Canvas canvas = targetUIParent.GetComponentInParent<Canvas>();
            CanvasScaler canvasScaler = canvas.GetComponent<CanvasScaler>();

            float pixelsPerUnit = 100f; // Unity default
            if (canvasScaler != null && useNativeResolution)
            {
                float scaleFactor = GetCanvasScaleFactor(canvas, canvasScaler);
                pixelsPerUnit = 100f * scaleFactor;
            }

            TextureImporterSettings settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.spritePixelsPerUnit = pixelsPerUnit;
            importer.SetTextureSettings(settings);

            // Apply platform-specific settings for better optimization
            TextureImporterPlatformSettings platformSettings = new TextureImporterPlatformSettings();
            platformSettings.name = "Standalone";
            platformSettings.overridden = true;
            platformSettings.maxTextureSize = Mathf.NextPowerOfTwo(Mathf.Max(texture.width, texture.height));

            if (HasAlphaChannel(texture))
            {
                platformSettings.format = TextureImporterFormat.DXT5; // Supports alpha
            }
            else
            {
                platformSettings.format = TextureImporterFormat.DXT1; // No alpha, better compression
            }

            importer.SetPlatformTextureSettings(platformSettings);

            AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceUpdate);

            Debug.Log($"Texture saved with dimensions: {texture.width}x{texture.height}, Compression: {importer.textureCompression}");
        }
    }

    /// <summary>
    /// Check if texture has meaningful alpha channel content
    /// </summary>
    private bool HasAlphaChannel(Texture2D texture)
    {
        Color[] pixels = texture.GetPixels();

        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].a < 0.99f) // Has transparency
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Check if dimension is a multiple of 4
    /// </summary>
    private bool IsDimensionMultipleOf4(int dimension)
    {
        return dimension % 4 == 0;
    }

    private void ConsolidateUIElements()
    {
        if (!ValidateConsolidation()) return;

        CreateOutputDirectory();

        Texture2D consolidatedTexture = GenerateConsolidatedTexture();
        if (consolidatedTexture == null)
        {
            EditorUtility.DisplayDialog("Error", "Failed to generate consolidated texture!", "OK");
            return;
        }

        string texturePath = Path.Combine(outputPath, $"{assetName}_Texture.png");
        Sprite consolidatedSprite = AssetDatabase.LoadAssetAtPath<Sprite>(texturePath);

        if (consolidatedSprite == null)
        {
            EditorUtility.DisplayDialog("Error", "Failed to load consolidated sprite!", "OK");
            return;
        }

        GameObject consolidatedObject = CreateConsolidatedGameObject(consolidatedSprite);

        if (preserveOriginalHierarchy)
        {
            DisableOriginalElements();
        }
        else
        {
            DestroyOriginalElements();
        }

        EditorUtility.DisplayDialog("Success",
            $"UI consolidation completed successfully!\nConsolidated object: {consolidatedObject.name}\nTexture saved at: {texturePath}\nDimensions: {consolidatedTexture.width}x{consolidatedTexture.height} pixels", "OK");

        Selection.activeGameObject = consolidatedObject;
    }

    private GameObject CreateConsolidatedGameObject(Sprite sprite)
    {
        GameObject consolidatedObject = new GameObject($"{assetName}_Consolidated");
        consolidatedObject.transform.SetParent(targetUIParent.transform.parent);
        consolidatedObject.transform.SetSiblingIndex(targetUIParent.transform.GetSiblingIndex());

        RectTransform rectTransform = consolidatedObject.AddComponent<RectTransform>();
        Image imageComponent = consolidatedObject.AddComponent<Image>();

        imageComponent.sprite = sprite;
        imageComponent.type = Image.Type.Simple;
        imageComponent.preserveAspect = false;

        Canvas parentCanvas = targetUIParent.GetComponentInParent<Canvas>();
        if (parentCanvas != null)
        {
            RectTransform canvasRect = parentCanvas.GetComponent<RectTransform>();

            if (useNativeResolution && originalCanvasBounds.width > 0 && originalCanvasBounds.height > 0)
            {
                rectTransform.SetParent(canvasRect);

                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                rectTransform.anchoredPosition = new Vector2(originalCanvasBounds.center.x, originalCanvasBounds.center.y);

                rectTransform.sizeDelta = new Vector2(originalCanvasBounds.width, originalCanvasBounds.height);
            }
            else
            {
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;
            }
        }

        return consolidatedObject;
    }

    private bool ValidateConsolidation()
    {
        if (targetUIParent == null)
        {
            EditorUtility.DisplayDialog("Error", "Target UI Parent is not assigned.", "OK");
            return false;
        }

        if (detectedElements.Count == 0)
        {
            EditorUtility.DisplayDialog("Error", "No UI elements detected. Please scan first.", "OK");
            return false;
        }

        if (detectedElements.FindAll(e => e.includeInConsolidation).Count == 0)
        {
            EditorUtility.DisplayDialog("Error", "No elements selected for consolidation.", "OK");
            return false;
        }

        Canvas canvas = targetUIParent.GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            EditorUtility.DisplayDialog("Error", "Target UI Parent must be a child of a Canvas.", "OK");
            return false;
        }

        return true;
    }

    private void CreateOutputDirectory()
    {
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
            AssetDatabase.Refresh();
        }
    }

    private void DisableOriginalElements()
    {
        foreach (UIElementData element in detectedElements)
        {
            if (element.includeInConsolidation)
            {
                element.gameObject.SetActive(false);
            }
        }
    }

    private void DestroyOriginalElements()
    {
        foreach (UIElementData element in detectedElements)
        {
            if (element.includeInConsolidation)
            {
                DestroyImmediate(element.gameObject);
            }
        }
    }

    private void ClearPreview()
    {
        if (previewTexture != null)
        {
            DestroyImmediate(previewTexture);
            previewTexture = null;
        }
    }

    private void OnDestroy()
    {
        ClearPreview();
    }
}