using RPG.Core.Utils;
using UnityEngine;

namespace RPG.UI
{
    /// <summary>
    /// Toggles the visibility of On-Screen UI elements, When UI's such as Inventory is set active.
    /// </summary>
    public class UIManager : SimpleSingleton<UIManager>, IUIElement
    {
        [SerializeField]
        private GameObject[] _backgroundUIs;

        public void ToggleUIs(bool shouldSetActive)
        {
            foreach (GameObject ui in _backgroundUIs)
            {
                ui.SetActive(shouldSetActive);
            }
        }
    }
}