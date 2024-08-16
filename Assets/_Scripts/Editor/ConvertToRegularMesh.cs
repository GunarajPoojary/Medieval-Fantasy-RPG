using UnityEngine;

namespace RPG.Editor.Utils
{
    /// <summary>
    /// Converts a SkinnedMeshRenderer to a MeshRenderer and MeshFilter for preview purposes.
    /// This is useful for displaying the mesh in the editor without the need for a SkinnedMeshRenderer.
    /// </summary>
    [ExecuteInEditMode]
    public class ConvertToRegularMesh : MonoBehaviour
    {
        public void Start()
        {
            if (GetComponent<SkinnedMeshRenderer>())
            {
                SkinnedMeshRenderer skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();

                MeshFilter filter = gameObject.AddComponent<MeshFilter>();

                MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();

                filter.mesh = skinnedMeshRenderer.sharedMesh;

                renderer.sharedMaterials = skinnedMeshRenderer.sharedMaterials;

                DestroyImmediate(skinnedMeshRenderer);

                DestroyImmediate(this);
            }
        }
    }
}