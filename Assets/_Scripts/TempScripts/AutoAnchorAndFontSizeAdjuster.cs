using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class FontSizeReset
{
    public float FigmaSize;
    public float UnitySize;
}

public class AutoAnchorAndFontSizeAdjuster : MonoBehaviour
{
    [SerializeField] private List<FontSizeReset> fontSizeResets;

    /// <summary>
    /// Align anchors to match the corners of all child RectTransforms and remove LayoutElement/ContentSizeFitter if present.
    /// </summary>
    [ContextMenu("Auto Align Anchors")]
    private void Perform()
    {
        if (gameObject == null)
        {
            Debug.LogWarning("GameObject is null. Cannot perform operation.");
            return;
        }
        TraverseChildren(gameObject);
    }

    /// <summary>
    /// Align anchors to match the corners of all child RectTransforms and remove LayoutElement/ContentSizeFitter if present.
    /// </summary>
    [ContextMenu("SetAnchors")]
    private void SetAnchors()
    {
        if (gameObject == null)
        {
            Debug.LogWarning("GameObject is null. Cannot perform operation.");
            return;
        }
        TraverseChildrenForAnchors(gameObject);
    }

    /// <summary>
    /// Traverses the hierarchy of a GameObject and aligns anchors for all RectTransforms.
    /// </summary>
    /// <param name="parent">The parent GameObject to traverse.</param>
    private void TraverseChildren(GameObject parent)
    {
        if (parent == null) return;

        // Attempt to get RectTransform of the current GameObject
        var rectTrans = parent.GetComponent<RectTransform>();
        if (rectTrans != null)
        {
            // Remove LayoutElement if it exists
            var layEle = parent.GetComponent<LayoutElement>();
            if (layEle != null)
            {
                DestroyImmediate(layEle);
            }

            // Remove ContentSizeFitter if it exists
            var contentSize = parent.GetComponent<ContentSizeFitter>();
            if (contentSize != null)
            {
                DestroyImmediate(contentSize);
            }

            var textComp = parent.GetComponent<TextMeshProUGUI>();
            if (textComp != null)
            {
                textComp.fontSize = CheckFontSize(textComp.fontSize);
            }

            // Align anchors to corners
            AnchorsToCorners(rectTrans);
        }

        // Recursively process child GameObjects
        foreach (Transform child in parent.transform)
        {
            TraverseChildren(child.gameObject);
        }
    }

    /// <summary>
    /// Traverses the hierarchy of a GameObject and aligns anchors for all RectTransforms.
    /// </summary>
    /// <param name="parent">The parent GameObject to traverse.</param>
    private void TraverseChildrenForAnchors(GameObject parent)
    {
        if (parent == null) return;

        // Attempt to get RectTransform of the current GameObject
        var rectTrans = parent.GetComponent<RectTransform>();
        if (rectTrans != null)
        {
            // Align anchors to corners
            AnchorsToCorners(rectTrans);
        }

        // Recursively process child GameObjects
        foreach (Transform child in parent.transform)
        {
            TraverseChildren(child.gameObject);
        }
    }

    /// <summary>
    /// Aligns the RectTransform anchors to its corners relative to its parent's RectTransform.
    /// </summary>
    /// <param name="t">The RectTransform to align.</param>
    private void AnchorsToCorners(RectTransform t)
    {
        if (t == null || t.parent == null) return;

        RectTransform pt = t.parent as RectTransform;
        if (pt == null) return;

        // Calculate new anchor positions based on offsets and parent's dimensions
        Vector2 newAnchorsMin = new Vector2(
            t.anchorMin.x + t.offsetMin.x / pt.rect.width,
            t.anchorMin.y + t.offsetMin.y / pt.rect.height
        );
        Vector2 newAnchorsMax = new Vector2(
            t.anchorMax.x + t.offsetMax.x / pt.rect.width,
            t.anchorMax.y + t.offsetMax.y / pt.rect.height
        );

        // Apply new anchors and reset offsets
        t.anchorMin = newAnchorsMin;
        t.anchorMax = newAnchorsMax;
        t.offsetMin = t.offsetMax = Vector2.zero;
    }

    /// <summary>
    /// Checks if the given FigmaSize exists and prints the corresponding UnitySize.
    /// </summary>
    /// <param name="input">The input FigmaSize to check.</param>
    public float CheckFontSize(float input)
    {
        // Find the matching FontSizeReset
        var matchingSize = fontSizeResets.FirstOrDefault(f => Mathf.Approximately(f.FigmaSize, input));

        if (matchingSize != null)
        {
            return matchingSize.UnitySize;
        }

        // Return input as default if no match is found
        Debug.LogWarning($"FigmaSize {input} not found in fontSizeResets. Returning the original input.");
        return input;
    }
}
