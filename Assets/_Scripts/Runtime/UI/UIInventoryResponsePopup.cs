using PrimeTween;
using TMPro;
using UnityEngine;

namespace RPG
{
    public class UIInventoryResponsePopup : MonoBehaviour
    {
        [SerializeField] private RectTransform _popupPanel;
        [SerializeField] private float _displayDuration = 3f;
        [SerializeField] private float _animationDuration = 0.3f;
        [SerializeField] private TMP_Text _messageText;
        [SerializeField] private float _visibleOffset = 0f;
        [SerializeField] private float _hiddenOffset = -120f;
        private bool _isAnimating = false;

        private void Start()
        {
            _popupPanel.localPosition = new Vector2(0, _hiddenOffset);

            _popupPanel.gameObject.SetActive(false);
        }

        public void ShowPopup(string message)
        {
            if (_isAnimating) return;
            
            _messageText.text = message;

            _popupPanel.gameObject.SetActive(true);

            _isAnimating = true;

            Tween.LocalPositionY(_popupPanel, _visibleOffset, _animationDuration, Ease.OutBack)
                 .OnComplete(OnShowPopup);
        }

        private void OnShowPopup() => Tween.Delay(_displayDuration)
                                           .OnComplete(OnReachDisplayDuration);

        private void OnReachDisplayDuration() => Tween.LocalPositionY(_popupPanel, _hiddenOffset, _animationDuration, Ease.InBack)
                                                      .OnComplete(OnHidePopup);

        private void OnHidePopup()
        {
            _popupPanel.gameObject.SetActive(false);
            _isAnimating = false;
        }
    }
}