namespace RPG.Core
{
    public class InputManager : GenericSingleton<InputManager>
    {
        private GameInputAction _inputActions;
        public GameInputAction.UIActions UIActions { get; private set; }
        public GameInputAction.PlayerActions PlayerActions { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            InitializeInputActions();
        }

        private void InitializeInputActions()
        {
            _inputActions = new GameInputAction();
            UIActions = _inputActions.UI;
            PlayerActions = _inputActions.Player;
        }

        private void OnEnable()
        {
            if (_inputActions == null)
                InitializeInputActions();

            PlayerActions.Enable();
            UIActions.Enable();
        }

        private void OnDisable()
        {
            if (_inputActions != null)
            {
                PlayerActions.Disable();
                UIActions.Disable();
            }
        }
    }
}
