using RPG.Core.Utils;
using RPG.InputActions;
using UnityEngine;

namespace RPG.Core.Managers
{
    /// <summary>
    /// Manages the UI input actions, allowing for centralized input control in the game.
    /// </summary>
    [DefaultExecutionOrder(-1)]
    public class UIInputManager : SimpleSingleton<UIInputManager>
    {
        public PlayerInputActions InputActions { get; set; }

        public PlayerInputActions.UIActions UIInputActions { get; set; }

        protected override void Awake()
        {
            base.Awake();
            InputActions = new PlayerInputActions();
            UIInputActions = InputActions.UI;
        }

        private void OnEnable() => InputActions.Enable();

        private void OnDisable() => InputActions.Disable();
    }
}