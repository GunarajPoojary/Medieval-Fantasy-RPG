using UnityEngine;

namespace RPG.Editor.Utils
{
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
