using UnityEngine.EventSystems;

namespace RPG.UI
{
    public interface IButtonSoundPlayer
    {
        void OnPointerEnter(BaseEventData data);

        void OnPointerClick(BaseEventData data);
    }
}