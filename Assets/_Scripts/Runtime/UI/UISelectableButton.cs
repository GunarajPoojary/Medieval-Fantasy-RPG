using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ProjectEmbersteel.UI
{
    public abstract class UISelectableButton<T> : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public event Action<T> OnClick;

        protected abstract T GetValue();

        public virtual void OnPointerClick(PointerEventData eventData) => OnClick?.Invoke(GetValue());

        public abstract void OnPointerEnter(PointerEventData eventData);

        public abstract void OnPointerExit(PointerEventData eventData);
    }
}