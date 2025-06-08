using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RPG
{
    public class DefaultButton : Button
    {
        [SerializeField] private float _pressedScale = 0.8f;
        [SerializeField] private float _animationDuration = 0.2f;

        public override void OnPointerDown(PointerEventData eventData)
        {
            // Scale down to 0.8 on press
            Tween.Scale(transform, _pressedScale, _animationDuration, Ease.OutQuad);
            base.OnPointerDown(eventData);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            // Scale back to 1.0 on release
            Tween.Scale(transform, 1f, _animationDuration, Ease.OutBounce);
            base.OnPointerUp(eventData);
        }
    }
}