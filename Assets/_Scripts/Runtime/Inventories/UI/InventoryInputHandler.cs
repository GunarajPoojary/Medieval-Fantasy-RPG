using RPG.Core.Managers;
using RPG.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG.Inventories.UI
{
    public class InventoryInputHandler : IInventoryInputHandler
    {
        private readonly GameObject _inventoryPanel;

        public InventoryInputHandler(GameObject inventoryPanel)
        {
            _inventoryPanel = inventoryPanel;
        }

        #region IInputHandler Methods
        public void Enable() => PlayerUIInputManager.Instance.UIInputActions.Inventory.started += HandleInventoryInput;

        public void Disable() => PlayerUIInputManager.Instance.UIInputActions.Inventory.started -= HandleInventoryInput;
        #endregion

        private void HandleInventoryInput(InputAction.CallbackContext context)
        {
            IUIVisibilityToggler uIElement = UIManager.Instance;

            bool isActive = _inventoryPanel.activeSelf;
            Time.timeScale = isActive ? 1.0f : 0.0f;
            uIElement.ToggleUIs(isActive);
            _inventoryPanel.SetActive(!isActive);
        }
    }
}