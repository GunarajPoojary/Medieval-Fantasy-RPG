using UnityEngine;
using UnityEngine.UI;

namespace ProjectEmbersteel.UI
{
    [RequireComponent(typeof(GridLayoutGroup))]
    [ExecuteAlways] // Allows updates in edit mode
    public class HorizontalAutoGridSpacing : MonoBehaviour
    {
        private GridLayoutGroup _gridLayout;
        private RectTransform _rectTransform;

        private void OnEnable()
        {
            InitializeComponents();
            UpdateSpacingHorizontally();
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (!Application.isPlaying)
            {
                UpdateSpacingHorizontally();
            }
        }
#endif

        public void UpdateSpacingHorizontally()
        {
            if (_gridLayout == null || _rectTransform == null)
                return;

            Vector2 totalSize = _rectTransform.rect.size;
            Vector2 cellSize = _gridLayout.cellSize;
            RectOffset padding = _gridLayout.padding;

            // Calculate how many columns fit
            int columns = Mathf.FloorToInt((totalSize.x - padding.left - padding.right) / cellSize.x);
            columns = Mathf.Max(1, columns);

            // Calculate total cell width
            float totalCellWidth = columns * cellSize.x;

            // Calculate X spacing (horizontal) only
            float spacingX = 0f;
            if (columns > 1)
            {
                spacingX = (totalSize.x - padding.left - padding.right - totalCellWidth) / (columns - 1);
            }

            // Apply only X spacing; preserve Y spacing
            _gridLayout.spacing = new Vector2(spacingX, _gridLayout.spacing.y);
        }

        private void InitializeComponents()
        {
            _gridLayout = GetComponent<GridLayoutGroup>();
            _rectTransform = GetComponent<RectTransform>();
        }
    }
}