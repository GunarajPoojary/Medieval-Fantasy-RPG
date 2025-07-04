using UnityEngine;
using UnityEngine.UI;

namespace ProjectEmbersteel.UI
{
    public class UILoadingBar : MonoBehaviour
    {
        [SerializeField] private Image _loadingBar;
        [SerializeField] private RectTransform _pivot;
        private RectTransform _loadingBarRect;

        private void Awake() => InitializeComponents();

        public void ResetBar() => _loadingBar.fillAmount = 0;

        public void UpdateBar(float percentage)
        {
            _loadingBar.fillAmount = percentage;
            _pivot.anchoredPosition = new Vector2(_loadingBarRect.rect.width * percentage, 0);
        }

        private void InitializeComponents() => _loadingBarRect = _loadingBar.GetComponent<RectTransform>();
    }
}