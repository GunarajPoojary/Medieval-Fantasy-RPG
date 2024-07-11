using RPG.SaveLoad;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Core
{
    public class ViewManager : GenericSingleton<ViewManager>
    {
        [SerializeField] private float _waitTime = 1.0f;

        private Animator _sceneTransition;

        private void Start() => InputManager.Instance.UIActions.CharacterMenu.started += OnInventoryPressed;

        private void OnInventoryPressed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            DataPersistenceManager.Instance.SaveGame();

            if (SceneManager.GetActiveScene().buildIndex == 2)
                StartCoroutine(LoadGameplay());
            else
                StartCoroutine(LoadCharacterMenu());
        }

        private void OnEnable() => SceneManager.sceneLoaded += SceneManager_sceneLoaded;

        private void OnDisable() => SceneManager.sceneLoaded -= SceneManager_sceneLoaded;

        private void SceneManager_sceneLoaded(Scene ascenerg0, LoadSceneMode mode) => _sceneTransition = GameObject.FindWithTag("SceneLoader").GetComponentInChildren<Animator>();

        public void ReloadScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        public void LoadMainMenu()
        {
            DataPersistenceManager.Instance.SaveGame();

            SceneManager.LoadScene(0);
        }

        private IEnumerator LoadCharacterMenu()
        {
            _sceneTransition.SetTrigger("Start");

            yield return new WaitForSeconds(_waitTime);

            SceneManager.LoadSceneAsync(2);
        }

        public IEnumerator LoadGameplay()
        {
            _sceneTransition.SetTrigger("Start");

            yield return new WaitForSeconds(_waitTime);

            SceneManager.LoadScene(1);
        }

        public int GetActiveScene()
        {
            return SceneManager.GetActiveScene().buildIndex;
        }
    }
}
