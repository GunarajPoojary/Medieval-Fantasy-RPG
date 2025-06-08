using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RPG
{
    [RequireComponent(typeof(Image))]
    public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        private Image _image;
        private TabGroup _tabGroup;
        private int _index;

        private void Awake() => _image = GetComponent<Image>();

        public void Setup(TabGroup group, int index)
        {
            _tabGroup = group;
            _index = index;
        }

        public void OnPointerClick(PointerEventData eventData) => _tabGroup.OnTabSelected(_index, _image);

        public void OnPointerEnter(PointerEventData eventData) => _tabGroup.OnHoverEnter(_image);

        public void OnPointerExit(PointerEventData eventData) => _tabGroup.OnHoverExit(_image);
    }
}