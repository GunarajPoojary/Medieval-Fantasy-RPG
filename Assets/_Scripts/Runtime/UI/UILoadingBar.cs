using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class UILoadingBar : MonoBehaviour
    {
        [SerializeField] private Image _loadingBar;
        [SerializeField] private RectTransform _pivot;
        private RectTransform _loadingBarRect;

        private void Awake() => _loadingBarRect = _loadingBar.GetComponent<RectTransform>();

        public void ResetBar() => _loadingBar.fillAmount = 0;

        public void UpdateBar(float percentage)
        {
            _loadingBar.fillAmount = percentage;
            _pivot.anchoredPosition = new Vector2(_loadingBarRect.rect.width * percentage, 0);
        }
    }
}