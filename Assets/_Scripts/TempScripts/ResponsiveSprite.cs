using UnityEngine;
using UnityEngine.UI;

namespace Realfy
{
    public class ResponsiveSprite : MonoBehaviour
    {
        [SerializeField] private Image[] _images;

        private void Start() => AdjustRectTransformToFit();

        private void AdjustRectTransformToFit()
        {
            foreach (var image in _images)
            {
                if (image == null || image.sprite == null)
                {
                    Debug.LogError("Image or Sprite is not assigned!");
                    return;
                }

                RectTransform rectTransform = image.rectTransform;

                // RectTransform dimensions
                float rectWidth = rectTransform.rect.width;
                float rectHeight = rectTransform.rect.height;

                // Sprite dimensions and aspect ratio
                float spriteWidth = image.sprite.rect.width;
                float spriteHeight = image.sprite.rect.height;
                float spriteAspect = spriteWidth / spriteHeight;

                // RectTransform aspect ratio
                float rectAspect = rectWidth / rectHeight;

                // Adjust RectTransform based on the aspect ratio comparison
                if (rectAspect > spriteAspect)
                {
                    // RectTransform is wider; increase height to remove vertical gap
                    float newHeight = rectWidth / spriteAspect;
                    rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
                }
                else if (rectAspect < spriteAspect)
                {
                    // RectTransform is taller; increase width to remove horizontal gap
                    float newWidth = rectHeight * spriteAspect;
                    rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
                }

                SetAnchorsToCorners(rectTransform);
            }
        }

        private void SetAnchorsToCorners(RectTransform rectTransform)
        {
            if (rectTransform == null || rectTransform.parent == null) return;

            RectTransform pt = rectTransform.parent as RectTransform;
            if (pt == null) return;

            // Calculate new anchor positions based on offsets and parent's dimensions
            Vector2 newAnchorsMin = new Vector2(
                rectTransform.anchorMin.x + rectTransform.offsetMin.x / pt.rect.width,
                rectTransform.anchorMin.y + rectTransform.offsetMin.y / pt.rect.height
            );
            Vector2 newAnchorsMax = new Vector2(
                rectTransform.anchorMax.x + rectTransform.offsetMax.x / pt.rect.width,
                rectTransform.anchorMax.y + rectTransform.offsetMax.y / pt.rect.height
            );

            // Apply new anchors and reset offsets
            rectTransform.anchorMin = newAnchorsMin;
            rectTransform.anchorMax = newAnchorsMax;
            rectTransform.offsetMin = rectTransform.offsetMax = Vector2.zero;
        }
    }
}