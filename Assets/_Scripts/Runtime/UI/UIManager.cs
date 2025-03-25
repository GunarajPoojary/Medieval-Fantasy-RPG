using UnityEngine;

namespace RPG
{
    /// <summary>
    /// Toggles the visibility of On-Screen UI elements, When UI's such as Inventory is set active.
    /// </summary>
    public class UIManager : SimpleSingleton<UIManager>, IUIVisibilityToggler
    {
        [SerializeField] private GameObject[] _backgroundUIs;

        public void ToggleUIs(bool shouldSetActive)
        {
            foreach (GameObject ui in _backgroundUIs)
            {
                ui.SetActive(shouldSetActive);
            }
        }
    }
}