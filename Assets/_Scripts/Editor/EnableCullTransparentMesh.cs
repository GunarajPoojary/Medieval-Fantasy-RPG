using UnityEngine;
using UnityEditor;

public class EnableCullTransparentMesh : EditorWindow
{
    [MenuItem("Tools/Custom/Enable Cull Transparent Mesh on All CanvasRenderers")]
    public static void EnableCullMesh()
    {
        int count = 0;

        // Get all CanvasRenderer components in the scene (including inactive objects)
        CanvasRenderer[] renderers = FindObjectsByType<CanvasRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        Undo.RecordObjects(renderers, "Enable Cull Transparent Mesh");

        foreach (CanvasRenderer cr in renderers)
        {
            if (!cr.cullTransparentMesh)
            {
                cr.cullTransparentMesh = true;
                EditorUtility.SetDirty(cr);
                count++;
            }
        }

        Debug.Log($"Enabled Cull Transparent Mesh on {count} CanvasRenderer(s).");
    }
}
