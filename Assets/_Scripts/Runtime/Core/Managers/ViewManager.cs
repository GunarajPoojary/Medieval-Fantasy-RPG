using EasyTransition;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace RPG
{
    /// <summary>
    /// Manages view transitions and scene loading, while maintaining input settings.
    /// </summary>
    public class ViewManager : PersistentSingleton<ViewManager>
    {
        [SerializeField] private float _startDelay;
        [SerializeField] private TransitionSettings _transition;

        [SerializeField] private PlayerInputHandler _input;

        private void OnEnable()
        {
            _input.CharacterMenuAction.performed += GoToCharacterMenu;
        }

        private void GoToCharacterMenu(InputAction.CallbackContext ctx)
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int targetSceneIndex = currentSceneIndex == 2 ? 1 : 2;

            LoadScene(targetSceneIndex);
        }

        private void OnDisable()
        {
            _input.CharacterMenuAction.performed -= GoToCharacterMenu;
        }

        public int GetActiveScene() => SceneManager.GetActiveScene().buildIndex;

        public void LoadScene(int index) => TransitionManager.Instance().Transition(index, _transition, _startDelay);

        public void HandleLoadScene(int sceneIndex) => LoadScene(sceneIndex);
    }
}