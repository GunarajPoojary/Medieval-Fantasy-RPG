using UnityEngine;
using UnityEngine.UI;

namespace Realfy
{
    public class GradientImage : MonoBehaviour
    {
        public Gradient gradient;
        public Image image;
        public int resolution = 256;

        void Start()
        {
            Texture2D gradientTexture = GenerateGradientTexture();
            image.material.mainTexture = gradientTexture;
        }

        Texture2D GenerateGradientTexture()
        {
            Texture2D texture = new Texture2D(1, resolution);
            for (int i = 0; i < resolution; i++)
            {
                Color color = gradient.Evaluate((float)i / (resolution - 1));
                texture.SetPixel(0, i, color);
            }
            texture.Apply();
            return texture;
        }
    }
}
