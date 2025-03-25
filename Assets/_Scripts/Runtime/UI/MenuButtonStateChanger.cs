using UnityEngine.UI;

namespace RPG
{
    public class MenuButtonStateChanger : IMenuButtonStateChanger
    {
        private readonly Button[] _menuButtons;

        public MenuButtonStateChanger(params Button[] menuButtons)
        {
            _menuButtons = menuButtons;
        }

        public void SetButtonsInteractable(bool state)
        {
            foreach (var button in _menuButtons)
            {
                button.interactable = state;
            }
        }
    }
}