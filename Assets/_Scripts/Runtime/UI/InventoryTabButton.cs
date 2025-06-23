using UnityEngine.EventSystems;

namespace ProjectEmbersteel.UI
{
    public class InventoryTabButton : UISelectableButton<int>
    {
        private int _index;

        public void Initialize(int index)
        {
            _index = index;
        }

        protected override int GetValue() => _index;

        public override void OnPointerEnter(PointerEventData eventData)
        {

        }

        public override void OnPointerExit(PointerEventData eventData)
        {

        }
    }
}