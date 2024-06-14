using UnityEngine;
using UnityEngine.SceneManagement;

namespace GunarajCode
{
    public class PlayerManager : Singleton<PlayerManager>
    {
        private GameObject _gameplayPlayer;
        public GameObject Player { get { return _gameplayPlayer; } }

        protected override void Awake()
        {
            base.Awake();
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        }

        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            _gameplayPlayer = GameObject.FindGameObjectWithTag("Player");
        }
    }
}
