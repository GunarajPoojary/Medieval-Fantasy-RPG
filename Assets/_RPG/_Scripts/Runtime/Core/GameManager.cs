using UnityEngine;

namespace RPG.Core
{
    public class GameManager : GenericSingleton<GameManager>
    {
        public bool IsFirstDataLoad { get; set; }

        [SerializeField] private UI.MainMenu _mainMenu;
        public bool IsNewGame { get; private set; } = true;


        protected override void Awake()
        {
            base.Awake();
            if (_mainMenu != null)
            {
                _mainMenu.OnNewGameButtonPressed += OnNewGameButtonPressed;
                _mainMenu.OnContinueButtonPressed += OnContinueButtonPressed;
            }
        }

        private void OnNewGameButtonPressed() => IsNewGame = true;

        private void OnContinueButtonPressed() => IsNewGame = false;
    }
}
