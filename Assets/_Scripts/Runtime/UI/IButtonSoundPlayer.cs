using UnityEngine.EventSystems;

namespace RPG
{
    public interface IButtonSoundPlayer
    {
        void OnPointerEnter(BaseEventData data);

        void OnPointerClick(BaseEventData data);
    }
}