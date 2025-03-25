using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Realfy
{
    [CustomEditor(typeof(RealfyImage))]
    public class RealfyImageEditor : Editor
    {
        //public override void OnInspectorGUI()
        //{
        //    RealfyImage realfyImage = (RealfyImage)target;

        //    // Draw the default inspector elements
        //    DrawDefaultInspector();
        //}
    }

    [RequireComponent(typeof(RectTransform))]
    public class RealfyImage : Image
    {
        [SerializeField]
        private Gradient gradient; // Serialize the gradient for inspector access

        // Gradient property for inspector visibility
        public Gradient Gradient
        {
            get => gradient;
            set => gradient = value;
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);

            if (gradient == null)
                return;

            var verts = new List<UIVertex>();
            vh.GetUIVertexStream(verts);

            // Apply gradient to the vertices
            for (int i = 0; i < verts.Count; i++)
            {
                UIVertex vertex = verts[i];

                // Calculate the color based on the position of the vertex (you can customize this logic)
                float position = Mathf.InverseLerp(0, 1, vertex.position.x / rectTransform.rect.width);
                vertex.color = gradient.Evaluate(position); // Set the color based on the gradient

                verts[i] = vertex;
            }

            vh.Clear();
            vh.AddUIVertexTriangleStream(verts);
        }
    }
}