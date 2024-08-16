using UnityEngine.UI;

namespace RPG.UI
{
    public class MenuButtonStateManager : IMenuButtonStateManager
    {
        private readonly Button[] _menuButtons;

        public MenuButtonStateManager(params Button[] menuButtons)
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