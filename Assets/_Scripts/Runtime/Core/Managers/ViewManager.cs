using EasyTransition;
using RPG.Core.Utils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace RPG.Core.Managers
{
    /// <summary>
    /// Manages view transitions and scene loading, while maintaining input settings.
    /// </summary>
    public class ViewManager : PersistentSingleton<ViewManager>
    {
        [SerializeField] private float _startDelay;
        [SerializeField] private TransitionSettings _transition;
        [Space]
        [SerializeField] private VoidReturnNonParameterEventChannelSO _continueGameChannelSO; // Event channel for continuing the game.
        [Space]
        [SerializeField] private VoidReturnIntParameterEventChannelSO _loadSceneChannelSO; // Event channel for loading a scene by index.

        private UIInputManager _inputManager;

        private void OnEnable()
        {
            _loadSceneChannelSO.OnEventRaised += HandleLoadScene;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            _loadSceneChannelSO.OnEventRaised -= HandleLoadScene;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public int GetActiveScene() => SceneManager.GetActiveScene().buildIndex;

        public void LoadScene(int index) => TransitionManager.Instance().Transition(index, _transition, _startDelay);

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => SetCharacterMenuInput();

        private void HandleLoadScene(int sceneIndex) => LoadScene(sceneIndex);

        private void SetCharacterMenuInput()
        {
            if (_inputManager == null)
            {
                _inputManager = UIInputManager.Instance;

                if (_inputManager != null)
                {
                    _inputManager.UIInputActions.CharacterMenu.performed += OnCharacterMenuPerformed;
                }
            }
        }

        private void OnCharacterMenuPerformed(InputAction.CallbackContext context)
        {
            _continueGameChannelSO.RaiseEvent();

            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int targetSceneIndex = currentSceneIndex == 2 ? 1 : 2;

            LoadScene(targetSceneIndex);
        }
    }
}