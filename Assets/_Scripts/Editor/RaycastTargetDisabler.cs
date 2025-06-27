using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public partial class RaycastTargetDisabler : EditorWindow
{
    [MenuItem("Tools/Custom/Disable Raycast Target and Maskable (Entire Scene)")]
    public static void DisableRaycastAndMaskableInScene()
    {
        Graphic[] graphics = FindObjectsByType<Graphic>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        int count = ProcessGraphics(graphics);
        Debug.Log($"[Scene] Disabled Raycast Target and Maskable on {count} UI element(s).");
    }

    [MenuItem("Tools/Custom/Disable Raycast Target and Maskable (Selection Only)")]
    public static void DisableRaycastAndMaskableInSelection()
    {
        if (Selection.transforms.Length == 0)
        {
            Debug.LogWarning("No GameObject selected.");
            return;
        }

        // Collect all Graphics in selected GameObjects and their children (including inactive)
        System.Collections.Generic.List<Graphic> graphics = new System.Collections.Generic.List<Graphic>();
        foreach (Transform root in Selection.transforms)
        {
            graphics.AddRange(root.GetComponentsInChildren<Graphic>(true));
        }

        int count = ProcessGraphics(graphics.ToArray());
        Debug.Log($"[Selection] Disabled Raycast Target and Maskable on {count} UI element(s).");
    }

    private static int ProcessGraphics(Graphic[] graphics)
    {
        int count = 0;
        Undo.RecordObjects(graphics, "Disable Raycast Target and Maskable");

        foreach (Graphic graphic in graphics)
        {
            bool changed = false;

            if (graphic.raycastTarget)
            {
                graphic.raycastTarget = false;
                changed = true;
            }

            if (graphic is MaskableGraphic maskableGraphic && maskableGraphic.maskable)
            {
                maskableGraphic.maskable = false;
                changed = true;
            }

            if (changed)
            {
                EditorUtility.SetDirty(graphic);
                count++;
            }
        }

        return count;
    }
}