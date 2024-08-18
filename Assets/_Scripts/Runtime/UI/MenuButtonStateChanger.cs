using UnityEngine.UI;

namespace RPG.UI
{
    public class MenuButtonStateChanger : IMenuButtonStateChangeable
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