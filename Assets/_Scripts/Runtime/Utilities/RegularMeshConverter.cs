using UnityEngine;

namespace RPG
{
    /// <summary>
    /// Replaces a SkinnedMeshRenderer with MeshRenderer and MeshFilter to make pickable gameobject.
    /// This is useful for displaying the mesh in the editor without the need for a SkinnedMeshRenderer.
    /// </summary>
    public class RegularMeshConverter : MonoBehaviour
    {
        [ContextMenu("Convert")]
        public void ConvertToRegularMesh()
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